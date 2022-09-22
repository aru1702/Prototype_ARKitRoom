using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Active several UIs and functions if TestMode toggle on MainMenu is activated
///
/// Testing UI list:
/// - Record and save object position for 3D into 2D screen position -> accuracy
/// - Raycast active to enable touch screen and see distance between touched
///   area to nearest object
/// - Time record between after "load object" and map recovery phase (initialization)
///   - This has two function: 1) automated, and 2) by button
/// </summary>
public class TestModeManager_NewARScene : MonoBehaviour
{
    [SerializeField]
    GameObject m_TestModeUI;

    [SerializeField]
    bool m_ActiveUI_1;

    [SerializeField]
    GameObject m_TestModeUI_2;

    [SerializeField]
    bool m_ActiveUI_2;

    [SerializeField]
    GameObject m_RaycastManager;

    bool m_IsTestMode;

    void OnEnable()
    {
        m_IsTestMode = GlobalConfig.TEST_MODE;

        m_TestModeUI.SetActive(m_IsTestMode && m_ActiveUI_1);
        m_TestModeUI_2.SetActive(m_IsTestMode && m_ActiveUI_2);

        //ActiveRaycast(m_IsTestMode);

        ///////////
        //if (GetComponent<Test_ShowLocationAboveObject>() != null)
        //    GetComponent<Test_ShowLocationAboveObject>().enabled = m_IsTestMode;
    }

    void ActiveRaycast(bool state)
    {
        m_RaycastManager
            .GetComponent<RaycastManager_NewARScene>()
            .SetTestMode(state);
    }
}
