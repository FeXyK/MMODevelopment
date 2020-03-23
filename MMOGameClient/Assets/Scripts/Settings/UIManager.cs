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

    public GameObject skillTreeForm;
    public bool isOpen = false;

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
        HealthBar.value = player.Health;
        ManaBar.value = player.mana;

        if (Input.GetKeyDown(KeyCode.K))
        {
            skillTreeForm.SetActive(!skillTreeForm.activeSelf);
            isOpen = skillTreeForm.activeSelf;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = false;
            skillTreeForm.SetActive(false);
        }
    }
}
