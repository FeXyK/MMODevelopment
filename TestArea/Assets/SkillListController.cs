using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillListController : MonoBehaviour
{
    public GameObject item;
    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject obj = Instantiate(item);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.transform.SetParent(this.transform);
            rect.anchoredPosition = new Vector2(200, i * 110);
        }
    }
}
