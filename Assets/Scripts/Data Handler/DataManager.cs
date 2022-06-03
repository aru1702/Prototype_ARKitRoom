using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class should be able to:
 * - get data from IoT db (with given name, ip, etc.)
 * - handle the fetched data
 * - return the fetched data as float number
 * - add: return the fetched data with range of all data (hi, lo)
 */

public class DataManager : MonoBehaviour
{
    private IPData _ipData;
    private float _currentValue;
    private float _hiValue, _loValue;

    public float Gvalue, hi, lo;

    public bool testingOnly { set; get; } = false;

    public float GetDataUpdate()
    {
        if (_ipData == null) { throw new System.Exception("No access information!"); }

        float value = 0;

        // by given ip address, access db
        // ...

        _currentValue = value;
        return value;
    }

    private void AssignHiLoValue()
    {
        // this for assigning when real data is exist
        // also normalizing, etc.
    }

    public float GetCurrentValue() { return _currentValue; }
    public float GetHighestValue() { return _hiValue; }
    public float GetLowestValue() { return _loValue; }

    // testing only
    public float Test_GetDataUpdate(float previous_value = 40.0f)
    {
        if (!testingOnly) { throw new System.Exception("Testing only currently disabled!"); }

        float temp_value;
        float temp_lo = previous_value + 5.0f;
        float temp_hi = previous_value - 5.0f;

        if (temp_lo < _loValue) { temp_lo = 40.0f; }
        if (temp_hi > _hiValue) { temp_hi = 100.0f; }
        if (temp_lo > temp_hi) { float tempV = temp_lo; temp_lo = temp_hi; temp_hi = tempV; }

        // generating random data from 40.0f ~ 100.0f
        temp_value = Random.Range(temp_lo, temp_hi);

        Gvalue = temp_value; hi = temp_hi; lo = temp_lo;

        _currentValue = temp_value;
        return temp_value;
    }

    public void Test_AssignHiLoValue()
    {
        if (!testingOnly) { throw new System.Exception("Testing only currently disabled!"); }
        _loValue = 40.0f;
        _hiValue = 100.0f;
    }
}