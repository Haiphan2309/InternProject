using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleBlock : MonoBehaviour
{
    [ReadOnly] public bool isOn;
    public void Setup(bool isOn)
    {
        this.isOn = isOn;
        if (isOn) SetOn();
        else SetOff();
    }
    private void SetOn()
    {
    }
    private void SetOff()
    {

    }
    public void Toggle()
    {
        Setup(!isOn);
    }
}
