using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(EdgeCollider2D))]
[RequireComponent(typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour
{
    [Header( "Hesh Generation" )]
    [Range(2, 500)] public int Num_Of_X_Vertices = 70;
    public float Width =10f;
    public float Height = 4f;
    public Material WaterMaterial; 
    private const int NUM_OF_Y_VERTICES = 2;
    [Header("Gizmo")]
    public Color GizmoCoLor = Color . white;
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Vector3[] vertices ;
    private int[] topVerticesIndex;
    private EdgeCollider2D edgeCollider;

    private void Start()
    {
        GenerateMesh();
    }

    private void Reset()//only run in editor
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.isTrigger = true;
    }
    public void ResetEdgeCollider()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] newPoints = new Vector2[2];
        Vector2 firstPoint = new Vector2(vertices[topVerticesIndex[0]].x ,vertices[topVerticesIndex[0]].y);
        newPoints[0] = firstPoint;
        Vector2 secondPoint = new Vector2(vertices[topVerticesIndex[topVerticesIndex.Length - 1]].x, vertices[
            topVerticesIndex[topVerticesIndex.Length - 1]].y);
        newPoints[1] = secondPoint;
        edgeCollider.offset = Vector2.zero;
        edgeCollider.points = newPoints;
    }

    public void GenerateMesh()
    {
        mesh = new Mesh();
        //add vertices
        vertices = new Vector3[Num_Of_X_Vertices * NUM_OF_Y_VERTICES] ; 
        topVerticesIndex = new int [Num_Of_X_Vertices];
        for(int y = 0; y < NUM_OF_Y_VERTICES; y++)
        {
            for (int x = 0; x < Num_Of_X_Vertices; x++)
            {
                float xPos = (x / (float)(Num_Of_X_Vertices - 1) )*Width - Width /2;
                float yPos = (y / (float)(NUM_OF_Y_VERTICES - 1))*Height - Height /2;
                vertices[y * Num_Of_X_Vertices + x] = new Vector3(xPos, yPos, 0f); 
                if (y == NUM_OF_Y_VERTICES -1)
                {
                    topVerticesIndex[x] = y*Num_Of_X_Vertices + x;
                }
            }
        }
    

        //construct triangles
        int[] triangles = new int[(Num_Of_X_Vertices - 1)*(NUM_OF_Y_VERTICES - 1)*6];
        int index = 0;
        for (int y = 0; y < NUM_OF_Y_VERTICES - 1; y++)
        {
            for (int x = 0; x < Num_Of_X_Vertices - 1; x++)
            {
                int bottomLeft = y*Num_Of_X_Vertices + x;
                int bottomRight = bottomLeft + 1 ;
                int topLeft = bottomLeft + Num_Of_X_Vertices ;
                int topRight = topLeft + 1;
                //first triangle
                triangles[index++] = bottomLeft;
                triangles[index++] = topLeft ;
                triangles[index++] = bottomRight;
                //second triangle
                triangles[index++] = bottomRight;
                triangles[index++] = topLeft ;
                triangles[index++] = topRight ;
            }
        }
        //UVs 
        Vector2[] uvs = new Vector2 [vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            uvs[i] = new Vector2((vertices[i].x + Width /2) / Width, ( vertices[i].y + Height /2) / Height);
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();
        meshRenderer.material = WaterMaterial;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds(); 
        meshFilter.mesh = mesh; 

        
    }


}
[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor
{
    private InteractableWater water;

    private void OnEnable()
    {
        water = (InteractableWater)target;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);
        root . Add(new VisualElement { style ={height = 10 } });
        Button generateMeshButton = new Button(() => water.GenerateMesh())
        {
            text = "Generate Mesh"
        };
        root.Add(generateMeshButton);
        Button placeEdgeColliderButton = new Button(() => water.ResetEdgeCollider())
        {
            text = "Place Edge Collider"
        };
        root.Add(placeEdgeColliderButton); 
        return root ;
    }

    private void ChangeDimensions(ref float width, ref float height, float calculatedWidthMax, float calculatedHeightMax)
    {
        width = Mathf.Max(0.1f, calculatedWidthMax);
        height = Mathf.Max(0.1f, calculatedHeightMax);
    }

    private void OnSceneGUI()
    {
         //Draw the wireframe box
        Handles.color =water.GizmoCoLor;
        Vector3 center =water . transform. position;
        Vector3 size = new Vector3( water .Width, water . Height, 6.1f);
        Handles . DrawWireCube(center,size);
        // Handles for width and height
        float handleSize = HandleUtility.GetHandleSize(center)*0.1f;
        Vector3 snap = Vector3.one * 0.1f;
        // Corner handles
        Vector3[] corners = new Vector3[4];
        corners[0] = center + new Vector3(-water.Width /2,-water .Height/2 ,0); // Bottom-left
        corners[1] = center + new Vector3(water .Width / 2, -water .Height / 2, 0); // Bottom-right 
        corners[2] = center + new Vector3(-water.Width / 2, water .Height / 2, 0); // Top-left 
        corners[3] = center + new Vector3(water .Width / 2, water .Height / 2, 0); // Top-right
        // Handle for each corner
        EditorGUI . BeginChangeCheck();
        Vector3 newBottomleft = Handles.FreeMoveHandle(corners[0], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref water . Width, ref water .Height, corners[1].x - newBottomleft .x,corners[3].y - newBottomleft.y);
            water . transform. position += new Vector3((newBottomleft.x - corners[0].x) / 2, (newBottomleft.y - corners[0].y) / 2, 0);
        }
        EditorGUI . BeginChangeCheck(); 
        Vector3 newBottomRight = Handles . FreeMoveHandle(corners[1], handleSize, snap, Handles.CubeHandleCap);
        if (EditorGUI.EndChangeCheck())
        {
            ChangeDimensions(ref water. Width, ref water . Height, newBottomRight.x - corners[0] .x, corners[3].y - newBottomRight .y);
            water . transform . position += new Vector3((newBottomRight.x - corners[1].x) / 2, (newBottomRight.y - corners[1].y) / 2, 0);

        }
        EditorGUI . BeginChangeCheck();
        Vector3 newTopleft = Handles . FreeMoveHandle(corners[2], handleSize, snap, Handles. CubeHandleCap);
        if (EditorGUI . EndChangeCheck())
        {
            ChangeDimensions(ref water . Width, ref water . Height, corners[3].x - newTopleft. x, newTopleft.y - corners[0] .y);
            water. transform . position += new Vector3((newTopleft.x - corners[2].x) / 2, (newTopleft.y - corners[2].y) / 2, 0);

        }
        EditorGUI. BeginChangeCheck();
        Vector3 newTopRight = Handles .FreeMoveHandle(corners[3], handleSize, snap, Handles .CubeHandleCap);
        if (EditorGUI . EndChangeCheck())
        {
            ChangeDimensions(ref water. Width, ref water . Height, newTopRight.x - corners[2].x, newTopRight.y - corners[1] .y);
            water. transform. position += new Vector3((newTopRight.x -corners[3].x) / 2, (newTopRight.y - corners[3].y) / 2, 0);

        }

        if (GUI.changed)
        {
            water.GenerateMesh();
        }
    }
    
       
    


}