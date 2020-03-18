using Lidgren.Network.ServerFiles;
using MMOLoginServer.ServerData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.LoginScreen
{
    public class SelectionController : MonoBehaviour
    {
        public Transform ServerForm;
        public Transform CharacterForm;

        private List<Button> characterItems = new List<Button>();
        private List<Button> serverItems = new List<Button>();
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
                        item.GetComponent<Image>().color = Color.cyan;
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
                        item.GetComponent<Image>().color = Color.cyan;
                        selectedServer = serverItems.IndexOf(button);
                    }
                    else
                    {
                        item.GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }
        public void DrawCharacterItems(List<CharacterData> myCharacters)
        {
            ClearSelection(characterItems);
            Button defaultButton = Resources.Load<Button>("CharacterItemDefault");
            Debug.Log("DRAW CHARACTER COUNT: " + myCharacters.Count);
            for (int i = 0; i < myCharacters.Count; i++)
            {
                Button newButton = Instantiate(defaultButton);
                newButton.GetComponent<CharacterSelectionItem>().Load(myCharacters[i]);

                newButton.transform.SetParent(CharacterForm);
                RectTransform rt = newButton.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 10, 300);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + i * 110, 100);

                characterItems.Add(newButton);
            }
        }
        public void DrawServerItems(List<GameServerData> gameServers)
        {
            ClearSelection(serverItems);
            Button defaultButton = Resources.Load<Button>("ServerItemDefault");
            for (int i = 0; i < gameServers.Count; i++)
            {
                Button newButton = Instantiate(defaultButton);
                newButton.GetComponent<ServerSelectionItem>().Load(gameServers[i], i);

                newButton.transform.SetParent(ServerForm);
                RectTransform rt = newButton.GetComponent<RectTransform>();
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 10, 300);
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10 + i * 110, 100);
                serverItems.Add(newButton);
            }
        }

        public void ClearSelection<T>(List<T> items)
        {
            Debug.Log("CLEARING");
            foreach (T button in items)
            {
                Debug.Log("CLEARING ITEM");
                Destroy((button as Button).gameObject);
            }
            Debug.Log("ITEMS CLEAR");
            items.Clear();
            Debug.Log("ITEMS CLEAR");
            Debug.Log("ITEMS COUNT: " + items.Count);

            SelectedCharacter = -1;
            SelectedServerID = -1;
        }
    }
}
