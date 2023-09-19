using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridlineUI_NewARScene : MonoBehaviour
{
    [SerializeField]
    GameObject m_OuterGridline, m_InnerGridline, m_CenterPointCross;

    bool showGridline = false;

    public void ShowHideGridlines()
    {
        if(m_OuterGridline && /*m_InnerGridline &&*/ m_CenterPointCross)
        {
            if (showGridline)
            {
                m_OuterGridline.SetActive(false);
                //m_InnerGridline.SetActive(false);
                m_CenterPointCross.SetActive(false);
                showGridline = false;
            }
            else
            {
                m_OuterGridline.SetActive(true);
                //m_InnerGridline.SetActive(true);
                m_CenterPointCross.SetActive(true);
                showGridline = true;
            }
        }
    }

    private void OnEnable()
    {
        // make them true then call function to hide them again
        showGridline = true;
        ShowHideGridlines();
    }
}
