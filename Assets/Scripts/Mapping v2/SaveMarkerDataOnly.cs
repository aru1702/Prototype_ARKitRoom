using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMarkerDataOnly : MonoBehaviour
{
    [SerializeField]
    GameObject m_MappingV2;

    [SerializeField]
    GameObject m_UIManager;

    public void SaveMarkerOnlyBtn()
    {
        m_MappingV2.GetComponent<MappingV2>().SaveMarkerOnly();
        m_UIManager.GetComponent<UIManager_CatExample>().MapStatus.text
            = "Marker data successfully saved!\n" +
              "No changes will affect the map data.";
        m_UIManager.GetComponent<UIManager_CatExample>().OpenPanel();
    }
}
