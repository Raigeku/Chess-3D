using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    // Direzioni oblique e dritte
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1),
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down
    };
    public override List<Vector2Int> SelectAvailableTile()
    {
        // Pulisce la lista di ogni mossa possibile, cosi che non rimangano le mosse possibili precedenti
        possibleMoves.Clear();
        // Per ogni direzione cicla e aggiunge alle mosse possibili ogni tile fino a quando non incontra il limite, un pezzo alleato o un pezzo avversario
        // (per quest'ultimo caso viene aggiunto anche l'ultimo tile per rendere possibile la cattura )
        foreach (Vector2Int direction in directions)
        {
            for (int i = 1; i <= 8; i++)
            {
                Vector2Int coord = occupiedTile + direction * i;
                Piece piece = chessboard.GetPieceOnTile(coord);
                if (!chessboard.NotOutOfBound(coord)) break;
                if (piece == null) AddMove(coord);
                else if (!piece.IsSameTeam(this))
                {
                    AddMove(coord);
                    break;
                }
                else if (piece.IsSameTeam(this))
                    break;
            }
        }
        return possibleMoves;
    }
}
