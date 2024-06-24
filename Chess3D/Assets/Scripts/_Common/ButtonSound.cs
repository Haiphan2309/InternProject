using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AudioPlayer;
using GDC.Managers;

namespace GDC.Common
{
    public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        SoundID buttonHoverID = SoundID.BUTTON_HOVER;
        SoundID buttonClickID = SoundID.BUTTON_CLICK;
        public void OnPointerDown(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySound(buttonClickID, 0.5f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySound(buttonHoverID, 0.5f);
        }
    }
}
