using Assets.AreaServer.Entity;
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
    public class AreaMessageSender
    {
        private static AreaMessageSender instance = null;
        private NetServer netServer;

        public AreaMessageSender(NetServer netServer)
        {
            if (instance == null)
            {
                this.netServer = netServer;
                instance = this;
            }
        }
        public static AreaMessageSender Instance
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
        //internal void SendSkillCast(SkillItem skill, Entity target)
        //{
        //    if (target != null)
        //    {
        //        NetOutgoingMessage msgOut = netClient.CreateMessage();
        //        msgOut.Write((byte)MessageType.StartSkillCast);
        //        msgOut.Write(target.id);
        //        msgOut.Write(skill.SkillID, 16);

        //        netClient.SendMessage(msgOut, NetDeliveryMethod.Unreliable);
        //        Debug.Log("SKILL SNET");
        //    }
        //}
        public void SendEntityUpdate(Entity target)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.EntityUpdate);
            msgOut.Write(target.EntityID, 16);
            msgOut.Write(target.EntityHealth, 16);

            netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
        }
    }
}
