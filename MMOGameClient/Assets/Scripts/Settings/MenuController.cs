using Assets.Scripts.Handlers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public TMP_InputField ChatInput;
    public TMP_InputField ChatWindow;
    public TMP_Text PingText;
    public GameMessageHandler messageHandler;

    private Image chatBg;

    private Movement movement;
    private InGameManager gameManager;
    private void Start()
    {
        chatBg = ChatWindow.GetComponent<Image>();
        gameManager = FindObjectOfType<InGameManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!ChatInput.gameObject.activeSelf)
            {
                MainMenu.SetActive(!MainMenu.activeSelf);
                OptionsMenu.SetActive(false);
                this.GetComponent<Canvas>().sortingOrder = MainMenu.activeSelf ? 1 : -1;
            }
            else
            {
                chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0);
                ChatInput.text = "";
                ChatInput.DeactivateInputField();
                ChatInput.gameObject.SetActive(false);
            }
        }
        if (MainMenu.activeSelf || OptionsMenu.activeSelf || ChatInput.gameObject.activeSelf)
        {
            if (movement != null)
                movement.movementEnabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            if (movement == null)
                movement = FindObjectOfType<Movement>();
            else
            {
                movement.movementEnabled = true;              
                Cursor.lockState = CursorLockMode.Locked;

                Cursor.visible = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) && !MainMenu.activeSelf && !OptionsMenu.activeSelf)
        {
            ChatInput.gameObject.SetActive(!ChatInput.gameObject.activeSelf);
            if (ChatInput.gameObject.activeSelf)
            {
                ChatInput.ActivateInputField();
                chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0.5f);
            }
            else
            {
                if (ChatInput.text.Length > 0)
                    messageHandler.ChatMessage = ChatInput.text;

                ChatInput.text = "";
                ChatInput.DeactivateInputField();
                chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0);
            }
        }
    }
    public void OnResumeClick()
    {
        MainMenu.SetActive(false);
        this.GetComponent<Canvas>().sortingOrder = MainMenu.activeSelf ? 1 : -1;
    }
    public void OnOptionsClick()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    public void OnExitClick()
    {
        Application.Quit();
    }
    public void OnBackClick()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
    }

}
