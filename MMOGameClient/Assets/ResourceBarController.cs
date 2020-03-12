using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarController : MonoBehaviour
{
    public TMP_Text ValueText;
    public Slider Slider;
    public void Update()
    {
        ValueText.text = Slider.value + " / " + Slider.maxValue + "   [" + (Slider.value / Slider.maxValue )* 100 + "%]";
    }
}
