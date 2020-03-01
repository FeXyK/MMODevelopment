using Lidgren.Network.ServerFiles;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.LoginScreen
{
    class ServerSelectionItem : MonoBehaviour
    {
        private SelectionController selectionController;

        private int id;

        public int ServerID
        {
            get { return id; }
            set { id = value; serverIDText.text = "ID: " + id; }
        }
        private string serverName;

        public string Name
        {
            get { return serverName; }
            set { serverName = value; nameText.text = serverName; }
        }
        private string ipAddress;

        public string IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; ipText.text = "IP: " + IPAddress; }
        }
        private int port;

        public int Port
        {
            get { return port; }
            set { port = value; portText.text = "Port: " + port; }
        }
        public Text nameText;
        public Text ipText;
        public Text portText;
        public Text serverIDText;

        public void Load(GameServerData serverData, int sId)
        {
            Name = serverData.name;
            IPAddress = serverData.ip;
            Port = serverData.port;
            ServerID = sId;
        }
        private void Start()
        {
            selectionController = FindObjectOfType<SelectionController>();
        }
        public void Selected()
        {
            selectionController.SelectedServerID = ServerID;
        }

    }
}
