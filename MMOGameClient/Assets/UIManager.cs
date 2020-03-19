using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject Skillbar;
    public GameObject Minimap;
    public Slider HealthBar;
    public Slider ManaBar;
    public GameObject TargetFrame;

    ResourceBarController healthBarController;
    ResourceBarController manaBarController;

    Entity player;

    private void Start()
    {
        healthBarController = HealthBar.GetComponent<ResourceBarController>();
        manaBarController = ManaBar.GetComponent<ResourceBarController>();
        player = FindObjectOfType<Entity>();
    }
    private void Update()
    {
        HealthBar.value = player.health;
        ManaBar.value = player.mana;
    }
}
