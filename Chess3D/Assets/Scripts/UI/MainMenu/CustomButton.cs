using DG.Tweening;
using GDC.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GDC.MainMenu
{
    public class CustomButton : MonoBehaviour
    {
        [SerializeField] Image[] images;
        [SerializeField] RectTransform cloudRect, treeRect;
        [SerializeField] RectTransform button;
        [SerializeField] AnimationCurve animCurve;
        [SerializeField] float rotateMax,rotateDuration;
        //[SerializeField] float delayToAnim;
        //Coroutine corDelay;
        Tween animTween;
        void Start()
        {
            transform.localScale = Vector3.one * 0.8f;
            Delay();
        }

        public void FadeSetUp()
        {
            Image btnColor = GetComponent<Image>();
            btnColor.color = new Color(btnColor.color.r, btnColor.color.g, btnColor.color.b, 0.5f);

            TMP_Text nameText = transform.GetComponentInChildren<TMP_Text>(true);
            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0.5f);
        }

        void Delay()
        {
            animTween?.Kill();
            animTween = button.DOLocalRotate(new Vector3(0, 0, rotateMax), rotateDuration).SetEase(animCurve).SetLoops(-1);
            animTween.Play();
        }

        public void EnterButton()
        {
            foreach (Image image in images)
            {
                image.DOFade(1, 0.5f);
            }
            //float distance = treeRect.anchoredPosition.x;
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_HOLD_ITEM);
            cloudRect?.DOAnchorPosX(0, 2);
            treeRect?.DOAnchorPosX(0, 2);
            animTween?.Kill();
            animTween = button.DOLocalRotate(Vector3.zero, 0.5f);
            animTween.Play();
            transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        public void ExitButton()
        {
            foreach (Image image in images)
            {
                image.DOFade(0.5f, 0.5f);
            }

            cloudRect?.DOAnchorPosX(-300, 2);
            treeRect?.DOAnchorPosX(300, 2);
            //nameText.DOFade(0.5f, 0.5f);
            transform.DOScale(0.8f, 0.5f);

            Delay();
        }

        public void ButtonDown()
        {
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_PRESS_BUTTON);
            foreach (Image image in images)
            {
                image.color = new Color(0.5f, 0.5f, 0.5f, 1);
            }
            transform.localScale = Vector3.one * 0.8f;
        }
        public void ButtonUp()
        {
            foreach (Image image in images)
            {
                image.color = Color.white;
            }
            transform.localScale = Vector3.one;
        }

    }
}