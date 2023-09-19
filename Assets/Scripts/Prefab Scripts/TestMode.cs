using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// If TestMode active, the User Role UI will has several testing UI
/// 
/// Testing UI list:
/// - Record and save object position for 3D into 2D screen position -> accuracy
/// - Raycast active to enable touch screen and see distance between touched
///   area to nearest object
/// - Time record between after "load object" and map recovery phase (initialization)
///   - This has two function: 1) automated, and 2) by button
/// </summary>
public class TestMode : MonoBehaviour
{
    private void OnEnable()
    {
        m_IsTestMode = GlobalConfig.TEST_MODE;
        m_TestMode.isOn = m_IsTestMode;

        m_IsActiveCorrectionMode = GlobalConfig.UseCorrectionMethod;
        m_ActiveCorrectionMode.isOn = m_IsActiveCorrectionMode;
    }

    ////////////////////////////////////////
    ////////////////////////////////////////
    ////////////////////////////////////////

    [SerializeField]
    Toggle m_TestMode;

    bool m_IsTestMode;

    bool GetTestModeToggle()
    {
        if (!m_TestMode) return false;

        return m_TestMode.isOn;
    }

    public void Set__TEST_MODE()
    {
        GlobalConfig.TEST_MODE = GetTestModeToggle();
        m_IsTestMode = GetTestModeToggle();
    }


    ////////////////////////////////////////
    ////////////////////////////////////////
    ////////////////////////////////////////

    [SerializeField]
    Toggle m_ActiveCorrectionMode;

    bool m_IsActiveCorrectionMode;

    bool GetActiveCorrectionModeToggle()
    {
        if (!m_ActiveCorrectionMode) return false;

        return m_ActiveCorrectionMode.isOn;
    }

    public void Set__CORRECTION_ACTIVE()
    {
        GlobalConfig.UseCorrectionMethod = GetActiveCorrectionModeToggle();
        m_IsActiveCorrectionMode = GetActiveCorrectionModeToggle();
    }
}
