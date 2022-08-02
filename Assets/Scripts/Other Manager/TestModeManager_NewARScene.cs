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
    GameObject m_RaycastManager;

    bool m_IsTestMode;

    void OnEnable()
    {
        m_IsTestMode = GlobalConfig.TEST_MODE;

        m_TestModeUI.SetActive(m_IsTestMode);
        ActiveRaycast(m_IsTestMode);
    }

    void ActiveRaycast(bool state)
    {
        m_RaycastManager
            .GetComponent<RaycastManager_NewARScene>()
            .SetTestMode(state);
    }
}
