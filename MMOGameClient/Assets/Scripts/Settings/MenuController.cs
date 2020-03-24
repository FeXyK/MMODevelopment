using Assets.Scripts.Handlers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public List<GameObject> stack = new List<GameObject>();

    public GameObject MainMenu;

    public GameObject wOptions;
    public GameObject wSkill;
    public GameObject wCharacter;
    public GameObject wGear;
    public GameObject wInvertory;

    public TMP_InputField ChatInput;
    public TMP_InputField ChatWindow;
    public TMP_Text PingText;

    public GameMessageHandler messageHandler;


    private Image chatBg;

    private Movement playerMovement;
    private UIManager gameUI;

    private bool isOpen = false;

    private void OnLevelWasLoaded(int level)
    {

        gameUI = FindObjectOfType<UIManager>();
    }
    private void Start()
    {
        chatBg = ChatWindow.GetComponent<Image>();
    }
    private void LateUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    stack.Add(wSkill);
        //}
        //else if (Input.GetKeyDown(KeyCode.P))
        //{
        //    stack.Add(wGear);
        //}
        //else if (Input.GetKeyDown(KeyCode.I))
        //{
        //    stack.Add(wInvertory);
        //}
        //else if (Input.GetKeyDown(KeyCode.C))
        //{
        //    stack.Add(wCharacter);
        //}
        //else if (Input.GetKeyDown(KeyCode.O))
        //{
        //    stack.Add(wOptions);
        //}
        //else if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (stack.Count > 0)
        //        stack.RemoveAt(stack.Count - 1);
        //    else stack.Add(MainMenu);
        //}
        //else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        //{

        //}

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (!ChatInput.gameObject.activeSelf)
        //    {
        //        MainMenu.SetActive(!MainMenu.activeSelf);
        //        wOptions.SetActive(false);
        //        isOpen = MainMenu.activeSelf;
        //        this.GetComponent<Canvas>().sortingOrder = MainMenu.activeSelf ? 1 : -1;
        //    }
        //    else
        //    {
        //        chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0);
        //        ChatInput.text = "";
        //        ChatInput.DeactivateInputField();
        //        ChatInput.gameObject.SetActive(false);
        //        isOpen = ChatInput.gameObject.activeSelf;
        //    }
        //}
        //if (this.isOpen || (gameUI != null && gameUI.isOpen))
        //{
        //    if (playerMovement != null)
        //        playerMovement.movementEnabled = false;
        //}
        //else
        //{
        //    if (playerMovement == null)
        //        playerMovement = FindObjectOfType<Movement>();
        //    else
        //    {
        //        playerMovement.movementEnabled = true;
        //    }
        //}
        ////if (Input.GetKeyDown(KeyCode.Return) && !MainMenu.activeSelf && !OptionsMenu.activeSelf)
        ////{
        ////    ChatInput.gameObject.SetActive(!ChatInput.gameObject.activeSelf);
        ////    isOpen = ChatInput.gameObject.activeSelf;
        ////    if (ChatInput.gameObject.activeSelf)
        ////    {
        ////        ChatInput.ActivateInputField();
        ////        chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0.5f);
        ////    }
        ////    else
        ////    {
        ////        if (ChatInput.text.Length > 0)
        ////            messageHandler.ChatMessage = ChatInput.text;

        ////        ChatInput.text = "";
        ////        ChatInput.DeactivateInputField();
        ////        chatBg.color = new Color(chatBg.color.r, chatBg.color.g, chatBg.color.b, 0);
        ////    }
        ////}
        //if (Input.GetMouseButton(1))
        //{
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
        //else
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
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
        wOptions.SetActive(true);
        isOpen = wOptions.activeSelf;
    }
    public void OnExitClick()
    {
        Application.Quit();
    }
    public void OnBackClick()
    {
        MainMenu.SetActive(true);
        wOptions.SetActive(false);
        isOpen = MainMenu.activeSelf;
    }

}
