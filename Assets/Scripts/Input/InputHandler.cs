using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    Vector3 clickPosition;
    [SerializeField] Chessboard chessboard;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50f, LayerMask.GetMask("ChessBoard"))) {
                clickPosition = hit.point;
                chessboard.SelectedTile(clickPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
}
