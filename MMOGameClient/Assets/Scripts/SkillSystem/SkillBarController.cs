using Assets.Scripts.SkillSystem;
using System.Collections.Generic;
using UnityEngine;

public class SkillBarController : MonoBehaviour
{
    private const float positionConstant = 0.5f;
    Dictionary<string, SkillItem> skills = new Dictionary<string, SkillItem>();
    GameObject skillBarItem;
    public int skillBarLength = 12;
    public float skillBarSize = 47;
    public float skillBarGap = 10;
    public SkillItem GetSkillOnHotkey(KeyCode key)
    {
        string intKey = ((int)key - 48).ToString();
        if (skills.ContainsKey(((int)key - 48).ToString()))
        {
            if (skills[intKey].SkillID != -1)
                return skills[((int)key - 48).ToString()];
        }
        return null;
    }
    private void Awake()
    {
        skillBarItem = Resources.Load("Prefabs/GameUI/SkillBarItem") as GameObject;
    }
    private void Start()
    {
        GenerateSkillBarItems();
    }
    public void Set(SkillItem skill, string hotkey)
    {
        if (skills.ContainsKey(hotkey))
            skills.Remove(hotkey);

        skills.Add(hotkey, skill);
    }
    private void GenerateSkillBarItems()
    {
        float middleX = this.GetComponent<RectTransform>().sizeDelta.x / 2;
        float shiftX = middleX - (skillBarGap + skillBarSize) * skillBarLength / 2;
        float scale = skillBarSize / skillBarItem.GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 0; i < skillBarLength; i++)
        {
            //obj = Instantiate(skillBarItem);
            //obj.transform.SetParent(this.transform);
            //obj.GetComponent<SkillBarItem>().hotkey = i.ToString();
            //rect = obj.GetComponent<RectTransform>();
            //rect.localScale = new Vector2(scale, scale);
            //rect.anchoredPosition = new Vector2(shiftX + (skillBarSize + skillBarGap) * (i + positionConstant), 0);
            Vector2 position = new Vector2(shiftX + (skillBarSize + skillBarGap) * (i + positionConstant), 0);
            SpawnSkillBarItem(position, scale, i);
        }
    }
    public void SpawnSkillBarItem(Vector2 position, float scale, int hotkey)
    {
        GameObject obj;
        RectTransform rect;
        obj = Instantiate(skillBarItem);
        obj.transform.SetParent(this.transform);
        obj.GetComponent<SkillBarItem>().hotkey = hotkey.ToString();
        rect = obj.GetComponent<RectTransform>();
        rect.localScale = new Vector2(scale, scale);
        rect.anchoredPosition = position;
    }
}
