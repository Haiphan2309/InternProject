using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManConfig : ScriptableObject
{
    private GameObject _prefab;
    private float _height;
    private GDC.Enums.ChessManType _chessManType;

    public GameObject prefab { get; set; }
    public float height { get; set; }
    public GDC.Enums.ChessManType chessManType { get; set; }

    public virtual List<Vector3> Move()
    {
        return null;
    }
}
