using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Collections.Generic;

public class GetData : MonoBehaviour
{
    //private const string functionUrl = "http://localhost:7260/api/GetData";
    private const string functionUrl = "https://serverlessfunctionjd.azurewebsites.net/api/GetData";
    private string[] entries;
    internal Dictionary<string, List<(int Value, DateTime Timestamp)>> dataTable = new Dictionary<string, List<(int Value, DateTime Timestamp)>>();
    [SerializeField] private Dictionary_Graph graphScript;
    void Start()
    {
        StartCoroutine(UpdateDataRoutine());
    }

    private IEnumerator UpdateDataRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            yield return GetDataRequest();

            dataTable.Clear();

            createList();

            graphScript.showGraph(dataTable, graphScript.xValuesVisible, graphScript.separatorCount, (DateTime _dt) => _dt.ToString("HH:mm:ss"), (int _i) => _i.ToString());
        }
    }

    private IEnumerator GetDataRequest()
    {
        UnityWebRequest request = UnityWebRequest.Get(functionUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get data from the Azure Function: " + request.error);
        }
        else
        {
            string responseData = request.downloadHandler.text;

            entries = responseData.Split(new string[] { "}" }, StringSplitOptions.RemoveEmptyEntries);

            yield break;          
        }
    }
    private void createList()
    {

        foreach (string entry in entries)
        {
            string separatedEntry = entry + "}";

            var entryData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(separatedEntry);
            string name = entryData["Name"].ToString();
            int value = Convert.ToInt32(entryData["Value"]);
            DateTime timestamp = Convert.ToDateTime(entryData["Date"]);

            if (dataTable.ContainsKey(name))
            {
                dataTable[name].Add((value, timestamp));
            }
            else
            {
                dataTable[name] = new List<(int Value, DateTime Timestamp)> { (value, timestamp) };
            }
        }
    }
}
