using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderAndInputField : MonoBehaviour
{
    [SerializeField]
    Slider m_Slider;

    [SerializeField]
    InputField m_InputField;

    bool disableSlider = false;
    bool disableInputField = false;

    public void OnSliderChangeInt()
    {
        if (disableSlider) return;

        disableInputField = true;
        var value = (int) (m_Slider.value * 100);
        m_InputField.text = value.ToString();
        //Debug.Log(disableInputField);
    }

    public void OnInputFieldChangeInt()
    {
        if (disableInputField) return;

        disableSlider = true;
        var value = float.Parse(m_InputField.text) / 100;
        m_Slider.value = value;
    }

    public void OnInputFieldEndEdit()
    {
        disableSlider = false;
    }

    public void OnSliderEndEdit()
    {
        disableInputField = false;
        //Debug.Log(disableInputField);
    }
}
