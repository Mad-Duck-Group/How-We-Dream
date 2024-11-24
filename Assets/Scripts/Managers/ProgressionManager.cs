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
    public int SkillPoints { get => skillPoints; set => skillPoints = value; }

    public float Progress
    {
        get
        {
            if (CurrentLevel.Endless) return 1;
            return (float)currentLevelIndex / levels.Count;
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        currentLevelIndex = Mathf.Clamp(currentLevelIndex, 0, levels.Count - 1);
        if (currentLevelIndex >= levels.Count) currentLevelIndex = 0;
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.GamePlay, LoadSceneMode.Single, false);
    }
    
    public void ReplayLevel()
    {
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.GamePlay, LoadSceneMode.Single, false);
    }

    [Button("Reset Skill")]
    private void ResetSkill()
    {
        skillNodes.ForEach(node => node.Reset());
    }
    
    [Button("Reset VN")]
    private void ResetVN()
    {
        vnPaths.ForEach(path => path.Reset());
    }
}
