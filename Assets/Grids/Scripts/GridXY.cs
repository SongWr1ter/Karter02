using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
namespace UnityEngine
{
    /// <summary>
    /// 设置属性只读
    /// </summary>
    public class DisplayOnly : PropertyAttribute
    {

    }
    [CustomPropertyDrawer(typeof(DisplayOnly))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}

public class Grid<TGirdObject>
{
    private int height;
    private int length;
    private float gridSize;
    private TGirdObject[,] gridObjectArray;
    private Vector3 startPosition;
    public struct GridEventArgs
    {
        public int row;
        public int col;
    }
    public delegate void OnGridObjectChangedDelegate(GridEventArgs args);
    public event OnGridObjectChangedDelegate OnGridValueChanged;
    public Grid(int height,int length,float gridSize,Vector3 startPosition,Func<Grid<TGirdObject>,int,int,TGirdObject> createGridObject)
    {
        this.height = height;
        this.length = length;
        this.gridSize = gridSize;
        gridObjectArray = new TGirdObject[height,length];//数组xy（行列）和坐标xy（横纵）是反着来的，我们按数组的来
        this.startPosition = startPosition;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < length; col++)
            {
                gridObjectArray[row,col] = createGridObject(this,row,col);
            }
        }
        
    }
    
    public int Height(){return height;}
    public int Length(){return length;}

    /// <summary>
    /// 通过坐标获取对象
    /// </summary>
    /// <param name="col">correspond to x or length</param>
    /// <param name="row">correspond to y or height</param>
    /// <returns></returns>
    public TGirdObject GridObjectArray(int col, int row)
    {
        if(row >= 0 && row < height && col >= 0 && col < length)
            return gridObjectArray[row,col];
        return default(TGirdObject);
    }
    
    #region Grid Interface

    public TGirdObject GetGridObjectByWorldPosition(Vector3 position)
    {
        int x, y;
        GetXY(position, out x, out y);
        return GridObjectArray(x,y);
    }
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x,y) * gridSize + startPosition;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
//        print(worldPos);
        worldPos -= startPosition + new Vector3(gridSize / 2.0f, gridSize / 2.0f, 0);
        x = Mathf.RoundToInt(worldPos.x / gridSize);
        y = Mathf.RoundToInt(worldPos.y / gridSize);
    }
    
    public void SetGridObjectValue(int x, int y,TGirdObject value)
    {//数组xy（行列）和坐标xy（横纵）是反着来的，我们按数组的来
        if(y < 0 || y >= height|| x < 0 || x >= length) return;
        if(gridObjectArray[y,x] == null) return;
        gridObjectArray[y,x] = value;
        OnGridValueChanged?.Invoke(new GridEventArgs
        {
            row = y,col = x
        });
    }

    public void TriggerGridValueChanged(int x, int y)
    {
        if(y < 0 || y >= height|| x < 0 || x >= length) return;
        if(gridObjectArray[y,x] == null) return;
        OnGridValueChanged?.Invoke(new GridEventArgs
        {
            row = y,col = x
        });
    }
    
    public void SetGridObjectValue(Vector3 worldPos,TGirdObject value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetGridObjectValue(x, y,value);
    }

    #endregion

}
public class GridXY : MonoBehaviour
{
    public float length;
    public float width;
    public float nodeSize = 1f;
    public Sprite nodeSprite;
    [DisplayOnly]
    public int nodeCount;

    private int colCount;
    private int rowCount;
    private Vector3 nodeStartPos;//长宽不能被结点尺寸整除时，采取1、砍边策略2、居中策略
    private int Node_Start_Strategy = 2;
    private Vector3 leftDown;
    private Vector3 rightUp;
    private Grid<NormalGridObjectClass> grid;
    private void Awake()
    {
        GenerateGird();
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 vec = Utils.GetMouseWorldPosition();
            vec.z = 0;
            NormalGridObjectClass normalObject = grid.GetGridObjectByWorldPosition(vec);
            normalObject?.ChangeValue(10f);
        }
    }
    

    public void GenerateGird()
    {
        leftDown = transform.position + new Vector3(-width / 2.0f, -length / 2.0f, 0);
        rightUp = transform.position + new Vector3(width / 2.0f, length / 2.0f, 0);
        rowCount = Mathf.RoundToInt(length / nodeSize);
        colCount = Mathf.RoundToInt(width / nodeSize);
        nodeCount = rowCount * colCount;
        
        if (Node_Start_Strategy == 1)
        {
            nodeStartPos = leftDown;
        }
        else
        {
            float actualLength = length - rowCount * nodeSize;
            float actualWidth = width - colCount * nodeSize;
            nodeStartPos = leftDown + new Vector3(actualLength / 2.0f, actualWidth / 2.0f, 0);
        }
        //row 是行数对于height col是列数对应length
        grid = new Grid<NormalGridObjectClass>(rowCount, colCount,nodeSize,nodeStartPos, 
            (grid,x,y)=>new NormalGridObjectClass(grid,x,y));
        //Visualization
        TextMeshPro[,] debugTextArray = new TextMeshPro[rowCount, colCount];
        GameObject parent = new GameObject("Grid");
        Vector3 offset = new Vector3(nodeSize / 2.0f, nodeSize / 2.0f, 0);
        for (int i = 0; i < grid.Height(); i++)
        {
            for (int j = 0; j < grid.Length(); j++)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Quad);
                var text =  g.AddComponent<TextMeshPro>();
                debugTextArray[i, j] = text;
                text.text = grid.GridObjectArray(j, i).ToString();
                text.color = Color.cyan;
                text.fontSize = 2.0f * nodeSize;
                text.alignment = TextAlignmentOptions.Center;
                text.rectTransform.sizeDelta = new Vector2(nodeSize, nodeSize);
                g.transform.SetParent(parent.transform);
                g.name = "Grid" + "(" + (j+1) + ", " + (i+1) + ")";//数组xy（行列）和坐标xy（横纵）是反着来的，
                                                           //我们按数组的来，但表现层按坐标来
                g.transform.position = nodeStartPos + new Vector3(j * nodeSize, i * nodeSize,0) + offset;
            }
        }

        grid.OnGridValueChanged += (args) =>
        {
            debugTextArray[args.row, args.col].text = grid.GridObjectArray(args.col, args.row).ToString();
        };
    }
    bool debug = true;

    public void ShowGird(bool show)
    {
        debug = show;
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            DrawSqure(leftDown, rightUp); 
            Gizmos.color = Color.yellow;
            //以此物体position为中心构造一个矩形
              
            DrawGrid(nodeStartPos,rowCount,colCount,nodeSize);
        }
    }


    static void DrawSqure(Vector3 leftDown, Vector3 rightUp)
    {
        Vector3 rightDown = new Vector3(rightUp.x, leftDown.y, rightUp.z);
        Vector3 leftUp = new Vector3(leftDown.x, rightUp.y, rightUp.z);
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(rightUp, rightDown);
        Gizmos.DrawLine(leftDown, rightUp);
        Gizmos.DrawLine(rightUp, leftUp);
        Gizmos.DrawLine(leftDown,rightDown);
    }

    static void DrawGrid(Vector3 startPos,int rowCount, int colCount,float size)
    {
        Vector3 XOffset = new Vector3(colCount* size,0,0);
        Vector3 YOffset = new Vector3(0,rowCount * size,0);
        for (int i = 0; i <= colCount; i++)
        {
            Vector3 from = startPos + new Vector3((float)i * size, 0, 0);
            Vector3 to = from + YOffset;
            Gizmos.DrawLine(from, to);
        }
        for (int j = 0; j <= rowCount; j++)
        {
            Vector3 from = startPos + new Vector3(0, (float)j * size, 0);
            Vector3 to = from + XOffset;
            Gizmos.DrawLine(from, to);
        }
    }
}

public class NormalGridObjectClass
{
    private int x;
    private int y;
    private Grid<NormalGridObjectClass> grid;

    private float value;
    //usage:grid[y,x][row,col],数组xy（行列）和坐标xy（横纵）是反着来的，我们按数组的来
    public NormalGridObjectClass(Grid<NormalGridObjectClass> grid, int row, int col)
    {
        this.grid = grid;
        this.x = col;
        this.y = row;
        value = 0f;
    }

    public void ChangeValue(float value)
    {
        this.value = value;
        grid.TriggerGridValueChanged(x,y);
    }

    public override string ToString()
    {
        return value.ToString();
    }
}

public static class Utils
{
    public static Vector3 GetMouseWorldPosition()
    {
        var vec = Input.mousePosition;
        //  注意：这里的**z**轴，如果不设置z轴，调用下面转换坐标，返回的是相机当前的世界坐标
        if(Camera.main is not null && Camera.main.orthographic == false)
            vec.z -= Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(vec);
    }
}
