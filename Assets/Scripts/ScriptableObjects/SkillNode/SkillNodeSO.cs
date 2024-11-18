using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillNode", menuName = "ScriptableObjects/SkillNode")]
public class SkillNodeSO : ScriptableObject
{
    [SerializeField] private List<SkillNodeSO> prerequisites;
    public List<SkillNodeSO> Prerequisites => prerequisites;
    [SerializeField] private List<SkillNodeSO> nextNodes;
    public List<SkillNodeSO> NextNodes => nextNodes;
    [SerializeField] private bool unlocked;
    public bool Unlocked {get => unlocked; set => unlocked = value;}
    [SerializeField, ShowIf(nameof(unlocked))] private bool unlockByDefault;
    [SerializeField] private bool acquired;
    public bool Acquired { get => acquired; set => acquired = value; }
    public SkillNodeUI ParentUI { get; set; }
    
    public delegate void SkillUnlock(SkillNodeSO skillNode);
    public static event SkillUnlock OnSkillUnlock;
    public delegate void SkillAcquired(SkillNodeSO skillNode);
    public static event SkillAcquired OnSkillAcquired;
    
    public void Reset()
    {
        if (!unlockByDefault) unlocked = false;
        acquired = false;
        ParentUI = null;
    }
    
    public void Unlock()
    {
        if (unlocked) return;
        unlocked = true;
        OnSkillUnlock?.Invoke(this);
    }
    
    public void Acquire()
    {
        if (acquired) return;
        acquired = true;
        nextNodes.ForEach(node => node.Unlock());
        OnSkillAcquired?.Invoke(this);
    }
}
