using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetFrameController : MonoBehaviour
{
    public Character target;
    public Slider HealthBar;
    public TMP_Text NameBar;
    private void Update()
    {
        if (target != null)
            HealthBar.value = target.health;
    }
    public void Set(Character target)
    {
        this.target = target;
        HealthBar.value = target.health;

        NameBar.text = target.characterName;
    }
}
