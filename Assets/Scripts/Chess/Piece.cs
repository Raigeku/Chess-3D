using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MaterialSetter))]
public abstract class Piece : MonoBehaviour
{
    //Classe astratta cosi che possa essere espansa per ogni tipo di pezzo

    //Dichiarazione materiale, scacchiera, colore, tile occupato e mosse possibili
    private MaterialSetter materialSetter;
    public Chessboard chessboard;
    public Team team;
    public bool hasEverMoved;
    public Vector2Int occupiedTile;
    public List<Vector2Int> possibleMoves;
    //Dichiarazione metodo astratto da overridare per ogni classe figlia
    public abstract List<Vector2Int> SelectAvailableTile();

    //Assegnamento variabili
    private void Awake()
    {
        possibleMoves = new List<Vector2Int>();
        materialSetter = GetComponent<MaterialSetter>();
        hasEverMoved = false;
    }


    public void SetPieceMaterial(Material material) {
        materialSetter.SetMaterial(material);
    }

    public bool IsSameTeam(Piece piece) {
        return team == piece.team;
    }

    public bool PossibleMove(Vector2Int coord) {
        return possibleMoves.Contains(coord);
    }

    // Metodo virtuale, cosi che sia overridabile in caso di necessità (Pawn, King)
    // Con le coordinate cambia posizione sia logica che fisica del pezzo
    public virtual void MovePiece(Vector2Int coord) {
        Vector3 position = chessboard.FromCoordToPosition(coord);
        occupiedTile = coord;
        hasEverMoved = true;
        transform.position = position;
    }

    // Check se nelle mosse possibili del pezzo un pezzo di tipo T è attaccato ( usato per vedere se il re è in scacco da questo pezzo )
    public bool IsAttackingEnemyPiece<T>()
    {
        foreach (var tile in possibleMoves) {
            if(chessboard.GetPieceOnTile(tile) is T) 
                return true;
        }
        return false;
    }

    public void AddMove(Vector2Int coord) {
        possibleMoves.Add(coord);
    }

    // Setta tutte le variabili principali del pezzo (coordinate, colore e scacchiera)
    public void SetData(Vector2Int coord, Team team, Chessboard chessboard) {
        this.chessboard = chessboard;
        this.team = team;
        occupiedTile = coord;
        transform.position = new Vector3(occupiedTile.x, 0, occupiedTile.y);
    }
}