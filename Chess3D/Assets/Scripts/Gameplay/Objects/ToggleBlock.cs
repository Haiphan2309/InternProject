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
