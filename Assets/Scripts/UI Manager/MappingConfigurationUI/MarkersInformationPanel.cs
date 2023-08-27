using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkersInformationPanel : MonoBehaviour
{
    [SerializeField]
    GameObject m_MarkerInformationPanel;

    [SerializeField]
    Text m_PanelText;

    [SerializeField]
    GameObject m_ScrollViewContent;

    float const_h;


    private void Start()
    {
        const_h = m_ScrollViewContent.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void SetText(string text)
    {
        m_PanelText.text = text;
    }

    public void SetScrollViewHeight(float height)
    {
        var x = m_ScrollViewContent.GetComponent<RectTransform>().sizeDelta.x;
        var y = m_ScrollViewContent.GetComponent<RectTransform>().sizeDelta.y;
        if (height <= y) return;
        m_ScrollViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(x, height);

        x = m_PanelText.GetComponent<RectTransform>().sizeDelta.x;
        y = m_PanelText.GetComponent<RectTransform>().sizeDelta.y;
        if (height <= y) return;
        m_PanelText.GetComponent<RectTransform>().sizeDelta = new Vector2(x, height);
    }

    public float CalculateTextHeight(string input_text)
    {
        // only works in font size = 14
        // and with the mentioned panel only
        float per_text = const_h / 20f;
        string[] nw = input_text.Split("\n");
        Debug.Log("ch: " + const_h + ", nh: " + per_text * nw.Length);
        return per_text * (float)nw.Length;
    }

    public void FuncShowHidePanel()
    {
        // activeSelf = active by self, follows the parent
        // activeInHierarchy = active by self, not follows the parent

        var status = m_MarkerInformationPanel.activeSelf;
        m_MarkerInformationPanel.SetActive(!status);
    }
}
