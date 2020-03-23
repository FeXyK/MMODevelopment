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
    UIManager gameUI;
    private bool isOpen = false;
    private void OnLevelWasLoaded(int level)
    {

        gameUI = FindObjectOfType<UIManager>();
    }
    private void Start()
    {
        chatBg = ChatWindow.GetComponent<Image>();
        gameManager = FindObjectOfType<InGameManager>();
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!ChatInput.gameObject.activeSelf)
            {
                MainMenu.SetActive(!MainMenu.activeSelf);
                OptionsMenu.SetActive(false);
                isOpen = MainMenu.activeSelf;
                this.GetComponent<Canvas>().sortingOrder = MainMenu.activeSelf ? 1 : -1;
            }
            else
            {
                chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0);
                ChatInput.text = "";
                ChatInput.DeactivateInputField();
                ChatInput.gameObject.SetActive(false);
                isOpen = ChatInput.gameObject.activeSelf;
            }
        }
        if (this.isOpen || (gameUI != null && gameUI.isOpen))
        {
            if (movement != null)
                movement.movementEnabled = false;
        }
        else
        {
            if (movement == null)
                movement = FindObjectOfType<Movement>();
            else
            {
                movement.movementEnabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) && !MainMenu.activeSelf && !OptionsMenu.activeSelf)
        {
            ChatInput.gameObject.SetActive(!ChatInput.gameObject.activeSelf);
            isOpen = ChatInput.gameObject.activeSelf;
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
        if(Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnResumeClick()
    {
        MainMenu.SetActive(false);
        isOpen = MainMenu.activeSelf;
        this.GetComponent<Canvas>().sortingOrder = MainMenu.activeSelf ? 1 : -1;
    }
    public void OnOptionsClick()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        isOpen = OptionsMenu.activeSelf;
    }
    public void OnExitClick()
    {
        Application.Quit();
    }
    public void OnBackClick()
    {
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        isOpen = MainMenu.activeSelf;
    }

}
