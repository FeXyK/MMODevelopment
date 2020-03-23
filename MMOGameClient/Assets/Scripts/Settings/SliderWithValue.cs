using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SliderWithValue : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField inputField;
    private void Start()
    {
        try
        {
            //slider.value = float.Parse(inputField.text);
        }
        catch (Exception)
        {
        }
    }
    public void SetSliderValue()
    {
        slider.value = float.Parse(inputField.text);
    }
    public void SetInputFieldValue()
    {
        inputField.text = Math.Round(slider.value, 2).ToString();
    }
    public void SetVolume()
    {
        FindObjectOfType<AudioSource>().volume = slider.value;
    }
}
