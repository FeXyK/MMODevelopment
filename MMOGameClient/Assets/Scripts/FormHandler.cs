using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormHandler : MonoBehaviour
{
    public List<Selectable> selectables = new List<Selectable>();
   
    public void SetInteractable(bool value)
    {
        foreach (var selectable in selectables)
        {
            selectable.interactable = value;
        }
    }
}
