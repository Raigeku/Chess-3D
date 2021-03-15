using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    // Dichiarazione colore, scacchiera e pezzi attivi
    public Team team;
    public Chessboard chessboard;
    public List<Piece> activePieces;
    
    // Metodo costruttore per creare il player nel game controller
    public Player(Team team, Chessboard chessboard) {
        this.chessboard = chessboard;
        this.team = team;
        activePieces = new List<Piece>();
    }

    public void AddPiece(Piece piece) {
        if (activePieces.Contains(piece) == false) {
            activePieces.Add(piece);
        }
    }

    public void DeletePiece(Piece piece)
    {
        if (activePieces.Contains(piece))
        {
            activePieces.Remove(piece);
        }
    }

    public void GeneratePossibleMoves() {
        foreach (Piece piece in activePieces)
        {
            piece.SelectAvailableTile();
        }
    }

    // Cerca tutti i pezzi che stanno attaccando un pezzo di tipo T e li restituisce come array di Piece
    public Piece[] GetAttackingEnemyPiece<T>()
    {
        List<Piece> attackingPieces = new List<Piece>();
        foreach (Piece piece in activePieces)
        {
            if(piece.IsAttackingEnemyPiece<T>())
            attackingPieces.Add(piece);
        }
        return attackingPieces.ToArray();
    }
    // Cerca tutti i pezzi di tipo T e li restituisce come array di Piece
    public Piece[] GetPieces<T>()
    {
        List<Piece> pieces = new List<Piece>();
        foreach (Piece piece in activePieces)
        {
            if (piece is T)
                pieces.Add(piece);
        }
        return pieces.ToArray();
    }
    
    public void OnRestart()
    {
        activePieces.Clear();
    }

    // Elimina tutte le mosse possibili che permettono la cattura del pezzo di tipo T
    public void DeleteCaptureEnablingMoves<T>(Player enemy, Piece currentPiece)
    {
        List<Vector2Int> coordToDelete = new List<Vector2Int>();

        foreach (var coord in currentPiece.possibleMoves) {
            Piece pieceOnTile = chessboard.GetPieceOnTile(coord);
            chessboard.MoveCurrentPiece(currentPiece, coord, null, currentPiece.occupiedTile);
            enemy.GeneratePossibleMoves();
            if (enemy.IsAttackingEnemyPiece<T>())
            {
                coordToDelete.Add(coord);
                // Se il pezzo scelto è il re ed è attaccato a destra o sinistra allora rimuovi l'opzione di arrocco
                if (currentPiece is King && coord == currentPiece.occupiedTile + Vector2Int.left)
                    coordToDelete.Add(currentPiece.occupiedTile + Vector2Int.left * 2);
                else if(currentPiece is King && coord == currentPiece.occupiedTile + Vector2Int.right)
                    coordToDelete.Add(currentPiece.occupiedTile + Vector2Int.right * 2);
            }
            chessboard.MoveCurrentPiece(currentPiece, currentPiece.occupiedTile, pieceOnTile, coord);
        }
        foreach (var coord in coordToDelete) 
        {
            currentPiece.possibleMoves.Remove(coord);
        }
    }

    // Se il pezzo sta attaccando un pezzo di tipo T ritorna true
    private bool IsAttackingEnemyPiece<T>()
    {
        foreach (var piece in activePieces) {
            if (chessboard.TileHasPiece(piece) && piece.IsAttackingEnemyPiece<T>())
                return true;
        }
        return false;
    }

    // Se è possibile nascondere il pezzo di tipo T dagli attacchi nemici ritorna true
    public bool CanHidePieceFromAttack<T>(Player enemy)
    {
        foreach (var piece in activePieces)
        {
            foreach (var coord in piece.possibleMoves)
            {
                Piece pieceOnCoord = chessboard.GetPieceOnTile(coord);
                chessboard.MoveCurrentPiece(piece, coord, null, piece.occupiedTile);
                enemy.GeneratePossibleMoves();
                if (!enemy.IsAttackingEnemyPiece<T>())
                {
                    chessboard.MoveCurrentPiece(piece, piece.occupiedTile, pieceOnCoord, coord);
                    return true;
                }
                chessboard.MoveCurrentPiece(piece, piece.occupiedTile, pieceOnCoord, coord);
            }
        }
        return false;
    }
}