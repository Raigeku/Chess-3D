using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawn : MonoBehaviour
{
    // Dichiarazione colori e prefab
    [SerializeField] Material whiteColor;
    [SerializeField] Material blackColor;
    [SerializeField] private GameObject[] piecesPrefabs;

    // Dizionario da string a GameObject per associare il tipo di un pezzo ad un prefab
    Dictionary<string, GameObject> namePieceDict = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (GameObject piece in piecesPrefabs) {
            namePieceDict.Add(piece.GetComponent<Piece>().GetType().ToString(), piece);
        }
    }

    // Istanzia un pezzo in base al tipo passato come parametro, creandolo come GameObject
    public GameObject SpawnPiece(Type type) {
        GameObject prefab = namePieceDict[type.ToString()];
        if (prefab) {
            GameObject piece = Instantiate(prefab);
            return piece;
        }
        return null;
    }

    public Material GetTeamMaterial(Team team) {
        return team == Team.White ? whiteColor : blackColor;
    }
}
