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
    private Dictionary<string, List<(int Value, DateTime Timestamp)>> dataTable = new Dictionary<string, List<(int Value, DateTime Timestamp)>>();

    void Start()
    {
        StartCoroutine(UpdateDataRoutine());
    }

    private IEnumerator UpdateDataRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Wait for 5 seconds

            yield return GetDataRequest(); // Execute the GetDataRequest coroutine

            // Clear the existing data table
            dataTable.Clear();

            // Update the data table with new values
            createList();
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

            // Split the string into separate entries using the closing curly brace
            entries = responseData.Split(new string[] { "}" }, StringSplitOptions.RemoveEmptyEntries);

            yield break;          
        }
    }
    private void createList()
    {

        foreach (string entry in entries)
        {
            // Add back the closing curly brace since it got removed during the split
            string separatedEntry = entry + "}";

            // Parse the entry into a dictionary
            var entryData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(separatedEntry);

            // Extract the values
            string name = entryData["Name"].ToString();
            int value = Convert.ToInt32(entryData["Value"]);
            DateTime timestamp = Convert.ToDateTime(entryData["Date"]);

            // Add the values to the data table
            if (dataTable.ContainsKey(name))
            {
                dataTable[name].Add((value, timestamp));
            }
            else
            {
                dataTable[name] = new List<(int Value, DateTime Timestamp)> { (value, timestamp) };
            }
        }

        printList();
    }

    private void printList()
    {
        foreach (var kvp in dataTable)
        {
            Debug.Log("Name: " + kvp.Key);
            foreach (var entry in kvp.Value)
            {
                Debug.Log("Value: " + entry.Value + ", Timestamp: " + entry.Timestamp);
            }
        }
    }
}
