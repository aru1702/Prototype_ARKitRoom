using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SliderEndEdit : MonoBehaviour, IPointerUpHandler
{
    [SerializeField]
    GameObject m_SliderAndInputField;

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("slider end");
        var s = m_SliderAndInputField.GetComponent<SliderAndInputField>();
        s.OnSliderEndEdit();
    }
}
