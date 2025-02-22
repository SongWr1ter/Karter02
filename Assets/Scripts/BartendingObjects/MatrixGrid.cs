using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatrixGridObject
{
    private Grid<MatrixGridObject> grid;
    private int x;
    private int y;
    public MatrixGridObject parent;
    public enum Walkable
    {
        walkable,
        obstacle
    }
    private float value;
    //usage:grid[y,x][row,col],数组xy（行列）和坐标xy（横纵）是反着来的，我们按数组的来
    public MatrixGridObject(Grid<MatrixGridObject> grid, int row, int col)
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
        return value.ToString(CultureInfo.InvariantCulture);
    }
    
    public float Value() {return value;}
    public Vector2 GetGridPosition() {return new Vector2(x,y);}
    public int GetGridX() {return x;}
    public int GetGridY() {return y;}
}
public class MatrixGrid : MonoBehaviour
{
    private Grid<MatrixGridObject> grid;
    [Header("Matrix Grid")]
    [SerializeField]private Vector2 mapSize;
    [SerializeField]private float mapGridSize;
    [SerializeField]private Sprite gridSprite;
    //[Tooltip("改变Grid的大小不会改变地图大小，而是改变Grid的稠密程度")]
    [SerializeField]private Transform startObjectTrans;//产生AStar网格游戏对象的坐标
    private Bartending.Matrix4X5 weight = new Bartending.Matrix4X5();
    public CharacterWeightSet charcSet;

    private Vector3 startPos;
    private Vector3 leftDown;
    private Vector3 rightUp;
    [SerializeField,DisplayOnly]private int rowCount = 4;
    [SerializeField,DisplayOnly]private int colCount = 5;
    [DisplayOnly]public int nodeCount;
    private int Node_Start_Strategy = 2;
    private Vector3 nodeStartPos;
    
    private void Awake()
    {
        Init();
    }
    
    public void Init()
    {
        leftDown = startObjectTrans.position + new Vector3(-mapSize.x / 2.0f, -mapSize.y / 2.0f, 0);
        rightUp = startObjectTrans.position + new Vector3(mapSize.x / 2.0f, mapSize.y / 2.0f, 0);
        //rowCount = Mathf.RoundToInt(mapSize.y / mapGridSize);
        //colCount = Mathf.RoundToInt(mapSize.x / mapGridSize);
        nodeCount = rowCount * colCount;
        
        if (Node_Start_Strategy == 1)
        {
            nodeStartPos = leftDown;
        }
        else
        {
            float actualLength = mapSize.y - rowCount * mapGridSize;
            float actualWidth = mapSize.x - colCount * mapGridSize;
            nodeStartPos = leftDown + new Vector3(actualLength / 2.0f, actualWidth / 2.0f, 0);
        }
        
        grid = new Grid<MatrixGridObject>(rowCount,colCount,mapGridSize,startObjectTrans.position,((grid1, row, col) =>
        {
            return new MatrixGridObject(grid1,row,col);
        }));
        
        Visualization("MatrixGrid4x5");

        Refresh();
    }

    void Visualization(string parentName)
    {
        //Visualization
        TextMeshPro[,] debugTextArray = new TextMeshPro[rowCount, colCount];
        GameObject parent = new GameObject(parentName);
        parent.transform.SetParent(startObjectTrans);
        Vector3 offset = new Vector3(mapGridSize / 2.0f, mapGridSize / 2.0f, 0);
        for (int i = 0; i < grid.Height(); i++)
        {
            for (int j = 0; j < grid.Length(); j++)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Quad);
                var text =  g.AddComponent<TextMeshPro>();
                debugTextArray[i, j] = text;
                text.text = grid.GridObjectArray(j, i).ToString();
                text.color = Color.white;
                text.fontSize = 8.0f * mapGridSize;
                text.alignment = TextAlignmentOptions.Center;
                text.rectTransform.sizeDelta = new Vector2(mapGridSize, mapGridSize);
                g.transform.SetParent(parent.transform);
                g.name = "Grid" + "(" + (j+1) + ", " + (i+1) + ")";//数组xy（行列）和坐标xy（横纵）是反着来的，
                //我们按数组的来，但表现层按坐标来
                g.transform.position = nodeStartPos + new Vector3(j * mapGridSize, -i * mapGridSize,0) + offset;
                //add sprite
                var child = new GameObject("sprite").AddComponent<Image>();
                child.transform.SetParent(g.transform,false);
                child.sprite = gridSprite;
                child.rectTransform.sizeDelta = new Vector2(mapGridSize, mapGridSize);
                child.transform.position = g.transform.position + Vector3.forward;
                if (i == 0)
                {
                    child.color = Color.blue;
                }else if (i == 1)
                {
                    child.color = Color.red;
                }else if (i == 2)
                {
                    child.color = Color.green;
                }else if (i == 3)
                {
                    child.color = Color.yellow;
                }
                else
                {
                    child.color = Color.magenta;
                }

                if (i == 0)
                {
                    //add title
                    string[] titles = { "Sweet","Spicy","Hot","Milk","Tomato"};
                    var title = GameObject.CreatePrimitive(PrimitiveType.Quad).AddComponent<TextMeshPro>();
                    title.text = titles[j];
                    title.fontStyle = FontStyles.Bold;
                    title.color = Color.white;
                    title.fontSize = 2.2f * mapGridSize;
                    title.alignment = TextAlignmentOptions.Center;
                    title.rectTransform.sizeDelta = new Vector2(mapGridSize, mapGridSize);
                    title.transform.SetParent(parent.transform);
                    title.name = "Title(" + (i+1) + ")";//数组xy（行列）和坐标xy（横纵）是反着来的，
                    //我们按数组的来，但表现层按坐标来
                    title.transform.position = nodeStartPos + new Vector3(j * mapGridSize, 0.6f * mapGridSize,0) + offset;

                }
            }
        }

        grid.OnGridValueChanged += (args) =>
        {
            debugTextArray[args.row, args.col].text = grid.GridObjectArray(args.col, args.row).ToString();
        };
    }

    private void Refresh()
    {
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                grid.GridObjectArray(col,row).ChangeValue(weight[row,col]);
            }
        }
    }
    private void OnEnable()
    {
        MessageCenter.AddListener(OnCharcSelc,MESSAGE_TYPE.CharcSelc);
    }

    private void OnDisable()
    {
        MessageCenter.RemoveListener(OnCharcSelc,MESSAGE_TYPE.CharcSelc);
    }
    public void OnCharcSelc(CommonMessage msg)
    {
        string name = (string)msg.content;
        weight = charcSet.GetWeight(name);
        Refresh();
    }

    #region GIZMOS

    bool debug = false;

    private void OnDrawGizmos()
    {
        if (debug)
        {
            DrawSqure(leftDown, rightUp); 
            Gizmos.color = Color.yellow;
            //以此物体position为中心构造一个矩形
              
            DrawGrid(nodeStartPos,rowCount,colCount,mapGridSize);
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

    #endregion
}
