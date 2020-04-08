using Assets.Scripts.SkillSystem.SkillSys;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillListDraw : MonoBehaviour
{
    GameObject reference;
    void Start()
    {
        reference = Resources.Load("SkillItemContainer") as GameObject;
        for (int i = 0; i < 10; i++)
        {
            Skill skill = new Skill();
            skill.ID = i;
            test_skills.Add(skill);

            GameObject obj = Instantiate(reference);
            RectTransform rect = obj.GetComponent<RectTransform>();
            obj.transform.SetParent(this.transform);
            rect.anchoredPosition = new Vector2(0, i * 100);
        }
    }
    List<Skill> test_skills = new List<Skill>();

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {

        }
    }
    public void Draw(List<Skill> skills)
    {

    }
}
