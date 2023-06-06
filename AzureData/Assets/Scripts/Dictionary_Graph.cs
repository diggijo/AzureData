using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;
using Unity.VisualScripting;

public class Dictionary_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private int xValuesVisible;
    [SerializeField] private int separatorCount;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private RectTransform labelTemplateTitle;
    private List<GameObject> gameObjects;

    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        labelTemplateTitle = graphContainer.Find("LabelTemplateTitle").GetComponent<RectTransform>();

        gameObjects = new List<GameObject>();

        Dictionary<string, List<(int Value, DateTime Timestamp)>> dataDictionary = new Dictionary<string, List<(int Value, DateTime Timestamp)>>();

        // Populate the dataDictionary with your data
        List<(int Value, DateTime Timestamp)> points = new List<(int Value, DateTime Timestamp)>()
        {
            (5, DateTime.Now),
            (98, DateTime.Now.AddHours(1)),
            (56, DateTime.Now.AddHours(2))
            // Add more data points here
        };

        // Add the data to the dictionary using a key (e.g., "Series 1")
        dataDictionary.Add("Series 1", points);

        showGraph(dataDictionary, xValuesVisible, separatorCount, (DateTime _dt) => _dt.ToString("HH:mm"), (int _i) => _i.ToString());
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject go = new GameObject("circle", typeof(Image));
        go.transform.SetParent(graphContainer, false);
        go.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return go;
    }

    private void showGraph(Dictionary<string, List<(int Value, DateTime Timestamp)>> valueDictionary, int xValuesOnDisplay, int yIncrements, Func<DateTime, string> axisLabelX = null, Func<int, string> axisLabelY = null)
    {
        if (axisLabelX == null)
        {
            axisLabelX = (_dt) => _dt.ToString();
        }
        if (axisLabelY == null)
        {
            axisLabelY = (_i) => _i.ToString();
        }

        if (xValuesOnDisplay <= 0)
        {
            throw new ArgumentException("Invalid value for xValuesOnDisplay. It must be greater than zero.");
        }

        foreach (GameObject go in gameObjects)
        {
            Destroy(go);
        }
        gameObjects.Clear();

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = 180f;
        float xSize = graphWidth / (xValuesOnDisplay + 1);
        int xIndex = 0;
        GameObject lastCircleGameObject = null;

        foreach (var entry in valueDictionary)
        {
            string key = entry.Key;
            List<(int Value, DateTime Timestamp)> valueList = entry.Value;

            RectTransform labelTitle = Instantiate(labelTemplateTitle);
            labelTitle.SetParent(graphContainer);
            labelTitle.gameObject.SetActive(true);
            labelTitle.anchoredPosition = new Vector2(420f, 350f);
            labelTitle.GetComponent<Text>().text = key;
            gameObjects.Add(labelTitle.gameObject);

            for (int i = Mathf.Max(valueList.Count - xValuesOnDisplay, 0); i < valueList.Count; i++)
            {
                float xPosition = (xIndex + 1) * xSize;
                float yPosition = ((valueList[i].Value / yMax)) * graphHeight;
                GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
                gameObjects.Add(circleGameObject);

                if (lastCircleGameObject != null)
                {
                    GameObject dotConntection = createDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                    gameObjects.Add(dotConntection);
                }

                lastCircleGameObject = circleGameObject;

                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPosition, -5f);
                labelX.GetComponent<Text>().text = axisLabelX(valueList[i].Timestamp);
                gameObjects.Add(labelX.gameObject);

                RectTransform dashX = Instantiate(dashTemplateX);
                dashX.SetParent(graphContainer, false);
                dashX.gameObject.SetActive(true);
                dashX.anchoredPosition = new Vector2(xPosition, 164f);
                gameObjects.Add(dashX.gameObject);

                xIndex++;
            }
        }

        for (int i = 0; i <= yIncrements; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / yIncrements;
            labelY.anchoredPosition = new Vector2(-35f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = axisLabelY((int)(normalizedValue * yMax));
            gameObjects.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(398.5f, normalizedValue * graphHeight);
            gameObjects.Add(dashY.gameObject);
        }
    }

    private GameObject createDotConnection(Vector2 lastDot, Vector2 nextDot)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (nextDot - lastDot).normalized;
        float distance = Vector2.Distance(lastDot, nextDot);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = lastDot + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));

        return gameObject;
    }
}
