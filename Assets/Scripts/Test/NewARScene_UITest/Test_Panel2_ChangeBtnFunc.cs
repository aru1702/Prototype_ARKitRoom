using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Panel2_ChangeBtnFunc : MonoBehaviour
{
    [SerializeField]
    GameObject m_PanelUI_1;

    [SerializeField]
    GameObject m_PanelUI_2;

    [SerializeField]
    Text m_ChangeBtn_text;

    [SerializeField]
    int screen = 0;

    public void ChangeBtn()
    {
        if (screen == 1)
            screen = 2;

        else if (screen == 2)
            screen = 1;

        SwitchPanel(screen);
        Debug.Log(screen);
    }

    void OnEnable()
    {
        // default
        if (screen == 0)
            screen = 1;
        SwitchPanel(screen);
    }

    void SwitchPanel(int screen)
    {
        switch(screen)
        {
            case 1:
                SetActive(m_PanelUI_1, true);
                SetActive(m_PanelUI_2, false);
                SetUIText(m_ChangeBtn_text, "Change to World Calib");
                break;

            case 2:
                SetActive(m_PanelUI_1, false);
                SetActive(m_PanelUI_2, true);
                SetUIText(m_ChangeBtn_text, "Change to Raycast");
                break;
        }
    }

    void SetActive(GameObject go, bool active)
    {
        go.SetActive(active);
    }

    void SetUIText(Text ui_text, string text)
    {
        ui_text.text = text;
    }
}
