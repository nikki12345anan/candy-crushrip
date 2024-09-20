using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Video;

public class CameraScale : MonoBehaviour
{
    private Board board;
    public float CameraOffSet;
    public float aspectRatio = 0.625f;
    public float padding = 2;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if(board != null)
        {
            RepositionCamera(board.width + 12, board.height- 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 TempPosition = new Vector3(x / 2,y / 2, CameraOffSet);
        transform.position = TempPosition;
        if(board.width >= board.height)
        {
            Camera.main.orthographicSize = (board.width / 2 + padding) / aspectRatio;

        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
