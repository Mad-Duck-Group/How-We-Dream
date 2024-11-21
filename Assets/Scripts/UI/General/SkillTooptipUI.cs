using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTooptipUI : MonoBehaviour
{
    [SerializeField] private TMP_Text skillName;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text skillCost;
    public void SetTooltip(Skill skill)
    {
        skillName.text = skill.SkillName;
        description.text = skill.Description;
        skillCost.text = $"Skill Cost: {skill.SkillCost}";
    }
}
