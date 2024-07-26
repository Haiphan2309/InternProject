using GDC.Enums;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBlock : MonoBehaviour
{
    [ReadOnly] public bool isOn;

    [SerializeField] private Material materialOn0;
    [SerializeField] private Material materialOn1;
    [SerializeField] private Material materialOff;
    public void Setup(bool isOn, bool isLoadInit = true)
    {
        this.isOn = isOn;
        if (isOn) SetOn(isLoadInit);
        else SetOff(isLoadInit);
    }
    private void SetOn(bool isLoadInit = true)
    {
        Material[] mat = { materialOn0, materialOn1 };
        this.transform.GetComponent<MeshRenderer>().materials = mat;
        transform.GetComponent<BoxCollider>().enabled = true;

        Vector3 pos = GameUtils.SnapToGrid(transform.position);

        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(pos);

        if (gameplayObject != null) 
        {
            TileType tile = GameUtils.GetTile(GameUtils.SnapToGrid(pos));

            Debug.Log("Tile: " + tile);

            if (tile == TileType.ENEMY_CHESS)
            {
                GameplayManager.Instance.DefeatEnemyChessMan(gameplayObject.index);
            }
            else if (tile == TileType.PLAYER_CHESS)
            {
                GameplayManager.Instance.DefeatPlayerChessMan(gameplayObject.index);
            }
            else if (tile == TileType.BOX)
            {
                GameplayManager.Instance.UpdateTile(pos);
            }

            gameplayObject.Defeated();
        }

        if (!isLoadInit)
        {
            Vector3 posIndex = GameUtils.SnapToGrid(transform.position);
            GameplayManager.Instance.SetTile(posIndex, GDC.Enums.TileType.GROUND);
        }
    }
    private void SetOff(bool isLoadInit = true)
    {
        Material[] mat = { materialOff };
        this.transform.GetComponent<MeshRenderer>().materials = mat;

        transform.GetComponent<BoxCollider>().enabled = false;

        if (!isLoadInit)
        {
            Vector3 posIndex = GameUtils.SnapToGrid(transform.position);
            GameplayManager.Instance.SetTile(posIndex, GDC.Enums.TileType.NONE);
        }

        Vector3 upperPos = GameUtils.SnapToGrid(transform.position) + Vector3.up;

        GameplayObject gameplayObject = GameUtils.GetGameplayObjectByPosition(upperPos);

        if (gameplayObject != null) gameplayObject.Drop();
    }
    public void Toggle()
    {
        Setup(!isOn, false);
    }
}
