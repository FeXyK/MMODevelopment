using Assets.Scripts.SkillSystem;
using UnityEngine;

public class SkillTester : MonoBehaviour
{
    void Start()
    {
        
    }
    public Transform target;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Instantiate(SkillList.Instance.skill[1]).GetComponent<Skill_FireballScript>().Set(this.transform, target);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Instantiate(SkillList.Instance.skill[0], target);
        }
    }
}
