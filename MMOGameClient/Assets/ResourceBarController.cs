using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBarController : MonoBehaviour
{
    public TMP_Text ValueText;
    public Slider Slider;
    private void Awake()
    {
        Slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    private void ValueChangeCheck()
    {
        ValueText.text = Slider.value + " / " + Slider.maxValue + "   [" + (int)((Slider.value / Slider.maxValue) * 100) + "%]";
    }
}
