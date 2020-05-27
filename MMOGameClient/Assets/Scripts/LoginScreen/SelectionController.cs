using Lidgren.Network.ServerFiles.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.LoginScreen
{
    public class SelectionController : MonoBehaviour
    {
        public Transform ServerForm;
        public Transform CharacterForm;

        public List<CharacterSelectionItem> characterItems = new List<CharacterSelectionItem>();
        private List<ServerSelectionItem> serverItems = new List<ServerSelectionItem>();
        private int selectedCharacter;
        private int selectedServer;
        public LoginDataController loginDataController = new LoginDataController();

        public int SelectedCharacter
        {
            get { return selectedCharacter; }
            set
            {
                foreach (var button in characterItems)
                {
                    CharacterSelectionItem item = button.GetComponent<CharacterSelectionItem>();
                    if (item.CharacterID == value)
                    {
                        item.GetComponent<Image>().color = new Color(0.7f, 0.8f, 0.9f);
                        selectedCharacter = characterItems.IndexOf(button);
                    }
                    else
                    {
                        item.GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }
        public int SelectedServerID
        {
            get { return selectedServer; }
            set
            {
                foreach (var button in serverItems)
                {
                    ServerSelectionItem item = button.GetComponent<ServerSelectionItem>();
                    if (item.ServerID == value)
                    {
                        item.GetComponent<Image>().color = new Color(0.7f, 0.8f, 0.9f);
                        selectedServer = serverItems.IndexOf(button);
                    }
                    else
                    {
                        item.GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }
        public void DrawCharacterItems(List<Entity> myCharacters)
        {
            ClearSelection();
            Button defaultButton = Resources.Load<Button>("CharacterItemDefault");
            Debug.Log("DRAW CHARACTER COUNT: " + myCharacters.Count);
            CharacterForm.GetComponent<RectTransform>().sizeDelta = new Vector2(CharacterForm.GetComponent<RectTransform>().sizeDelta.x, (1 + myCharacters.Count) * defaultButton.GetComponent<RectTransform>().sizeDelta.y);
            for (int i = 0; i < myCharacters.Count; i++)
            {
                CharacterSelectionItem newButton = Instantiate(defaultButton).GetComponent<CharacterSelectionItem>();
                newButton.Load(myCharacters[i]);

                newButton.transform.SetParent(CharacterForm);
                RectTransform rt = newButton.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 300);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + i * 110, 100);

                characterItems.Add(newButton);
            }
            if (characterItems.Count > 0)
                characterItems[0].Selected();
        }
        public void DrawServerItems(List<GameServerData> gameServers)
        {
            //ClearSelection(serverItems);
            ServerSelectionItem defaultButton = Resources.Load<Button>("ServerItemDefault").GetComponent<ServerSelectionItem>();
            for (int i = 0; i < gameServers.Count; i++)
            {
                ServerSelectionItem newButton = Instantiate(defaultButton);
                newButton.Load(gameServers[i], i);

                newButton.transform.SetParent(ServerForm);
                RectTransform rt = newButton.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 10, 300);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + i * 110, 100);
                serverItems.Add(newButton);
            }
            if (serverItems.Count > 0)
                serverItems[0].Selected();
        }

        public void ClearSelection()
        {
            for (int i = characterItems.Count - 1; i >= 0; i--)
            {
                Destroy(characterItems[i].gameObject);
            }
            characterItems.Clear();
            SelectedCharacter = -1;
        }
    }
}
