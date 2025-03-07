using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressionManager : PersistentMonoSingleton<ProgressionManager>
{
    [SerializeField] private List<LevelSO> levels;
    [SerializeField] private int currentLevelIndex;
    [SerializeField] private List<SkillNodeSO> skillNodes;
    [SerializeField] private int skillPoints;
    [SerializeField] private List<VNPathSO> vnPaths;
    public LevelSO CurrentLevel => levels[currentLevelIndex];
    public int CurrentLevelIndex => currentLevelIndex;
    public int SkillPoints
    {
        get => skillPoints;
        set
        {
            skillPoints = value;
            OnSkillPointChanged?.Invoke(skillPoints);
        }
    }

    public float Progress
    {
        get
        {
            if (CurrentLevel.Endless) return 1;
            return (float)currentLevelIndex / levels.Count;
        }
    }

    public delegate void SkillPointChanged(int points);
    public static event SkillPointChanged OnSkillPointChanged;

    public void NextLevel()
    {
        currentLevelIndex++;
        //currentLevelIndex = Mathf.Clamp(currentLevelIndex, 0, levels.Count - 1);
        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
            SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.MainMenu, LoadSceneMode.Additive, false);
            return;
        }
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.Gameplay, LoadSceneMode.Single, false);
    }
    
    public void ReplayLevel()
    {
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.Gameplay, LoadSceneMode.Single, false);
    }

    [Button("Reset Skill")]
    public void ResetSkill()
    {
        skillNodes.ForEach(node => node.Reset());
        skillPoints = 0;
    }
    
    [Button("Reset VN")]
    public void ResetVN()
    {
        vnPaths.ForEach(path => path.Reset());
    }
}
