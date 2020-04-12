using Assets.Scripts.Character;
using Assets.Scripts.Handlers;
using Assets.Scripts.UI_Window;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public List<GameObject> stack = new List<GameObject>();

    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance != null) return instance;
            return null;
        }
    }
    public FloatingNotification floatingNotification;
    public WindowHotbar wHotbar;
    public GameObject Minimap;
    public Slider HealthBar;
    public Slider ManaBar;
    public GameObject TargetFrame;

    public TMP_Text wPing;

    public GameObject wMain;
    public GameObject wOptions;
    public WindowSkillIventory wSkill;
    public GameObject wCharacter;
    public GameObject wGear;
    public GameObject wInvertory;
    public ChatController wChat;

    WindowTooltip tooltip;


    ResourceBarController healthBarController;
    ResourceBarController manaBarController;

    EntityContainer player;
    Movement movement;
    private void Start()
    {
        tooltip = FindObjectOfType<WindowTooltip>();
        healthBarController = HealthBar.GetComponent<ResourceBarController>();
        manaBarController = ManaBar.GetComponent<ResourceBarController>();
        player = FindObjectOfType<EntityContainer>();
        if (player != null)
            movement = player.GetComponent<Movement>();
        instance = this;
    }
    public void SetCooldown(int skillID)
    {
        wHotbar.SetCooldown(skillID);
    }
    private void Update()
    {
        if (player != null)
        {
            HealthBar.value = player.Health;
            ManaBar.value = player.entity.mana;

            movement.movementEnabled = stack.Count > 0 ? false : true;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StackAdd(wSkill.gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            StackAdd(wGear);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            StackAdd(wCharacter);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            StackAdd(wInvertory);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            StackAdd(wOptions);
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            wChat.InputSwitch();
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (stack.Count > 0)
                StackRemove();
            else
                StackAdd(wMain);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            StackClear();
        }
    }
    public void StackAdd(GameObject window)
    {
        if (tooltip == null)
            tooltip = FindObjectOfType<WindowTooltip>();
        if (tooltip != null)
            tooltip.Hide();
        if (stack.Contains(window))
        {
            stack.Remove(window);
            window.SetActive(false);
            window.transform.SetAsLastSibling();
        }
        else
        {
            stack.Add(window);
            window.SetActive(true);
        }
    }
    public void StackRemove()
    {

        if (tooltip == null)
            tooltip = FindObjectOfType<WindowTooltip>();
        if (tooltip != null)
            tooltip.Hide();
        if (stack.Count > 0)
        {
            stack[stack.Count - 1].SetActive(false);
            stack.RemoveAt(stack.Count - 1);
        }
    }
    public void StackClear()
    {

        if (tooltip == null)
            tooltip = FindObjectOfType<WindowTooltip>();
        if (tooltip != null)
            tooltip.Hide();
        foreach (var window in stack)
        {
            window.SetActive(false);
        }
        stack.Clear();
    }
    public void Exit()
    {
        GameMessageSender.Instance.SendDisconnect();
        Application.Quit();
    }

    internal void Ping(string value)
    {
        wPing.text = value;
    }

    internal void SetFloatingNotification(string msg)
    {
        Instantiate(floatingNotification, this.transform).Message.text = msg;
    }
}
