using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : GameplayObject
{
    [SerializeField, ReadOnly] private List<ToggleBlock> toggleBlocks = new List<ToggleBlock>();
    [SerializeField, ReadOnly] private bool isActive = false;
    public void Setup(List<ToggleBlock> blocks)
    {
        toggleBlocks = blocks;
        posIndex = transform.position;
        targetPosition = posIndex;
    }
    [Button]
    private void ActiveButton()
    {
        if (isActive == true) return;

        //isActive = true;
        foreach (ToggleBlock block in toggleBlocks)
        {
            block.Toggle();
        }
    }
    [Button]
    private void InactiveButton()
    {
        if (isActive == false) return;

        //isActive = false;
        foreach (ToggleBlock block in toggleBlocks)
        {
            block.Toggle();
        }
    }

    //Bien isUndo de check sau khi undo chi set lai bien isActive phu hop
    public void CheckActiveButton(bool isUndo = false) 
    {
        if (GameUtils.GetTile(posIndex) != GDC.Enums.TileType.NONE)
        {
            if (isUndo == false)
            {
                ActiveButton();
            }
            isActive = true;
        }    
        else
        {
            if (isUndo == false)
            {
                InactiveButton();
            }
            isActive = false;
        }
    }

}
