using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Test_DownloadHandler : MonoBehaviour
{
    string downloadedText;

    const string url = "https://docs.google.com/spreadsheets/d/e/" +
        "2PACX-1vSro1d6_-qq849bGCdHpk1G3GJFEbN3HVTebeU9YyGRzeoscFmJkDapQ0ShaFdJ9y5njwW84FWOwBE0" +
        "/pub?gid=0&single=true&output=csv";

    private void Start()
    {
        StartCoroutine(GetText());
        Debug.Log(downloadedText);
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = new(url)
        {
            downloadHandler = new DownloadHandlerBuffer()
        };
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            //Debug.Log(www.downloadHandler.text);
            downloadedText = www.downloadHandler.text;
            //Debug.Log(downloadedText);

            List<string[]> l_str = ImportCSV.GetDataFromRawString(downloadedText, true);
            foreach (var item in l_str)
            {
                string a = "";
                for (int i = 0; i < item.Length; i++)
                {
                    a += item[i] + ",";
                }
                Debug.Log(a);
            }

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }
    }
}
