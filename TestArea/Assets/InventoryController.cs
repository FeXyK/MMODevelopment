using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject item;
    void Start()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 5; j++)
            {
                GameObject obj = Instantiate(item);
                RectTransform rect = obj.GetComponent<RectTransform>();
                rect.transform.SetParent(this.transform);
                rect.anchoredPosition = new Vector2(j * 110 - 400, i * 110 - 100);
            }
    }
}
