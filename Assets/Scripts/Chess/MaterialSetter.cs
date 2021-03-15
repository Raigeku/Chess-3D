using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    MeshRenderer mesh;

    private MeshRenderer meshRenderer {
        get {
            if (mesh == null) mesh = GetComponent<MeshRenderer>();
            return mesh;
        }
    }
    public void SetMaterial(Material material) {
        meshRenderer.material = material;
    }
}
