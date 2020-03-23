using Assets.Scripts.SkillSystem;
using Lidgren.Network;
using Lidgren.Network.ServerFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Handlers
{
    public class GameMessageSender
    {
        private static GameMessageSender instance = null;
        private NetClient netClient;

        public GameMessageSender(NetClient netClient)
        {
            if (instance == null)
            {
                this.netClient = netClient;
                instance = this;
            }
        }
        public static GameMessageSender Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new Exception("GameMessageSender is not initialized yet");
                }
                return instance;
            }
        }
        internal void SendSkillCast(SkillItem skill, Entity target)
        {
            if (target != null)
            {
                NetOutgoingMessage msgOut = netClient.CreateMessage();
                msgOut.Write((byte)MessageType.StartSkillCast);
                msgOut.Write(target.id,16);
                msgOut.Write(skill.SkillID, 16);

                netClient.SendMessage(msgOut, NetDeliveryMethod.Unreliable);
            }
        }
    }
}
