using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillAsset", menuName = "Scriptable Objects/New Skill Asset")]
public class SkillAsset : ScriptableObject
{
    [SerializeField] Sprite art;
    [SerializeField] string skillName;
    [SerializeField] string details;
    [SerializeField] int goldCost;
    [SerializeField] int manaCost;
    [SerializeField] int level;
    public Sprite Art { get => art; set => art = value; }
    public string ItemName { get => skillName; set => skillName = value; }
    public string Details { get => details; set => details = value; }
    public int ManaCost { get => manaCost; set => this.manaCost = value; }
    public int GoldCost { get => goldCost; set => goldCost = value; }
    public int Level { get => level; set => level = value; }
}
