using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float bumpScale = 0.1f;
    private Tween bumpTween;
    private Button button;
    
    private float originalScale;

    private void Awake()
    {
        originalScale = transform.localScale.x;
        button = GetComponent<Button>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        if (bumpTween.IsActive()) bumpTween.Kill();
        bumpTween = transform.DOScale(bumpScale, 0.2f).SetRelative();
        GlobalSoundManager.Instance.PlayUISFX("MousePoint");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (bumpTween.IsActive()) bumpTween.Kill();
        bumpTween = transform.DOScale(originalScale, 0.2f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!button.interactable) return;
        GlobalSoundManager.Instance.PlayUISFX("MouseClick");
    }
}
