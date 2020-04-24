using System;
using Lidgren.Network;
using Lidgren.Network.Message;
using Assets.AreaServer.Entity;
using MMOGameServer;
using UnityEngine;
using Assets.AreaServer.InventorySystem;

namespace Assets.Scripts.Handlers
{
    public class AreaMessageSender
    {
        private static AreaMessageSender instance = null;
        private NetServer netServer;
        private AreaDataHandler dataHandler;

        public AreaMessageSender(NetServer netServer, AreaDataHandler dataHandler)
        {
            if (instance == null)
            {
                this.netServer = netServer;
                this.dataHandler = dataHandler;
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
            msgOut.Write(target.EntityMana, 16);

            msgOut.Write(target.Inventory.Count, 16);

            if (target.EntityHealth <= 0)
            {
                foreach (var item in target.Inventory)
                {
                    msgOut.Write(item.Value.ID, 32);
                    msgOut.Write(item.Value.Amount, 16);
                    msgOut.Write(item.Value.Level, 16);
                }
            }

            netServer.SendToAll(msgOut, NetDeliveryMethod.Unreliable);
        }

        internal void AddedItem(int entityID, SlotItem item)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.AddedItem);
            msgOut.Write(item.ID, 32);
            msgOut.Write(item.Level, 16);
            msgOut.Write(item.Amount, 16);

            dataHandler.GetEntityConnection(entityID).SendMessage(msgOut, NetDeliveryMethod.Unreliable, 0);
        }
        internal void AddedItem(NetConnection connection, SlotItem item)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.AddedItem);
            msgOut.Write(item.ID, 32);
            msgOut.Write(item.Level, 16);
            msgOut.Write(item.Amount, 16);

            connection.SendMessage(msgOut, NetDeliveryMethod.Unreliable, 0);
        }
        internal void DropLootTo(Entity source, Entity corpse)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.DroppedLootItem);
            msgOut.Write(source.EntityID, 32);// this entity can take this
            msgOut.Write(corpse.EntityID, 32);// what dropped it

            msgOut.Write(corpse.Inventory.Count, 16);
            foreach (var item in corpse.Inventory)
            {
                DropItem drop = new DropItem(corpse.transform.position, item.Value);
                msgOut.Write(dataHandler.droppedItemsCount, 32);
                msgOut.Write(item.Value.ID, 32);
                msgOut.Write(item.Value.Level, 16);
                msgOut.Write(item.Value.Amount, 16);
                dataHandler.droppedItems.Add(dataHandler.droppedItemsCount, drop);
                dataHandler.droppedItemsCount++;
            }
            netServer.SendToAll(msgOut, NetDeliveryMethod.ReliableUnordered, 0);
        }

        internal void RemovedItem(int entityID, int itemID, int amount)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.RemovedItem);
            msgOut.Write(itemID, 32);
            msgOut.Write(amount, 16);
            dataHandler.GetEntity(entityID).Connection.SendMessage(msgOut, NetDeliveryMethod.ReliableUnordered, 0);
        }

        internal void RemovedItem(NetConnection connection, int itemID, int amount)
        {
            NetOutgoingMessage msgOut = netServer.CreateMessage();
            msgOut.Write((byte)MessageType.RemovedItem);
            msgOut.Write(itemID, 32);
            msgOut.Write(amount, 16);
            connection.SendMessage(msgOut, NetDeliveryMethod.ReliableUnordered, 0);
        }

    }
}
