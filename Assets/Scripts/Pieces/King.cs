using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    // Direzioni possibili del re
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)
    };
    // Dichiarazione variabili necessarie per arrocco
    private Vector2Int leftCastle, rightCastle;
    private Piece leftRook, rightRook;

    // Se il pezzo non si è mai mosso prende le torri a destra e a sinistra e aggiunge come mossa possibile l'arrocco nella direzione disponibile
    private void CastleMove() {
        if (hasEverMoved) 
            return;
        leftRook = GetRookInDirection<Rook>(team, Vector2Int.left);
        rightRook = GetRookInDirection<Rook>(team, Vector2Int.right);
        if (leftRook && !leftRook.hasEverMoved) {
            leftCastle = occupiedTile + Vector2Int.left * 2;
            possibleMoves.Add(leftCastle);
        }
        if (rightRook && !rightRook.hasEverMoved)
        {
            rightCastle = occupiedTile + Vector2Int.right * 2;
            possibleMoves.Add(rightCastle);
        }
    }
    // Cicla fino ad arrivare al limite o ad un pezzo, che restituisce solo se è del tipo chiesto
    private Piece GetRookInDirection<T>(Team team, Vector2Int direction)
    {
        for (int i = 1; i <= 8; i++) {
            Vector2Int nextCoord = occupiedTile + direction * i;
            Piece piece = chessboard.GetPieceOnTile(nextCoord);
            if (!chessboard.NotOutOfBound(nextCoord))
                return null;
            if (piece != null) {
                if (piece.team != team || !(piece is T))
                    return null;
                else if (piece.team == team && piece is T)
                    return piece;
            }
        }
        return null;
    }

    public override List<Vector2Int> SelectAvailableTile()
    {
        possibleMoves.Clear();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int coord = occupiedTile + direction ;
            Piece piece = chessboard.GetPieceOnTile(coord);
            if (!chessboard.NotOutOfBound(coord)) continue;
            if (piece == null) AddMove(coord);
            else if (!piece.IsSameTeam(this))
                AddMove(coord);
        }
        CastleMove();
        return possibleMoves;
    }

    public override void MovePiece(Vector2Int coord)
    {
        base.MovePiece(coord);
        if (coord == leftCastle) {
            chessboard.MoveCurrentPiece(leftRook, coord + Vector2Int.right, null, leftRook.occupiedTile);
            leftRook.MovePiece(coord + Vector2Int.right);
        }else if (coord == rightCastle)
        {
            chessboard.MoveCurrentPiece(rightRook, coord + Vector2Int.left, null, rightRook.occupiedTile);
            rightRook.MovePiece(coord + Vector2Int.left);
        }
    }
}
