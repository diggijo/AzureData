using System;
using UnityEngine;

public class Data_Visualization : MonoBehaviour
{
    [SerializeField] private GetData getData;
    [SerializeField] private Dictionary_Graph dictionaryGraph;
    [SerializeField] internal int xValuesVisible;
    [SerializeField] internal int separatorCount;

    private void Update()
    {
        // Access data from GetData class and pass it to Dictionary_Graph class for visualization
        if (getData.dataTable.Count > 0)
        {
            dictionaryGraph.showGraph(getData.dataTable, xValuesVisible, separatorCount, (DateTime _dt) => _dt.ToString("HH:mm"), (int _i) => _i.ToString());
        }
    }
}
