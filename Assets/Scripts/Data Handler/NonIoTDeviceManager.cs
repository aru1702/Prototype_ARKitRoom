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
    private string _materialPath = "Materials/Fade_prefabs_0";
    private bool _materialChange = false;

    public void AssignMaterial()
    {
        // change cube mats --> transparent
        if (!_materialChange)
        {
            Material zBlock_mats = Resources.Load<Material>(_materialPath);
            GetComponent<Renderer>().material = zBlock_mats;
            _materialChange = true;
        }
    }
}
