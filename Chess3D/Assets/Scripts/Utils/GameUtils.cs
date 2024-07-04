using GDC.Enums;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class GameUtils
{
    public static bool CompareVector3(Vector3 v1, Vector3 v2)
    {
        return (Mathf.RoundToInt(v1.x) == Mathf.RoundToInt(v2.x)
             && Mathf.RoundToInt(v1.y) == Mathf.RoundToInt(v2.y)
             && Mathf.RoundToInt(v1.z) == Mathf.RoundToInt(v2.z));
    }

    /// <summary>
    /// Return the position snap to grid 
    /// </summary>
    /// <param name="position">Vector3 position</param>
    public static Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
    }

    /// <summary>
    /// Check the TileType is slope or not
    /// </summary>
    /// <param name="tileType">The tile to check</param>
    public static bool CheckSlope(TileType tileType)
    {
        return tileType == TileType.SLOPE_0 || tileType == TileType.SLOPE_90 || tileType == TileType.SLOPE_180 || tileType == TileType.SLOPE_270;
    }

    public static TileType GetTileBelowObject(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y - 1f;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    public static TileType GetTile(Vector3 position)
    {
        float Xpos = position.x;
        float Ypos = position.y;
        float Zpos = position.z;
        return GameplayManager.Instance.levelData.GetTileInfo()[
               (int)Mathf.Round(Xpos),
               (int)Mathf.Round(Ypos),
               (int)Mathf.Round(Zpos)
               ].tileType;
    }

    public static GameObject GetObjectByPosition(Vector3 position, LayerMask layerMask)
    {
        GameObject foundObject = null;
        foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
        {
            if (CompareVector3(obj.transform.position, position) && obj.layer.CompareTo(layerMask.value) == 1)
            {
                foundObject = obj;
                break;
            }
        }

        return foundObject;
    }

    public static GameplayObject GetGameplayObjectByPosition(Vector3 position)
    {
        GameplayObject foundObject = null;
        foreach (GameplayObject obj in Object.FindObjectsOfType<GameplayObject>())
        {
            if (CompareVector3(obj.transform.position, position))
            {
                foundObject = obj.GetComponent<GameplayObject>();
                break;
            }
        }

        return foundObject;
    }

    public static bool InBound(Vector3 currentMove)
    {
        bool inBound = true;
        float Xpos = currentMove.x;
        float Ypos = currentMove.y;
        float Zpos = currentMove.z;

        if (Xpos < 0 || Xpos >= GDC.Constants.GameConstants.MAX_X_SIZE)
        {
            inBound = false;
        }
        if (Ypos <= 0 || Ypos >= GDC.Constants.GameConstants.MAX_Y_SIZE)
        {
            inBound = false;
        }
        if (Zpos < 0 || Zpos >= GDC.Constants.GameConstants.MAX_Z_SIZE)
        {
            inBound = false;
        }
        // Debug.Log("InBound = " + inBound);
        return inBound;
    }
}
