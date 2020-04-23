using Assets.Scripts.Character;
using Assets.Scripts.Handlers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropItemController : MonoBehaviour
{
    public TMP_Text Name;
    public int entityID;
    public int ID;
    public int Amount;
    public int Level;
    public int TransactionID;
    public float timer = 100;
    public UIManager uiManager;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
            Destroy(this.gameObject);

        Name.transform.parent.rotation = Camera.main.transform.rotation;
    }
    public void Interact()
    {
        if (uiManager.wInvertory.CurrentInventorySize< uiManager.wInvertory.MaxInventorySize)
        {
            GameMessageSender.Instance.SendPickUpMessage(TransactionID);
            Destroy(this.gameObject);
        }
    }
    private void OnMouseDown()
    {
        Interact();
    }
}
