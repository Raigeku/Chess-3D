using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(TileHighlightCreator))]
public class Chessboard : MonoBehaviour
{
    private Piece[,] grid;
    private Piece currentPiece;
    public GameController gameController;
    private TileHighlightCreator tileHighlightCreator;
    public Vector2Int enPassant = new Vector2Int(-1, -1);
    public Piece enPassantPiece;
    public bool enPassantPossible;
    private void Awake()
    {
        tileHighlightCreator = GetComponent<TileHighlightCreator>();
        grid = new Piece[8, 8];

    }

    public Vector3 FromCoordToPosition(Vector2Int coord) {
        Vector3 position = new Vector3(coord.x, 0, coord.y);
        return position;
    }

    public Vector2Int FromPositionToCoord(Vector3 clickPosition)
    {
        Vector2Int coord = new Vector2Int((int)(clickPosition.x + 0.5f), (int)(clickPosition.z + 0.5f));
        return coord;
    }

    public void SelectedTile(Vector3 clickPosition) {
        // Se la partita non è iniziata non puoi selezionare i tile della chessboard
        if (!gameController.IsStateStarted()) return;

        Vector2Int coord = FromPositionToCoord(clickPosition);
        Piece piece = GetPieceOnTile(coord);
        // Se il tile scelto ha un pezzo che è uguale al pezzo già selezionato allora deseleziona
        // Se il tile scelto ha un pezzo che è diverso dal pezzo scelto ed è del colore del player attuale selezionalo
        // Se il tile scelto è una mossa possibile del pezzo già scelto allora muovi il pezzo, deseleziona e passa il turno
        if (currentPiece)
        {
            if (piece != null && currentPiece == piece)
                DeselectPiece();
            else if (piece != null && currentPiece != piece && gameController.IsCurrentPlayer(piece.team))
                SelectPiece(piece);
            else if (currentPiece.PossibleMove(coord))
                CurrentPieceMoved(coord, currentPiece);
        }
        else {
            if (piece != null && gameController.IsCurrentPlayer(piece.team)){
                SelectPiece(piece);
            }
        }
    }

    // Promuove il pezzo ad un tipo ( da aggiornare con scelta di tipo )
    public void PromotePiece(Piece piece)
    {
        TakePiece(piece);
        gameController.SpawnPieceAndSet(piece.team, typeof(Queen), piece.occupiedTile);
    }

    // 
    private void CurrentPieceMoved(Vector2Int coord, Piece piece)
    {
        // Salva posizione En Passant se il pezzo è un pedone, se no resetta la posizione a -1 -1
        EnPassantSave(piece, coord);
        // Se un pezzo nemico è presente nella coordinata scelta lo cattura
        TakeEnemyPiece(coord);
        // Muove il pezzo alle coordinate e setta il pezzo nella precedente posizione a null
        MoveCurrentPiece(piece, coord, null, piece.occupiedTile);
        currentPiece.MovePiece(coord);
        // Deseleziona il pezzo e finisce il turno
        DeselectPiece();
        gameController.EndTurn();
    }

    // Muove il pezzo alle coordinate e setta il pezzo nella precedente posizione a null
    public void MoveCurrentPiece(Piece piece, Vector2Int coord, Piece oldPiece, Vector2Int oldCoord)
    {
        grid[oldCoord.x, oldCoord.y] = oldPiece;
        grid[coord.x, coord.y] = piece;
    }

    // Elimina tutte le mosse che farebbero catturare il re alleato, poi seleziona il pezzo e mostra le mosse possibili
    private void SelectPiece(Piece piece)
    {
        gameController.DeleteCaptureEnablingMoveOnPiece<King>(piece);
        currentPiece = piece;
        List<Vector2Int> selection = currentPiece.possibleMoves;
        ShowHighlightTile(selection, piece);
    }

    // Se un pezzo nemico è presente nella coordinata scelta lo cattura
    private void TakeEnemyPiece(Vector2Int coord)
    {
        Piece piece = GetPieceOnTile(coord);
        if (piece != null && !currentPiece.IsSameTeam(piece))
            TakePiece(piece);
    }

    // Elimina il pezzo logicamente e fisicamente
    private void TakePiece(Piece piece)
    {
        if (piece) {
            grid[piece.occupiedTile.x, piece.occupiedTile.y] = null;
            gameController.OnPieceDeleted(piece);
        }
    }

    // Mostra gli highlight seguendo un dizionario che associa un vettore ad un bool, che sta ad indicare se la mossa è una cattura o uno spostamento
    // Ricava la posizione fisica in base alle coordinate logiche mandate, controlla se nel tile scelto è presente un nemico o se è una mossa di En Passant
    // Infine aggiunge al dizionario l'highlight della mossa e lo manda alla funzione ShowHighlight
    private void ShowHighlightTile(List<Vector2Int> selection, Piece piece)
    {
        Dictionary<Vector3, bool> tileData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++)
        {
            Vector3 position = FromCoordToPosition(selection[i]);
            bool isTileNull = GetPieceOnTile(selection[i]) == null;
            Vector2Int enPassantOffset = piece.team == Team.White ? new Vector2Int(enPassant.x, enPassant.y + 1) : new Vector2Int(enPassant.x, enPassant.y - 1);
            if (isTileNull && selection[i] == enPassantOffset && piece is Pawn && enPassantPossible ) 
                tileData.Add(position, false);
            else
                tileData.Add(position, isTileNull);
        }
        tileHighlightCreator.ShowHighlight(tileData);
    }
    // Resetta il currentPiece e distrugge gli highlights
    private void DeselectPiece()
    {
        currentPiece = null;
        tileHighlightCreator.ClearHighlight();
    }
    // Ritorna il pezzo presente nelle coordinate della scacchiera
    public Piece GetPieceOnTile(Vector2Int coord)
    {
        
        if(NotOutOfBound(coord))
            return grid[coord.x, coord.y];
        return null;
    }
    // Setta un pezzo alle coordinate della scacchiera
    public void SetPiece(Vector2Int tileCoord, Piece piece)
    {
        if (NotOutOfBound(tileCoord)) {
            grid[tileCoord.x, tileCoord.y] = piece;
        }
    }
    // Controlla se le coordinate vanno oltre la scacchiera
    public bool NotOutOfBound(Vector2Int coord) {
        if (coord.x < 8 && coord.y < 8 && coord.x >= 0 && coord.y >= 0) return true;
        return false;
    }
    // Controlla se nel tile è presente un pezzo
    public bool TileHasPiece(Piece piece) {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (grid[i, j] == piece)
                    return true;
            }
        }
        return false;
    }
    // Resetta la scacchiera e il pezzo scelto
    public void OnRestart() {
        currentPiece = null;
        grid = new Piece[8, 8];
    }

    private void EnPassantSave(Piece piece, Vector2Int coord)
    {
        // Se è un pedone e la sua prima mossa è muoversi di 2 in avanti allora verrà salvata la posizione nuova per rendere disponibile l'azione di En Passant
        if (piece.GetType() == typeof(Pawn))
        {
            if (piece.occupiedTile.y == 1 && coord.y == 3 && piece.team == Team.White)
                SetEnPassant(coord, piece, true);
            else if (piece.occupiedTile.y == 6 && coord.y == 4 && piece.team == Team.Black)
                SetEnPassant(coord, piece, true);
            else enPassantPossible = false;
        }
        else
            SetEnPassant(new Vector2Int(-1, -1), null, false);
    }

    private void SetEnPassant(Vector2Int coord, Piece piece, bool isEnPassantPossible) {
        enPassant = coord;
        enPassantPiece = piece;
        enPassantPossible = isEnPassantPossible;
    }
    // Se è stata fatta una mossa di En Passant allora cattura il pezzo su cui era possibile la mossa e resetta le variabili dell'En Passant 
    public void OnEnPassant(Vector2Int coord, Team team)
    {
        bool checkWhite = coord.x == enPassant.x && coord.y - 1 == enPassant.y && team == Team.White;
        bool checkBlack = coord.x == enPassant.x && coord.y + 1 == enPassant.y && team == Team.Black;
        if (checkWhite || checkBlack) 
            TakeEnPassant();
    }

    public void TakeEnPassant() {
        TakeEnemyPiece(enPassant);
        SetEnPassant(new Vector2Int(-1, -1), null, false);
    }
}
