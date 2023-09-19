using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class should able to:
 * - get range data
 * - determine color (from ColorPick)
 * - update object material color
 * 
 * Also this class should be assigned to each object.
 */

public class NonIoTDeviceManager : MonoBehaviour
{
    [SerializeField]
    Material m_ZBufferMaterial;

    string _materialPath = "Materials/ZDepthBuffer_Transparent";
    bool _materialChange = false;

    public void AssignMaterial()
    {
        // change cube mats --> transparent
        if (!_materialChange)
        {
            if (m_ZBufferMaterial)
            {
                GetComponent<Renderer>().material = m_ZBufferMaterial;
            }
            else
            {
                Material zBlock_mats = Resources.Load<Material>(_materialPath);
                GetComponent<Renderer>().material = zBlock_mats;
            }
            _materialChange = true;
        }
    }

    public static bool CheckIfHasRenderer(GameObject gameObject)
    {
        if (gameObject.GetComponent<Renderer>() == null) return false;
        if (gameObject.GetComponent<MeshRenderer>() == null) return false;

        return true;
    }
}
