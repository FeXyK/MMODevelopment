using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAsset", menuName = "Scriptable Objects/New Item Asset")]
public class ItemAsset : ScriptableObject
{
    [SerializeField] Sprite art;
    [SerializeField] string itemName;
    [SerializeField] string details;
    [SerializeField] int value;
    public Sprite Art { get => art; set => art = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public string Details { get => details; set => details = value; }
    public int Value { get => value; set => this.value = value; }

}
