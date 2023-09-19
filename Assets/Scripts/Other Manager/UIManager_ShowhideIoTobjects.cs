using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager_ShowhideIoTobjects : MonoBehaviour
{
    [SerializeField]
    [Tooltip("To import IoT gameObject list")]
    GameObject m_LoadObjectManager;

    bool showhide = true;

    public void BtnShowHideIoTObject()
    {
        var objects = m_LoadObjectManager
                .GetComponent<LoadObject_CatExample_2>()
                .GetObjectsWithoutMarkerPrefabs();

        foreach (var item in objects)
        {
            if (showhide) item.SetActive(false);
            else item.SetActive(true);
        }

        if (showhide) showhide = false;
        else showhide = true;
    }
}
