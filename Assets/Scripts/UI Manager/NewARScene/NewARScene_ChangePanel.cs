using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewARScene_ChangePanel : MonoBehaviour
{
    [SerializeField]
    GameObject m_FirstPanel;

    [SerializeField]
    GameObject m_SecondPanel;


    int chosen_panel = 1;


    // Start is called before the first frame update
    void Start()
    {
        chosen_panel = 1;
        SetActive(m_FirstPanel, true);
        SetActive(m_SecondPanel, false);
    }

    void SetActive(GameObject go, bool t)
    {
        go.SetActive(t);
    }

    public void ButtonChangePanel()
    {
        switch(chosen_panel)
        {
            case 1:
                {
                    chosen_panel = 2;
                    SetActive(m_FirstPanel, false);
                    SetActive(m_SecondPanel, true);
                    break;
                }

            case 2:
                {
                    chosen_panel = 1;
                    SetActive(m_FirstPanel, true);
                    SetActive(m_SecondPanel, false);
                    break;
                }

            default:
                {
                    chosen_panel = 1;
                    SetActive(m_FirstPanel, true);
                    SetActive(m_SecondPanel, false);
                    break;
                }
        }
    }
}
