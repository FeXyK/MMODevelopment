using MMOLoginServer.ServerData;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.LoginScreen
{
    public class CharacterSelectionItem : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private SelectionController selectionController;

        public Text nameText;
        public Text levelText;
        public Text goldText;
        public Text typeText;
        public Text characterIDText;
        public Text accountIDText;

        public Image Border;
        public Image Background;

        private string characterName;
        public string Name
        {
            get { return characterName; }
            set { characterName = value; nameText.text = characterName; }
        }
        private int level;

        public int Level
        {
            get { return level; }
            set { level = value; levelText.text = "Level: " + level; }
        }
        private int gold;

        public int Gold
        {
            get { return gold; }
            set { gold = value; goldText.text = "Gold: " + gold + " g"; }
        }
        private int cType;
        public int CharacterType
        {
            get { return gold; }
            set { gold = value; typeText.text = "Type: " + cType; }
        }
        private int accountID;

        public int AccountID
        {
            get { return accountID; }
            set { accountID = value; accountIDText.text = "AccID: " + accountID; }
        }
        private int characterID;

        public int CharacterID
        {
            get { return characterID; }
            set { characterID = value; characterIDText.text = "ChID: " + characterID; }
        }


        private void Start()
        {
            selectionController = FindObjectOfType<SelectionController>();
        }
        public void Selected()
        {
            selectionController = FindObjectOfType<SelectionController>();
            selectionController.SelectedCharacter = characterID;
        }

        public void Load(CharacterData characterData)
        {
            Name = characterData.name;
            CharacterID = characterData.id;
            AccountID = characterData.accountID;
            CharacterType = characterData.characterType;
            Gold = characterData.gold;
            Level = characterData.level;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            Background.color = new Color(0.5f, 0.5f, 0.5f);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Background.color = Color.white;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            Border.gameObject.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            Border.gameObject.SetActive(false);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Selected();
        }

    }
}
