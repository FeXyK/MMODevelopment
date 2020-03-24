using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.SkillSystem
{
    public class SkillTreeController : MonoBehaviour
    {
        public List<SkillTreeItem> skills = new List<SkillTreeItem>();
        public int SkillPoints;
        UIArrow arrows;

        private void Start()
        {

            arrows = this.GetComponent<UIArrow>();

            TextAsset skillsText = (TextAsset)Resources.Load("SkillDetails/SkillTreeInfo", typeof(TextAsset));
            string[] skillStrings = skillsText.text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            //string[] skillStrings = File.ReadAllLines("D:/Github/MMODevelopment/SkillTreeInfo.txt");
            int i = 0, j = 0;
            foreach (var skillString in skillStrings)
            {
                string[] skillData = skillString.Split(';');
                SkillTreeItem skillTreeItem = Instantiate(Resources.Load<SkillTreeItem>("SkillTreeItem"));
                skillTreeItem.transform.SetParent(this.transform);
                skillTreeItem.Name = skillData[0];
                skillTreeItem.Art.sprite = Resources.Load<Sprite>("SkillIcons/" + skillData[0]);
                skillTreeItem.SkillID = int.Parse(skillData[1]);
                skillTreeItem.RequiredLevel = int.Parse(skillData[2]);
                skillTreeItem.RequiredSkillPoint = int.Parse(skillData[3]);
                RectTransform rt = skillTreeItem.GetComponent<RectTransform>();
                if (Screen.width - 200 < 30 + i * 50)
                {
                    i = 0;
                    j++;
                }
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 30 + i * 50, 45);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 30 + 50 * j, 45);
                skills.Add(skillTreeItem);

                i++;
            }
            TextAsset skillConnectionsText = (TextAsset)Resources.Load("SkillDetails/SkillConnections", typeof(TextAsset));
            string[] skillConnections = skillConnectionsText.text.Split(new[] { Environment.NewLine },StringSplitOptions.None);
            //          File.ReadAllLines("/SkillConnections.txt");

            foreach (var skillConnection in skillConnections)
            {
                string[] data = skillConnection.Split(';');
                SkillTreeItem item = skills.Find(x => x.SkillID == int.Parse(data[0]));
                string[] skillIDs = data[1].Split('|');
                foreach (var skillID in skillIDs)
                {
                    if (skillID != "")
                    {
                        foreach (var skill in skills)
                        {

                            if (skill.SkillID == int.Parse(skillID))
                            {
                                item.PreconditionOfSkill.Add(skill);
                            }
                        }
                    }
                }
            }
            Recursive(0, this.GetComponent<RectTransform>().rect.height / 2 - 25, 0, 0, skills[0]);
            //Debug.Log(skills[0].GetComponent<RectTransform>().anchoredPosition);
        }
        private void Recursive(int depth, float parentPosition, int childCount, int count, SkillTreeItem skillItem)
        {
            RectTransform rt = skillItem.GetComponent<RectTransform>();

            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 30 + depth * 100, 50);
            float varialDepth = 0;
            if (depth > 0 && (count != 0 || count != 1))
                varialDepth = 1f / depth;

            float diff = parentPosition + (depth > 1 ? 50 : 150) * count + varialDepth * 10 * count;
            float diff2 = 0;
            if (depth > 1)
                diff2 = childCount % 2 == 0 && childCount != 0 ? 27.5f * -count : 0;
            if (childCount == 1)
                diff2 = 0;
            //Debug.Log(skillItem.Name + " VD: " + varialDepth + " d2: " + diff2 + " Count" + count);
            rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, diff + diff2, 50);
            if (skillItem.PreconditionOfSkill.Count == 0)
            {

                return;
            }
            int i = -skillItem.PreconditionOfSkill.Count / 2;
            foreach (var skill in skillItem.PreconditionOfSkill)
            {
                //Debug.Log(i + " " + skill.Name + " " + depth);
                Recursive(depth + 1, diff + diff2, skillItem.PreconditionOfSkill.Count, i, skill);

                arrows.Draw(skillItem.GetComponent<RectTransform>().anchoredPosition, skill.GetComponent<RectTransform>().anchoredPosition);
                i++;
                if (skillItem.PreconditionOfSkill.Count % 2 == 0)
                    if (i == 0)
                        i = 1;
            }
        }
        public void DrawTransition()
        {

        }
    }
}
