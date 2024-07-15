using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using GDC.Enums;

public class SolveSystem : MonoBehaviour
{
    private List<Vector3> moveListTmp = new List<Vector3>();
    private List<Vector3> moveList = new List<Vector3>();
    [SerializeField] private GameObject baseHint;
    [SerializeField] private Transform availableMovePrefab;
    [SerializeField] private List<Vector3> temp;

    public static SolveSystem Instance { get; private set; }

    [Button]
    private void Test()
    {
        ShowHintMove();
    }

    private bool GetTurn()
    {
        return GameplayManager.Instance.enemyTurn;
    }

    public void ShowHintMove()
    {
        
        if (moveList.Count <= 0) return;

        if (!GetTurn())
        {
            Debug.Log("ShowHint");
            Vector3 target = moveList.ElementAt(0);
            GameObject moveTarget = Instantiate(availableMovePrefab, target, Quaternion.identity).gameObject;

            TileType tileType = GameUtils.GetTileBelowObject(target);

            switch (tileType)
            {
                case TileType.GROUND:
                    moveTarget.transform.localScale = new Vector3(1, 1, 1);
                    moveTarget.transform.rotation = Quaternion.Euler(90, 0, 0);
                    break;
                case TileType.BOX:
                    moveTarget.transform.localScale = new Vector3(1, 1, 1);
                    moveTarget.transform.rotation = Quaternion.Euler(90, 0, 0);
                    break;
                case TileType.SLOPE_0:
                    moveTarget.transform.position += new Vector3(0f, -0.48f, 0.05f);
                    moveTarget.transform.localScale = new Vector3(1, 1.4f, 1);
                    moveTarget.transform.rotation = Quaternion.Euler(45, 180, 0);
                    break;
                case TileType.SLOPE_90:
                    moveTarget.transform.position += new Vector3(-0.05f, -0.48f, 0f);
                    moveTarget.transform.localScale = new Vector3(1, 1.4f, 1);
                    moveTarget.transform.rotation = Quaternion.Euler(45, 90, 0);
                    break;
                case TileType.SLOPE_180:
                    moveTarget.transform.position += new Vector3(0f, -0.48f, -0.05f);
                    moveTarget.transform.localScale = new Vector3(1, 1.4f, 1);
                    moveTarget.transform.rotation = Quaternion.Euler(45, 0, 0);
                    break;
                case TileType.SLOPE_270:
                    moveTarget.transform.position += new Vector3(0.05f, -0.48f, 0f);
                    moveTarget.transform.localScale = new Vector3(1, 1.4f, 1);
                    moveTarget.transform.rotation = Quaternion.Euler(45, 270, 0);
                    break;
            }

            moveTarget.transform.SetParent(baseHint.transform);
        }

        else
        {
            Debug.Log("HideHint");
            DestroyAllChildren(baseHint.gameObject);
            moveListTmp.Add(moveList[0]);
            moveList.RemoveAt(0);
        }
    }

    private void Awake()
    {
        Instance = this;
        moveList = CopyList(temp);
    }

    public void DestroyAllChildren(GameObject parent)
    {
        // Loop through each child of the parent GameObject
        foreach (Transform child in parent.transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }

    private List<Vector3> CopyList(List<Vector3> target)
    {
        List<Vector3> result = new List<Vector3>();

        foreach (var item in target)
        {
            var newItem = new Vector3(item.x, item.y, item.z);
            result.Add(newItem);
        }

        return result;
    }

}
