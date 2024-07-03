using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtils
{
    public static bool CompareVector3(Vector3 v1, Vector3 v2)
    {
        return (Mathf.RoundToInt(v1.x) == Mathf.RoundToInt(v2.x)
             && Mathf.RoundToInt(v1.y) == Mathf.RoundToInt(v2.y)
             && Mathf.RoundToInt(v1.z) == Mathf.RoundToInt(v2.z));
    }
}
