using System.Collections.Generic;
using UnityEngine;

public class Talisman : MonoBehaviour
{
    LineRenderer lineRenderer;

    [SerializeField]
    List<TalismanPoint> allPoints = new();
    [SerializeField]
    List<TalismanPoint> drawnPoints = new();
    //[SerializeField]
    //ObservableValue<int> drawnPointsCount
    //{
    //    get
    //    {
    //        return new(drawnPoints.Count, OndrawnPointsCountChange);
    //    }
    //}
    TalismanPoint lastPoint => drawnPoints[^3];
    TalismanPoint thisPoint => drawnPoints[^2];
    TalismanPoint nextPoint => drawnPoints[^1];
    [HelpBox("可调↓", HelpBoxType.Info)]
    [Label("转弯阈值")][SerializeField]
    float veerThreshold = 30f;
    [HelpBox("可观测↓", HelpBoxType.Info)]
    [SerializeField]
    ObservableValue<int> drawnLineCount;
    [Label("最近一次的转弯角度")][SerializeField]
    float curAngle; //0-180 degrees
    private void Awake()
    {
        drawnLineCount = new(-1, OndrawnLineCountChange);
        Initialize();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Initialize();
            //RefreshAllPoints();
        }
    }
    void CalculateAngle()
    {
        if (drawnPoints.Count <= 2)
            return;
        Vector3 dirLast = thisPoint.transform.position - lastPoint.transform.position;
        Vector3 dirThis = nextPoint.transform.position - thisPoint.transform.position;
        curAngle = Vector3.Angle(dirLast, dirThis);
        if (curAngle >= veerThreshold)
            drawnLineCount.Value++;
    }
    void Initialize()
    {
        lineRenderer = GetComponent<LineRenderer>();
        CollectAllPoints();
        RefreshAllPoints();
    }
    public void DrawPoint(TalismanPoint point)
    {
        drawnPoints.Add(point);OndrawnPointsChange();
        CalculateAngle();
    }
    void CollectAllPoints()
    {
        allPoints.Clear();
        drawnPoints.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            allPoints.Add(transform.GetChild(i).GetComponent<TalismanPoint>());
            allPoints[^1].Initialize(this);
        }
        //RefreshAllPoints();
    }
    void RefreshAllPoints()
    {
        foreach (var point in allPoints)
        {
            point.Refresh();
        }
        drawnLineCount.Value = 0;
        curAngle = 0f;
        drawnPoints.Clear();OndrawnPointsChange();
    }

    void OndrawnLineCountChange(int oldV,int newV)
    {
        //lineRenderer.positionCount = drawnPoints.Count;
        //for (int i = 0; i < drawnPoints.Count; i++)
        //    lineRenderer.SetPosition(i, drawnPoints[i].transform.position);
            
    }
    void OndrawnPointsChange(/*int oldV, int newV*/)
    {
        lineRenderer.positionCount = drawnPoints.Count;
        for (int i = 0; i < drawnPoints.Count; i++)
            lineRenderer.SetPosition(i, drawnPoints[i].transform.position);
    }
}
