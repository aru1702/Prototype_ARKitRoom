using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is bounded by the prefab
/// </summary>
public class CrossLikeText : MonoBehaviour
{
    [SerializeField]
    TextMesh m_CrossLikeText;

    public TextMesh crossLikeText
    {
        get { return m_CrossLikeText; }
        set { m_CrossLikeText = value; }
    }

    public void SetCrossLikeText(string text)
    {
        m_CrossLikeText.text = text;
    }

    public string GetCrossLikeText()
    {
        return m_CrossLikeText.text;
    }
}
