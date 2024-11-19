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

public class SkillNodeUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SkillNodeSO skillNode;
    private Image image;
    private UILineConnector lineConnector;
    private PositionConstraint positionConstraint;
   
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
        image.color = Color.white;
    }
    
    private void OnSkillAcquire(SkillNodeSO node)
    {
        if (node != skillNode) return;
        image.color = Color.green;
    }
    
    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        lineConnector = GetComponentInChildren<UILineConnector>();
        positionConstraint = GetComponentInChildren<PositionConstraint>();
        skillNode.ParentUI = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        image.color = skillNode.Unlocked ? Color.white : Color.gray;
        image.color = skillNode.Acquired ? Color.green : image.color;
        Transform scrollRect = GetComponentInParent<ScrollRect>().transform;
        positionConstraint.AddSource(new ConstraintSource {sourceTransform = scrollRect, weight = 1});
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
        if (!skillNode.Unlocked) return;
        if (!ProgressionManager.Instance.CanUpgradeSkill) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        skillNode.Acquire();
        ProgressionManager.Instance.CanUpgradeSkill = false;
    }
}
