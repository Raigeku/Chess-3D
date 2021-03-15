using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlightCreator : MonoBehaviour
{
    [SerializeField] private Material normalHighlightMaterial;
    [SerializeField] private Material enemyHighlightMaterial;
    [SerializeField] private GameObject highlightPrefab;
    private List<GameObject> instantiatedHighlight = new List<GameObject>();

    public void ShowHighlight(Dictionary<Vector3, bool> tileData) {
        ClearHighlight();
        foreach (var data in tileData) {
            GameObject highlight = Instantiate(highlightPrefab, data.Key, Quaternion.identity);
            instantiatedHighlight.Add(highlight);
            foreach (var setter in highlight.GetComponentsInChildren<MaterialSetter>()) {
                setter.SetMaterial(data.Value ? normalHighlightMaterial : enemyHighlightMaterial);
            }
        }
    }

    public void ClearHighlight()
    {
        foreach (GameObject highlight in instantiatedHighlight)
        {
            Destroy(highlight.gameObject);
        }
    }
}
