using Assets.Scripts.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Image Art;
    public TMP_Text NameText;
    public TMP_Text DetailsText;
    public TMP_Text CostText;
    public TMP_Text CooldownText;
    public RectTransform Container;

    public string Name;
    public string Details;
    public float Cost;
    public float Cooldown;
    public void Set(Sprite sprite, string name, string details, float cost, float cooldown)
    {
        Art.sprite = sprite;
        Name = name;
        Details = details;
        Cost = cost;
        Cooldown = cooldown;
    }
    //public void Set(SkillItemDrag skillDrag)
    //{
    //    Art.sprite = skillDrag.skill.ArtSprite;
    //    Name = skillDrag.skill.name;
    //    //Details = skill.details
    //    Cost = skillDrag.skill.Cost;
    //    Cooldown = skillDrag.skill.CooldownTimeDefault;
    //    this.transform.position = skillDrag.transform.position;
    //    Refresh();
    //}
    private void Refresh()
    {
        NameText.text = Name;
        //DetailsText.text = Details;
        CostText.text = Cost.ToString();
        CooldownText.text = Cooldown.ToString();
    }
    private void Update()
    {
        if (Container.gameObject.activeSelf)
        {
            if (Input.mousePosition.y + Container.sizeDelta.y+5 > Screen.height)
                this.transform.position = Input.mousePosition + new Vector3(Container.sizeDelta.x / 2 + 5, -(Container.sizeDelta.y / 2 + 5));
            else
                this.transform.position = Input.mousePosition + new Vector3(Container.sizeDelta.x / 2 + 5, Container.sizeDelta.y / 2 + 5);
        }
    }
    public void Show()
    {
        Container.gameObject.SetActive(true);
    }
    public void Hide()
    {
        Container.gameObject.SetActive(false);
    }
}
