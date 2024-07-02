using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayObject : MonoBehaviour
{
    public float defaultSpeed;
    public Outline outline;

    public virtual void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger = false)
    {
        Debug.Log("A");
    }
    public virtual void MoveAnim(Vector3 posIndexToMove, float speed, bool isRoundInteger = false, ChessMan chessManAbove = null)
    {
    }

    public virtual void DestroyAnim()
    {

    }
}
