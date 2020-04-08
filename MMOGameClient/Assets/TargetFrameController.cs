using Assets.Scripts.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetFrameController : MonoBehaviour
{
    public EntityContainer target;
    public Slider HealthBar;
    public TMP_Text NameBar;
    private void Update()
    {
     if (target != null)
        {
            HealthBar.value = target.Health;
            HealthBar.maxValue = target.MaxHealth;
        }
}
    public void Set(EntityContainer target)
    {
        this.target = target;
        HealthBar.value = target.Health;

        NameBar.text = target.entity.characterName;
    }
}
