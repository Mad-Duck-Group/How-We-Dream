using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SkillNodeUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SkillNodeSO skillNode;
    [SerializeField] private Image selectionImage;
    [SerializeField] private Sprite unavailableSprite;
    [SerializeField] private Sprite availableSprite;
    [SerializeField] private Sprite acquiredSprite;
    private Image image;
    private UILineConnector lineConnector;
    private PositionConstraint positionConstraint;
    private Tooltip tooltip;
   
    private void OnEnable()
    {
        SkillNodeSO.OnSkillAcquired += OnSkillAcquire;
        SkillNodeSO.OnSkillUnlock += OnSkillUnlock;
        skillNode.ParentUI = this;
    }

    private void OnDisable()
    {
        SkillNodeSO.OnSkillAcquired -= OnSkillAcquire;
        SkillNodeSO.OnSkillUnlock -= OnSkillUnlock;
        skillNode.ParentUI = null;
    }

    private void OnDestroy()
    {
        skillNode.ParentUI = null;
    }

    private void OnSkillUnlock(SkillNodeSO skillnode)
    {
        if (skillnode != skillNode) return;
        image.sprite = availableSprite;
    }
    
    private void OnSkillAcquire(SkillNodeSO node)
    {
        if (node != skillNode) return;
        image.sprite = acquiredSprite;
        GlobalSoundManager.Instance.PlayUISFX("SkillUnlocked");
    }
    
    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        lineConnector = GetComponentInChildren<UILineConnector>();
        positionConstraint = GetComponentInChildren<PositionConstraint>();
        tooltip = GetComponent<Tooltip>();
        tooltip.TooltipObject = skillNode.Skill;
        skillNode.ParentUI = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        image.sprite = skillNode.Unlocked ? availableSprite : unavailableSprite;
        image.sprite = skillNode.Acquired ? acquiredSprite : image.sprite;
        Transform scrollRect = GetComponentInParent<ScrollRect>().transform;
        positionConstraint.AddSource(new ConstraintSource {sourceTransform = scrollRect.parent, weight = 1});
        positionConstraint.translationOffset = Vector3.zero;
        positionConstraint.locked = true;
        positionConstraint.constraintActive = true;
        if (skillNode.NextNodes.Count == 0) return;
        foreach (var node in skillNode.NextNodes)
        {
            lineConnector.transforms.Add(transform as RectTransform);
            lineConnector.transforms.Add(node.ParentUI.transform as RectTransform);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        bool locked = !skillNode.Unlocked;
        if (skillNode.Acquired) locked = true;
        if (ProgressionManager.Instance.SkillPoints < skillNode.Skill.SkillCost) locked = true;
        if (locked)
        {
            GlobalSoundManager.Instance.PlayUISFX("Locked");
            return;
        }
        skillNode.Acquire();
        ProgressionManager.Instance.SkillPoints -= skillNode.Skill.SkillCost;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectionImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectionImage.enabled = false;
    }
}
