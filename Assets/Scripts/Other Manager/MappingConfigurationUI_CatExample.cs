using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MappingConfigurationUI_CatExample : MonoBehaviour
{
    ///////////////////////////////////////////
    /// Mapping status panel on the left up ///
    ///////////////////////////////////////////

    [SerializeField]
    GameObject m_MappingStatusPanel;

    [SerializeField]
    bool showHideMappingValue = true;

    private void Start()
    {
        //showHideMappingValue = true;

        //ShowHideMappingStatus();
        //ToggleRelocateARCamera();
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


    /////////////////////////////////
    /// Mapping status text input ///
    /////////////////////////////////

    [SerializeField]
    Text m_MappingStatusText;

    public string MappingStatusText
    {
        get { return m_MappingStatusText.text; }
        set { m_MappingStatusText.text = value; }
    }


    //////////////////////////////////////////
    /// Display or not display point cloud ///
    //////////////////////////////////////////

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


    ////////////////////////////////////
    /// Display or not display plane ///
    ////////////////////////////////////

    [SerializeField]
    Slider m_DisplayPlane;

    public bool GetDisplayPlaneSlider()
    {
        if ((int)m_DisplayPlane.value == 1) return true;
        else return false;
    }


    //////////////////////////////////////////////////////
    /// 3D object of camera trail (?) (sorry I forgot) ///
    //////////////////////////////////////////////////////

    [SerializeField]
    Button m_Visualize3DObject;

    [SerializeField]
    GameObject m_ImageRecognitionManager;

    public void Visualize3DObject()
    {
        m_Visualize3DObject.gameObject.SetActive(false);
        if (m_ImageRecognitionManager) m_ImageRecognitionManager.SetActive(true);
    }


    ///////////////////////////////////////////////////////////
    /// Toggle for AR camera relocalization when see marker ///
    ///////////////////////////////////////////////////////////

    [SerializeField]
    Toggle m_ToggleRelocateARCamera;

    public bool GetToggleRelocateARCamera()
    {
        return m_ToggleRelocateARCamera.isOn;
    }

    public void ToggleRelocateARCamera()
    {
        if (!GetToggleRelocateARCamera())
        {
            m_ToggleRelocateARCamera.isOn = true;
            GlobalConfig.UseCorrectionMethod = true;
        }
        else
        {
            m_ToggleRelocateARCamera.isOn = false;
            GlobalConfig.UseCorrectionMethod = false;
        }
    }
}
