using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private ChessboardLayout startingChessboardLayout;
    [SerializeField] private Chessboard chessboard;

    [Header("UI")]
    [SerializeField] private Button startButton;
    [SerializeField] private Text buttonText;
    [SerializeField] private Text winnerText;
    [SerializeField] private Text currentPlayerText;

    enum State {
        Initialization=0, Started=1, Ended=2
    }

    private PieceSpawn pieceSpawn;
    private Player whitePlayer;
    private Player blackPlayer;
    private Player currentPlayer;
    private State state;

    // Aggiunge al tasto start la funzione StartGame e crea i 2 Player
    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        pieceSpawn = GetComponent<PieceSpawn>();
        whitePlayer = new Player(Team.White, chessboard);
        blackPlayer = new Player(Team.Black, chessboard);
    }

    // Setta lo state in inizializzazione, disabilità l'UI, spawna i pezzi e crea ogni mossa possibile per il currentPlayer, infine setta lo state come Started
    private void StartGame() {
        state = State.Initialization;
        UIStart();
        chessboard.gameController = this;
        SpawnPiecesFromLayout(startingChessboardLayout);
        currentPlayer = whitePlayer;
        GeneratePossibleMovesPlayer(currentPlayer);
        state = State.Started;
    }

    // Disabilita l'UI, distrugge tutti i pezzi attivi e restarta player e board. Infine richiama StartGame
    private void RestartGame() {
        startButton.gameObject.SetActive(false);
        DestroyActivePieces();
        chessboard.OnRestart();
        whitePlayer.OnRestart();
        blackPlayer.OnRestart();
        StartGame();
    }

    private void DestroyActivePieces()
    {
        foreach (Piece piece in whitePlayer.activePieces) 
            Destroy(piece.gameObject);
        foreach (Piece piece in blackPlayer.activePieces) 
            Destroy(piece.gameObject);
    }

    public bool IsStateStarted() {
        return state == State.Started;
    }
    // Genera i pezzi seguendo il layout in base all'index, settando team, tipo e coordinate
    private void SpawnPiecesFromLayout(ChessboardLayout layout) {
        foreach (var piece in layout.boardSquares)
        {
            Vector2Int offset = new Vector2Int(-1, -1);
            Team team = piece.team;
            string typeString = piece.pieceType.ToString();
            Vector2Int tileCoord = piece.position + offset;

            Type type = Type.GetType(typeString);
            SpawnPieceAndSet(team, type, tileCoord);
        }
    }

    // Genera un pezzo in base al tipo passato come parametro, settando le variabili principali e assegnandolo ai pezzi attivi del player
    public void SpawnPieceAndSet(Team team, Type type, Vector2Int tileCoord)
    {
        Piece newPiece = pieceSpawn.SpawnPiece(type).GetComponent<Piece>();
        newPiece.SetData(tileCoord, team, chessboard);
        Material teamMaterial = pieceSpawn.GetTeamMaterial(team);
        newPiece.SetPieceMaterial(teamMaterial);

        chessboard.SetPiece(tileCoord, newPiece);
        Player currentPlayer = team == Team.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }
    public bool IsCurrentPlayer(Team team)
    {
        return currentPlayer.team == team;
    }
    // Ogni fine turno genera mosse possibili per il currentPlayer e per il nemico, infine o finisce la partita o cambia il currentPlayer
    public void EndTurn()
    {
        GeneratePossibleMovesPlayer(currentPlayer);
        GeneratePossibleMovesPlayer(GetEnemyPlayer(currentPlayer));
        if (IsGameOver())
            EndGame();
        else
            ChangeCurrentPlayer();
    }

    // Se il re è attaccato da qualsiasi pezzo del currentPlayer, il re nemico non può muoversi e nessun pezzo può nasconderlo dall'attacco allora finisce la partita per scacco matto
    private bool IsGameOver()
    {
        Piece[] piecesAttackingKing = currentPlayer.GetAttackingEnemyPiece<King>();
        if (piecesAttackingKing.Length > 0) {
            Player enemyPlayer = GetEnemyPlayer(currentPlayer);
            Piece enemyKing = enemyPlayer.GetPieces<King>()[0];
            enemyPlayer.DeleteCaptureEnablingMoves<King>(currentPlayer, enemyKing);

            if (enemyKing.possibleMoves.Count == 0) {
                bool IsKingCoverable = enemyPlayer.CanHidePieceFromAttack<King>(currentPlayer);
                if (!IsKingCoverable) return true;
            }
        }
        return false;
    }
    // Setta lo state a Ended, abilità l'UI con il vincitore stampato e lo startButton avrà la funzione di Restart
    private void EndGame()
    {
        Debug.Log("Ended");
        state = State.Ended;
        UIRestart();
    }

    private void UIStart() {
        currentPlayerText.gameObject.SetActive(true);
        winnerText.gameObject.SetActive(false);
        buttonText.text = "Restart";
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(RestartGame);
        startButton.gameObject.SetActive(false);
    }
    private void UIRestart()
    {
        currentPlayerText.gameObject.SetActive(false);
        winnerText.text = currentPlayer == whitePlayer ? "Winner is White" : "Winner is Black";
        winnerText.gameObject.SetActive(true);
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(RestartGame);
        startButton.gameObject.SetActive(true);
    }
    private void ChangeCurrentPlayer()
    {
        currentPlayer = currentPlayer == whitePlayer ? blackPlayer : whitePlayer;
        currentPlayerText.text = currentPlayer.team == Team.White ? "White Player" : "Black Player";
    }

    private Player GetEnemyPlayer(Player player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    public void OnPieceDeleted(Piece piece)
    {
        Player piecePlayer = (piece.team == Team.White) ? whitePlayer : blackPlayer;
        piecePlayer.DeletePiece(piece);
        Destroy(piece.gameObject);
    }

    private void GeneratePossibleMovesPlayer(Player currentPlayer)
    {
        currentPlayer.GeneratePossibleMoves();
    }

    public void DeleteCaptureEnablingMoveOnPiece<T>(Piece piece) {
        currentPlayer.DeleteCaptureEnablingMoves<T>(GetEnemyPlayer(currentPlayer), piece);
    }
}