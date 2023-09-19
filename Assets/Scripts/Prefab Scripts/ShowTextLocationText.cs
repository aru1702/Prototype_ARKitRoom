using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is bounded by the prefab
/// </summary>
public class ShowTextLocationText : MonoBehaviour
{
    [SerializeField]
    TextMesh m_Location1Text;

    [SerializeField]
    TextMesh m_Location2Text;

    /// <summary>
    /// Location1 
    /// </summary>

    public TextMesh location1Text
    {
        get { return m_Location1Text; }
        set { m_Location1Text = value; }
    }

    public void SetLocation1Text(string text)
    {
        m_Location1Text.text = text;
    }

    public string GetLocation1Text()
    {
        return m_Location1Text.text;
    }

    /// <summary>
    /// Location 2
    /// </summary>

    public TextMesh location2Text
    {
        get { return m_Location2Text; }
        set { m_Location2Text = value; }
    }

    public void SetLocation2Text(string text)
    {
        m_Location2Text.text = text;
    }

    public string GetLocation2Text()
    {
        return m_Location2Text.text;
    }
}
