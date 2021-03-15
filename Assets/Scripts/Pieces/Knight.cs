using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    // Salti ad L del Cavallo
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(2,1),
        new Vector2Int(2,-1),
        new Vector2Int(-2,1),
        new Vector2Int(-2,-1),
        new Vector2Int(1,2),
        new Vector2Int(1,-2),
        new Vector2Int(-1,2),
        new Vector2Int(-1,-2)
    };

    public override List<Vector2Int> SelectAvailableTile()
    {
        // Pulisce la lista di ogni mossa possibile, cosi che non rimangano le mosse possibili precedenti
        possibleMoves.Clear();

        // Per ogni direzione cicla e aggiunge alle mosse possibili ogni tile eccetto quando incontra il limite o un pezzo alleato
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int coord = occupiedTile + directions[i];
            Piece piece = chessboard.GetPieceOnTile(coord);

            if (chessboard.NotOutOfBound(coord))
            {
                if (piece == null || !piece.IsSameTeam(this)) AddMove(coord);
            }
        }
        return possibleMoves;
    }
}
