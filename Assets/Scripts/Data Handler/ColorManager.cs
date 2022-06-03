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

public class ColorManager : MonoBehaviour
{
    public float value { set; get; }
    public float HIGH { set; get; }
    public float LOW { set; get; }
    public float _alpha { set; get; } = 0.5f;

    private string _materialPath = "Materials/Transparency_box";
    private bool _materialChange = false;

    public void AssignHighLow(float h, float l) { HIGH = h; LOW = l; }
    public void AssignHighLowAlpha(float h, float l, float a) { HIGH = h; LOW = l; _alpha = a; }

    public void UpdateColor(float value)
    {
        // check if HIGH and LOW already assigned
        if (HIGH == 0 && LOW == 0) { throw new System.Exception("No range data assigned"); }

        // check if HIGH < LOW
        if (HIGH < LOW) { throw new System.Exception("Highest data should not lower than lowest data"); }

        // get data
        Color newColor = ColorPick.GetColor(value, HIGH, LOW, _alpha);

        // change cube mats --> transparent
        if (!_materialChange)
        {
            var transparent_mats = Resources.Load<Material>(_materialPath);
            this.GetComponent<Renderer>().material = transparent_mats;
            _materialChange = true;
        }

        // apply color
        this.GetComponent<Renderer>().material.color = newColor;
    }
}
