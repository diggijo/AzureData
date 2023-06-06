using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;
using Unity.VisualScripting;

public class Windows_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjects;

    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        gameObjects = new List<GameObject>();

        List<int> points = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33, 100, 56, 45, 30, 22, 17 };

        showGraph(points, -10, (int _i) => " " + (_i + 1), (float _f) => Mathf.RoundToInt(_f) + "km/h");
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

    private void showGraph(List<int> valueList, int xValuesOnDisplay, Func<int, string> axisLabelX = null, Func<float, string> axisLabelY = null)
    {
        if (axisLabelX == null)
        {
            axisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (axisLabelY == null)
        {
            axisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        if (xValuesOnDisplay <= 0)
        {
            xValuesOnDisplay = valueList.Count;
        }

        foreach (GameObject go in gameObjects)
        {
            Destroy(go);
        }
        gameObjects.Clear();

        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;
        float yMax = valueList[0];
        float yMin = valueList[0];
        float xSize = graphWidth / (xValuesOnDisplay + 1);
        int xIndex = 0;
        GameObject lastCircleGameObject = null;

        foreach(int value in valueList)
        {
            if (value > yMax)
            {
                yMax = value;

            }
            if (value < yMin)
            {
                yMin = value;
            }
        }

        yMax = yMax + ((yMax - yMin) * .2f);
        yMin = yMin - ((yMax - yMin) * .2f);

        for(int i = Mathf.Max(valueList.Count - xValuesOnDisplay, 0); i < valueList.Count; i++)
        {
            float xPosition = (xIndex+1) * xSize;
            float yPosition = ((valueList[i] - yMin) / (yMax - yMin)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));          
            gameObjects.Add(circleGameObject);

            if(lastCircleGameObject != null)
            {
                GameObject dotConntection = createDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjects.Add(dotConntection);
            }
            
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -5f);
            labelX.GetComponent<Text>().text = axisLabelX(i);
            gameObjects.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, 164f);
            gameObjects.Add(dashX.gameObject);

            xIndex++;
        }

        int seperatorCount = 15;
        for(int i = 0; i <= seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / seperatorCount;
            labelY.anchoredPosition = new Vector2(-50f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = axisLabelY(normalizedValue * yMax);
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