using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Bumpable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private bool handleByOtherScript;
    [SerializeField] private float bumpScale = 0.1f;
    [SerializeField] private bool playPointSound = true;
    [SerializeField] private bool playClickSound = true;
    private Tween bumpTween;
    private Selectable selectable;
    private ClickableArea clickableArea;
    
    private float originalScale;

    private void Awake()
    {
        originalScale = transform.localScale.x;
        TryGetComponent(out selectable);
        TryGetComponent(out clickableArea);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handleByOtherScript) return;
        if (selectable && !selectable.interactable) return;
        if (clickableArea && !clickableArea.Interactable) return;
        BumpUp();
    }

    public void BumpUp()
    {
        if (bumpTween.IsActive()) bumpTween.Kill();
        bumpTween = transform.DOScale(bumpScale, 0.2f).SetRelative();
        if (playPointSound) 
            GlobalSoundManager.Instance.PlayUISFX("MousePoint");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (handleByOtherScript) return;
        BumpDown();
    }

    public void BumpDown()
    {
        if (bumpTween.IsActive()) bumpTween.Kill();
        bumpTween = transform.DOScale(originalScale, 0.2f);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (handleByOtherScript) return;
        if (selectable && !selectable.interactable) return;
        if (clickableArea && !clickableArea.Interactable) return;
        Click();
    }

    public void Click()
    {
        if (playClickSound) 
            GlobalSoundManager.Instance.PlayUISFX("MouseClick");
    }
}
