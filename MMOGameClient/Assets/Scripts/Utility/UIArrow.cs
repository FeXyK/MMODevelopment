using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArrow : MonoBehaviour
{
    Image arrowImage;
    Image arrowCorner;
    Image obj;
    List<Image> images = new List<Image>();
    public Vector2 a;
    public Vector2 b;
    void Start()
    {
        arrowImage = Resources.Load<Image>("ArrowImage");
        arrowCorner = Resources.Load<Image>("ArrowCorner");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            Draw(a, b);
    }
    public void Draw(Vector2 start, Vector2 end)
    {
        if (arrowImage == null)
        {
            arrowImage = Resources.Load<Image>("ArrowImage");
            arrowCorner = Resources.Load<Image>("ArrowCorner");
        }
        Vector2 size;
        size = start - end;
        Vector2 middlePos = (start + end) / 2;
        float posx = (end.x - start.x) / 2;

        float sizex = start.x - end.x;
        obj = Instantiate(arrowImage);
        obj.transform.SetParent(this.transform);
        obj.rectTransform.sizeDelta = new Vector2(Mathf.Abs(sizex), 30);
        obj.rectTransform.anchoredPosition = new Vector2(start.x + 50, end.y);
        obj.transform.SetAsFirstSibling();
        images.Add(obj);
        if (size.y != 0)
        {
            float posy = (end.y - start.y) / 2;
            float sizey = start.y - end.y;
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
