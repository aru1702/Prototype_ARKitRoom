using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Test_CorrectionDataSave : MonoBehaviour
{
    public static void SaveDataIntoCSV(List<GameObject> objs)
    {
        List<string[]> dataS = new();
        int i = 1;

        foreach (var item in objs)
        {
            var pos = GlobalConfig.GetPositionFromM44(
                GlobalConfig.GetM44ByGameObjRef
                (item, GlobalConfig.PlaySpaceOriginGO));

            string[] data = new[]
            {
                i.ToString(),
                pos.x.ToString(),
                pos.y.ToString(),
                pos.z.ToString()
            };

            dataS.Add(data);
            i++;
        }

        if (dataS.Count <= 0) return;

        var title = "Test_CorrectionDataSave";

        string time = GlobalConfig.GetNowDateandTime();
        string map = GlobalConfig.SAVE_INTO_MAP.ToString();
        string fileName = time + "__with_" + title + "__ObjectCorrection__Maps_" + map + ".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        ExportCSV.exportData(path, dataS);

        Debug.Log("Saved into: " + path);
    }
}
