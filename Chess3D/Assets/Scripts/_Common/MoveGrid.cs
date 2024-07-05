using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGrid : MonoBehaviour
{
    [SerializeField] Transform grid;
    [SerializeField] float xAdd, yAdd, zAdd;

    [Button]
    void RepositionGrid()
    {
        for (int i = 0; i < grid.childCount; i++)
        {
            for(int j=0; j<grid.GetChild(i).childCount; j++)
            {
                grid.GetChild(i).GetChild(j).position += new Vector3(xAdd, yAdd, zAdd);
            }
        }
    }
}
