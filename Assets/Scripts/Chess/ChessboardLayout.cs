using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Chessboard/Layout")]
public class ChessboardLayout : ScriptableObject
{
    //Classe che identifica ogni tile che si vuole aggiungere nel layout iniziale della scacchiera, serializzabile tramite inspector
    [Serializable]
    public class ChessBoardTileSetup {
        public Team team;
        public PieceType pieceType;
        public Vector2Int position;
    }

    // Dichiarazione di un array di pezzi aggiunti al layout iniziale, cosi che si possa creare tramite inspector un pezzo alla volta il layout iniziale
    [SerializeField] public ChessBoardTileSetup[] boardSquares;
    public int GetPiecesCount() {
        return boardSquares.Length;
    }

    // Ritorna coordinate tramite indice di array
    public Vector2Int GetTileCoordIndex(int index) {
        if (boardSquares.Length <= index)
        {
            Debug.Log("Index fuori array");
            return new Vector2Int(-1, -1);
        }
        return new Vector2Int(boardSquares[index].position.x - 1, boardSquares[index].position.y - 1);
    }

    // Ritorna nome del pezzo tramite indice di array
    public string GetTileNameIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.Log("Index fuori array");
            return "Null";
        }
        return boardSquares[index].pieceType.ToString();
    }

    // Ritorna colore del pezzo tramite indice di array
    public Team GetTileTeamIndex(int index)
    {
        if (boardSquares.Length <= index)
        {
            Debug.Log("Index fuori array");
            return Team.Black;
        }
        return boardSquares[index].team;
    }
}
