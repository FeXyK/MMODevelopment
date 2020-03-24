using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatController : MonoBehaviour
{
    public TMP_InputField ChatWindow;
    public TMP_Dropdown ChatInputType;
    public TMP_InputField ChatInput;

    [SerializeField]
    private Color activeWindowColor;
    [SerializeField]
    private Color inactiveWindowColor;

    [SerializeField]
    private Color activeInputWindowColor;
    [SerializeField]
    private Color inactiveInputWindowColor;

    [SerializeField]
    private Color activeTextWindowColor;
    [SerializeField]
    private Color inactiveTextWindowColor;

    [SerializeField]
    private Color activeTextInputColor;
    [SerializeField]
    private Color inactiveTextInputColor;

    public bool Input;
    public Color ActiveWindowColor { get => activeWindowColor; set => activeWindowColor = value; }
    public Color InactiveWindowColor { get => inactiveWindowColor; set => inactiveWindowColor = value; }
    public Color ActiveInputWindowColor { get => activeInputWindowColor; set => activeInputWindowColor = value; }
    public Color InactiveInputWindowColor { get => inactiveInputWindowColor; set => inactiveInputWindowColor = value; }
    public Color ActiveTextWindowColor { get => activeTextWindowColor; set => activeTextWindowColor = value; }
    public Color InactiveTextWindowColor { get => inactiveTextWindowColor; set => inactiveTextWindowColor = value; }
    public Color ActiveTextInputColor { get => activeTextInputColor; set => activeTextInputColor = value; }
    public Color InactiveTextInputColor { get => inactiveTextInputColor; set => inactiveTextInputColor = value; }

    // Update is called once per frame
    private void Start()
    {
        ChatWindow.image.color = inactiveWindowColor;
        ChatInput.image.color = inactiveInputWindowColor;
        ChatInput.textComponent.color = activeTextInputColor;
    }
    void Update()
    {
        
    }
    public string GetMessage()
    {
        return ChatInput.text;
    }
    internal void InputSwitch()
    {
        Input = !Input;
        if (Input)
        {
            ChatWindow.image.color = activeWindowColor;
            ChatInput.image.color = activeInputWindowColor;
            this.transform.SetAsLastSibling();
            ChatInput.interactable = true;
            ChatInput.ActivateInputField();//.Select();
        }
        else
        {
            ChatWindow.image.color = inactiveWindowColor;
            ChatInput.image.color = inactiveInputWindowColor;

            ChatInput.text = "";
            ChatInput.interactable = false;
            this.transform.SetAsFirstSibling();
        }
    }
}
