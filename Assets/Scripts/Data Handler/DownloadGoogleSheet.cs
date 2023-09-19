using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DownloadGoogleSheet : MonoBehaviour
{
    public static async Task<List<string[]>> ImportData(string url_id)
    {
        string front_url = "https://docs.google.com/spreadsheets/d/e/";
        string back_url = "/pub?gid=0&single=true&output=csv";
        string full_url = front_url + url_id + back_url;

        var importedData = GetData(full_url);
        await importedData;
        List<string[]> l_str = ImportCSV.GetDataFromRawString(
            importedData.ToString(),
            true);

        return l_str;
    }

    static async Task<string> GetData(string url)
    {
        UnityWebRequest www = new(url)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };

        var getReq = www.SendWebRequest();
        while (getReq.isDone)
        {
            await Task.Yield();
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return "";
        }
        else
        {
            return www.downloadHandler.text;
        }
    }
}
