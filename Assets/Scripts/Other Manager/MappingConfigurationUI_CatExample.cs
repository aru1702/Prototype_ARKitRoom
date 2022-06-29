using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MappingConfigurationUI_CatExample : MonoBehaviour
{
    [SerializeField]
    GameObject m_MappingStatusPanel;

    bool showHideMappingValue;

    private void Start()
    {
        showHideMappingValue = true;
    }

    public void ShowHideMappingStatus()
    {
        if (showHideMappingValue)
        {
            m_MappingStatusPanel.SetActive(false);
            showHideMappingValue = false;
        }
        else
        {
            m_MappingStatusPanel.SetActive(true);
            showHideMappingValue = true;
        }
    }

    [SerializeField]
    Text m_MappingStatusText;

    public string MappingStatusText
    {
        get { return m_MappingStatusText.text; }
        set { m_MappingStatusText.text = value; }
    }

    [SerializeField]
    Slider m_DisplayPointCloud;

    //public int GetDisplayPointCloudSlider()
    //{
    //    return (int) m_DisplayPointCloud.value;
    //}

    public bool GetDisplayPointCloudSlider()
    {
        if ((int)m_DisplayPointCloud.value == 1) return true;
        else return false;
    }

    [SerializeField]
    Slider m_DisplayPlane;

    public bool GetDisplayPlaneSlider()
    {
        if ((int)m_DisplayPlane.value == 1) return true;
        else return false;
    }
}
