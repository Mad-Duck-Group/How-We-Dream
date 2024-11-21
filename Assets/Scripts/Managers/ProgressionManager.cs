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
    public LevelSO CurrentLevel => levels[currentLevelIndex];
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Button("Reset Skill")]
    private void ResetSkill()
    {
        skillNodes.ForEach(node => node.Reset());
    }
}
