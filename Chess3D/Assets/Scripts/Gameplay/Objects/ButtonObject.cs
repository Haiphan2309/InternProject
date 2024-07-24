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
    }
    [Button]
    public void ActiveButton()
    {
        if (isActive == true) return;

        isActive = true;
        foreach (ToggleBlock block in toggleBlocks)
        {
            block.Toggle();
        }
    }
    [Button]
    public void InActiveButton()
    {
        if (isActive == false) return;

        isActive = false;
        foreach (ToggleBlock block in toggleBlocks)
        {
            block.Toggle();
        }
    }
}
