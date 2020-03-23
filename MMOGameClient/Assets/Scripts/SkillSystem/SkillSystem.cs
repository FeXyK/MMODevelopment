using Assets.Scripts.Handlers;
using Assets.Scripts.SkillSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    SkillBarController skillController;
    Entity Target;
    TargetFrameController targetFrame;
    SkillItem skill;
    void Start()
    {
        skillController = FindObjectOfType<SkillBarController>();
        targetFrame = FindObjectOfType<TargetFrameController>();
    }
    void Update()
    {
        if (targetFrame.target != null)
        {
            Target = targetFrame.target;
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha0);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha1);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha2);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha3);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha4);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha5);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha6);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha7);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha8);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                skill = skillController.GetSkillOnHotkey(KeyCode.Alpha9);
                if (skill != null)
                    GameMessageSender.Instance.SendSkillCast(skill, Target);
            }
        }

    }
}
