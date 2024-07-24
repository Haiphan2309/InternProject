using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonObject : MonoBehaviour
{
    private List<ToggleBlock> toggleBlocks = new List<ToggleBlock>();
    void Setup(List<ToggleBlock> blocks)
    {
        toggleBlocks = blocks;
    }    
}
