using Assets.Scripts.Character;
using Assets.Scripts.Handlers;
using Assets.Scripts.LoginScreen;
using Lidgren.Network;
using Lidgren.Network.Message;
using Lidgren.Network.Override;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.GameNetworkScripts
{
    public class ClientGameManager : NetPeerOverride
    {
        public new GameMessageHandler messageHandler;
        private UIManager menu;

        private NetIncomingMessage msgIn;
        private MessageType msgType;
        public override void Initialize(string PEER_NAME, int PEER_PORT = 0, bool IS_SERVER = false, bool simulateLatency = true)
        {
            Debug.Log("INITIALIZING");
            base.Initialize(PEER_NAME, PEER_PORT, IS_SERVER, simulateLatency);

            SelectionController selection = GameObject.FindObjectOfType<SelectionController>();
            //SpawnPlayer();
            messageHandler = new GameMessageHandler(netPeer as NetClient);
            menu = GameObject.FindObjectOfType<UIManager>();

            int id = LoginDataHandler.GetInstance().selectedCharacter.id;
            string ip = selection.loginDataController.serverIP;
            int port = selection.loginDataController.serverPort;
            byte[] authKey = selection.loginDataController.authToken;
            string publicKey = selection.loginDataController.publicKey;

            GameMessageSender.Instance.ConnectToGameServer(id, authKey, publicKey, ip, port);
            Debug.Log("CONNECTED TO AREA SERVER");
        }
        public void SpawnPlayer()
        {
            SelectionController selection = GameObject.FindObjectOfType<SelectionController>();

            Vector3 position = selection.loginDataController.selectedCharacter.position;
            GameObject player = null;
            switch (selection.loginDataController.selectedCharacter.characterType)
            {
                case Character.CharacterApperance.Male:
                    player = GameObject.Instantiate(Resources.Load<GameObject>("PlayerPrefabs/PlayerMale"), position, Quaternion.identity);
                    break;
                case Character.CharacterApperance.Female:
                    player = GameObject.Instantiate(Resources.Load<GameObject>("PlayerPrefabs/PlayerFemale"), position, Quaternion.identity);
                    break;
                case Character.CharacterApperance.NotDecided:
                    player = GameObject.Instantiate(Resources.Load<GameObject>("PlayerPrefabs/PlayerNotDecided"), position, Quaternion.identity);
                    break;
            }
            messageHandler.dataHandler.myCharacter = player.GetComponent<EntityContainer>();
            player.GetComponent<EntityContainer>().Set(selection.loginDataController.selectedCharacter);
            float health = player.GetComponent<EntityContainer>().Health;
            float maxHealth = player.GetComponent<EntityContainer>().MaxHealth;
            float mana = player.GetComponent<EntityContainer>().Mana;
            float maxMana= player.GetComponent<EntityContainer>().MaxMana;



            GameObject.FindGameObjectWithTag("Player_HealthBar").GetComponentInChildren<TMP_Text>().text = health + " / " + maxHealth + "   [" + (int)((health / maxHealth) * 100) + "%]";
            GameObject.FindGameObjectWithTag("Player_ManaBar").GetComponentInChildren<TMP_Text>().text = mana + " / " + maxMana + "   [" + (int)((mana / maxMana) * 100) + "%]";
            //GameObject.FindGameObjectWithTag("Player_ManaBar");
        }
        public override void ReceiveMessages()
        {
            ShowPing();
            while ((msgIn = netPeer.ReadMessage()) != null)
            {
                if (msgIn.MessageType == NetIncomingMessageType.Data)
                {
                    msgType = (MessageType)msgIn.ReadByte();
                    switch (msgType)
                    {
                        case MessageType.Notification:
                            messageHandler.Notification(msgIn);
                            break;
                        case MessageType.NewCharacter:
                            Debug.Log(msgType);
                            messageHandler.EntitySpawn(msgIn);
                            break;
                        case MessageType.CharacterMovement:
                            messageHandler.EntityPositionUpdate(msgIn);
                            break;
                        case MessageType.OtherCharacterRemove:
                            messageHandler.EntityDespawn(msgIn);
                            break;
                        case MessageType.AdminChatMessage:
                            messageHandler.AdminCommand(msgIn);
                            break;
                        case MessageType.PrivateChatMessage:
                            messageHandler.HandleChatMessage(msgIn, "PM");
                            break;
                        case MessageType.PublicChatMessage:
                            messageHandler.HandleChatMessage(msgIn);
                            break;
                        case MessageType.NewMobAreaData:
                            messageHandler.MobSpawn(msgIn);
                            break;
                        case MessageType.MobInformation:
                            messageHandler.MobPositionUpdate(msgIn);
                            break;
                        case MessageType.SkillCasted:
                            messageHandler.SkillCasted(msgIn);
                            break;
                        case MessageType.EntityUpdate:
                            messageHandler.EntityHealthUpdate(msgIn);
                            break;
                        case MessageType.SkillListInformation:
                            messageHandler.SkillListInformation(msgIn);
                            break;
                    }
                }
                if (msgIn.MessageType == NetIncomingMessageType.StatusChanged)
                {
                    NetConnectionStatus msgStat = (NetConnectionStatus)msgIn.ReadByte();
                    switch (msgStat)
                    {
                        case NetConnectionStatus.Connected:
                            GameMessageSender.Instance.SendClientReady();
                            Debug.Log("Connected to GameServer");
                            break;
                        case NetConnectionStatus.Disconnected:
                            Debug.Log("Disconnected from GameServer");
                            break;
                    }
                }
            }
        }
        public override void Update()
        {
            ReceiveMessages();
            GameMessageSender.Instance.SendPositionUpdate();
        }
        private void ShowPing()
        {
            if ((netPeer as NetClient).ServerConnection != null)
                menu.Ping(Mathf.RoundToInt((netPeer as NetClient).ServerConnection.AverageRoundtripTime * 1000) + " ms");
            else menu.Ping("Disconnected");
        }
    }
}
