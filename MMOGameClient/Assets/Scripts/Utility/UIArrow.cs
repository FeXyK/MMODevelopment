using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArrow : MonoBehaviour
{
    Image arrowImage;
    Image arrowCorner;
    Image obj;
    List<Image> images = new List<Image>();
    void Start()
    {
        arrowImage = Resources.Load<Image>("ArrowImage");
        arrowCorner = Resources.Load<Image>("ArrowCorner");
    }
    public void Draw(Vector2 start, Vector2 end)
    {
        float sizex = start.x - end.x;
        float sizey = start.y - end.y;
        float posy = (end.y - start.y) / 2;
        if (arrowImage == null)
        {
            arrowImage = Resources.Load<Image>("ArrowImage");
            arrowCorner = Resources.Load<Image>("ArrowCorner");
        }
        obj = Instantiate(arrowImage);
        obj.transform.SetParent(this.transform);
        obj.rectTransform.sizeDelta = new Vector2(Mathf.Abs(sizex), 30);
        obj.rectTransform.anchoredPosition = new Vector2(start.x + 50, end.y);
        obj.transform.SetAsFirstSibling();
        images.Add(obj);
        if (sizey != 0)
        {
            obj = Instantiate(arrowImage);
            images.Add(obj);
            obj.transform.SetParent(this.transform);
            obj.rectTransform.Rotate(new Vector3(0, 0, 90));
            obj.rectTransform.sizeDelta = new Vector2(Mathf.Abs(sizey), 30);
            obj.rectTransform.anchoredPosition = new Vector2(start.x, start.y + posy);
            obj.transform.SetAsFirstSibling();
            obj = Instantiate(arrowCorner);

            obj.transform.SetParent(this.transform);
            obj.rectTransform.anchoredPosition = new Vector2(start.x, end.y);
            images.Add(obj);
        }
    }
}
