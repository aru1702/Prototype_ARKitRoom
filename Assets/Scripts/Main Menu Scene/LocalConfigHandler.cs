using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handling local configuration with save and load through csv file.
/// </summary>
public class LocalConfigHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Main UI
    /// </summary>

    [SerializeField]
    InputField m_SaveMapInputField;

    [SerializeField]
    InputField m_LoadMapInputField;

    [SerializeField]
    Toggle m_ActiveCorrectionModeToggle;

    [SerializeField]
    Toggle m_TestModeToggle;

    [SerializeField]
    Dropdown m_CorrectionFunctionDropDown;

    public void SetSaveMapInputField(int value) { m_SaveMapInputField.text = value.ToString(); }
    public int GetSaveMapInputField() { return int.Parse(m_SaveMapInputField.text); }

    public void SetLoadMapInputField(int value) { m_LoadMapInputField.text = value.ToString(); }
    public int GetLoadMapInputField() { return int.Parse(m_LoadMapInputField.text); }

    public void SetActiveCorrectionModeToggle(bool value) { m_ActiveCorrectionModeToggle.isOn = value; }
    public bool GetActiveCorrectionModeToggle() { return m_ActiveCorrectionModeToggle.isOn; }

    public void SetTestModeToggle(bool value) { m_TestModeToggle.isOn = value; }
    public bool GetTestModeToggle() { return m_TestModeToggle.isOn; }

    public void SetCorrectionFunctionDropDown(int value) { m_CorrectionFunctionDropDown.value = value; }
    public int GetCorrectionFunctionDropDown() { return m_CorrectionFunctionDropDown.value; }

    /// <summary>
    /// Config UI
    /// </summary>

    [SerializeField]
    InputField m_OTMScalarInputField;

    [SerializeField]
    Slider m_OTMPrioritySlider;

    [SerializeField]
    InputField m_OTMPriorityInputField;

    [SerializeField]
    InputField m_CTTtimeScalarInputField;

    [SerializeField]
    Slider m_CTTtimePrioritySlider;

    [SerializeField]
    InputField m_CTTtimePriorityInputField;

    [SerializeField]
    InputField m_CTMScalarInputField;

    [SerializeField]
    InputField m_UTDScalarInputField;

    [SerializeField]
    InputField m_RAScalarInputField;

    public void SetOTMScalarInputField(float value) { m_OTMScalarInputField.text = value.ToString(); }
    public float GetOTMScalarInputField() { return float.Parse(m_OTMScalarInputField.text); }

    public void SetOTMPrioritySlider(float value) { m_OTMPrioritySlider.value = value; }
    public float GetOTMPrioritySlider() { return m_OTMPrioritySlider.value; }

    public void SetOTMPriorityInputField(int value) { m_OTMPriorityInputField.text = value.ToString(); }
    public int GetOTMPriorityInputField() { return int.Parse(m_OTMPriorityInputField.text); }

    public void SetCTTtimeScalarInputField(float value) { m_CTTtimeScalarInputField.text = value.ToString(); }
    public float GetCTTtimeScalarInputField() { return float.Parse(m_CTTtimeScalarInputField.text); }

    public void SetCTTtimePrioritySlider(float value) { m_CTTtimePrioritySlider.value = value; }
    public float GetCTTtimePrioritySlider() { return m_CTTtimePrioritySlider.value; }

    public void SetCTTtimePriorityInputField(int value) { m_CTTtimePriorityInputField.text = value.ToString(); }
    public int GetCTTtimePriorityInputField() { return int.Parse(m_CTTtimePriorityInputField.text); }

    public void SetCTMScalarInputField(float value) { m_CTMScalarInputField.text = value.ToString(); }
    public float GetCTMScalarInputField() { return float.Parse(m_CTMScalarInputField.text); }

    public void SetUTDScalarInputField(float value) { m_UTDScalarInputField.text = value.ToString(); }
    public float GetUTDScalarInputField() { return float.Parse(m_UTDScalarInputField.text); }

    public void SetRAScalarInputField(float value) { m_RAScalarInputField.text = value.ToString(); }
    public float GetRAScalarInputField() { return float.Parse(m_RAScalarInputField.text); }


    public void ExportToCSV()
    {
        string[] values =
        {
            GetSaveMapInputField().ToString(),
            GetLoadMapInputField().ToString(),
            GetActiveCorrectionModeToggle().ToString(),
            GetTestModeToggle().ToString(),
            GetCorrectionFunctionDropDown().ToString(),

            GetOTMScalarInputField().ToString(),
            GetOTMPrioritySlider().ToString(),
            GetOTMPriorityInputField().ToString(),
            GetCTTtimeScalarInputField().ToString(),
            GetCTTtimePrioritySlider().ToString(),
            GetCTTtimePriorityInputField().ToString(),

            GetCTMScalarInputField().ToString(),
            GetUTDScalarInputField().ToString(),

            GetRAScalarInputField().ToString()
        };

        var path = Path.Combine(Application.persistentDataPath, "config");
        ExportCSV.exportData(path, values);

        ApplyValueToGlobalConfig();
    }

    public void ImportFromCSV()
    {
        var path = Path.Combine(Application.persistentDataPath, "config");
        var values = ImportCSV.getDataPersistentPath(path);

        if (values.Count <= 0) return;

        SetSaveMapInputField(int.Parse(values[0]));
        SetLoadMapInputField(int.Parse(values[1]));
        SetActiveCorrectionModeToggle(bool.Parse(values[2]));
        SetTestModeToggle(bool.Parse(values[3]));
        SetCorrectionFunctionDropDown(int.Parse(values[4]));

        SetOTMScalarInputField(float.Parse(values[5]));
        SetOTMPrioritySlider(float.Parse(values[6]));
        SetOTMPriorityInputField(int.Parse(values[7]));
        SetCTTtimeScalarInputField(float.Parse(values[8]));
        SetCTTtimePrioritySlider(float.Parse(values[9]));
        SetCTTtimePriorityInputField(int.Parse(values[10]));

        SetCTMScalarInputField(float.Parse(values[11]));
        SetUTDScalarInputField(float.Parse(values[12]));

        SetRAScalarInputField(float.Parse(values[13]));

        ApplyValueToGlobalConfig();
    }

    void ApplyValueToGlobalConfig()
    {
        GlobalConfig.OTM_SCALAR = GetOTMScalarInputField();
        GlobalConfig.OTM_PRIORITY = GetOTMPrioritySlider();
        GlobalConfig.CTTtime_SCALAR = GetCTTtimeScalarInputField();
        GlobalConfig.CTTtime_PRIORITY = GetCTTtimePriorityInputField();
        GlobalConfig.CTM_SCALAR = GetCTMScalarInputField();
        GlobalConfig.RA_ANGLE = GetRAScalarInputField();
    }
}
