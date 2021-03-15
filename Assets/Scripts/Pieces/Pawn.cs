using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> SelectAvailableTile()
    {
        // Pulisce la lista di ogni mossa possibile, cosi che non rimangano le mosse possibili precedenti
        possibleMoves.Clear();

        // Direzione in base al colore ( se bianco su, se nero giù )
        Vector2Int direction = team == Team.White ? Vector2Int.up : Vector2Int.down;

        // Se il pezzo si è mai mosso allora potrà fare soltanto un passo in avanti, se no 2
        int range = hasEverMoved ? 1 : 2;

        // Per ogni direzione cicla e aggiunge alle mosse possibili ogni tile fino a quando non incontra il limite, un pezzo alleato o un pezzo avversario
        for (int i = 1; i <= range; i++)
        {
            Vector2Int coord = occupiedTile + direction * i;
            Piece piece = chessboard.GetPieceOnTile(coord);
            if (!chessboard.NotOutOfBound(coord))
                break;
            if (piece == null)
                AddMove(coord);
            else if (piece != null) 
                break;
        }

        // Direzioni di cattura e di En Passant
        Vector2Int[] captureDirections = new Vector2Int[]
        {
            new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y)
        };
        Vector2Int[] enPassantDirections = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        // Cicla per trovare un pezzo possibile da catturare o una mossa di En Passant e aggiunge eventualmente alle mosse possibili
        for (int i = 0; i < captureDirections.Length; i++) {
            Vector2Int coord = occupiedTile + captureDirections[i];
            Vector2Int enPassantCoord = occupiedTile + enPassantDirections[i];
            Piece piece = chessboard.GetPieceOnTile(coord);
            if (!chessboard.NotOutOfBound(coord))
                continue;
            if (piece != null && !piece.IsSameTeam(this)) AddMove(coord);

            if (chessboard.enPassant == enPassantCoord && chessboard.enPassantPossible) {
                chessboard.enPassantPiece = chessboard.GetPieceOnTile(enPassantCoord);
                AddMove(coord);
            }
        }

        return possibleMoves;
    }

    // Override del metodo MovePiece per aggiungere eventuale mossa di En Passant e di promozione
    public override void MovePiece(Vector2Int coord)
    {
        base.MovePiece(coord);
        chessboard.OnEnPassant(coord, team);
        TryPromotion();
    }

    // Se arriva all'ultimo tile possibile allora il pezzo si promuove
    private void TryPromotion()
    {
        int endOfBoard = team == Team.White ? 7 : 0;
        if (occupiedTile.y == endOfBoard)
        {
            chessboard.PromotePiece(this);
        }
    }
}