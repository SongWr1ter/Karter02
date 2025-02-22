using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public bool FreezeWhenStop;
    public int VerticeLength;
    [Tooltip("FreeWhenStop模式下：越小顶点走的越整齐")]
    public float speed;
    public float trailSpeed;
    private LineRenderer lineRenderer;
    
    private Vector3[] vertices;
    private Vector3[] verticesVelocity;

    public Transform targetTrans;
    [Tooltip("lineRenderer中顶点之间间隔")]
    public float targetDistance;//dist btween per vert

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleTarget;

    public GameObject bodyPrefab;
    private List<Transform> bodyParts = new List<Transform>();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        vertices = new Vector3[VerticeLength];
        verticesVelocity = new Vector3[VerticeLength];
        lineRenderer.positionCount = VerticeLength;

        if(bodyPrefab != null)
            for (int i = 0; i < VerticeLength - 1; i++)
            {
                GameObject body = Instantiate(bodyPrefab, null);
                body.name = transform.name + "_Body_" + i;
                bodyParts.Add(body.transform);
                var r = body.AddComponent<BodyRotation>();
                r.speed = 10f;
                r.target = i == 0 ? targetTrans : bodyParts[i - 1];
            }
    }

    private void Update()
    {
        wiggleTarget.localRotation = Quaternion.Euler(0,0,Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);
        
        
        vertices[0] = targetTrans.position;
        for (int i = 1; i < vertices.Length; i++)
        {
            if (FreezeWhenStop)
            {
                Vector3 targetPos = vertices[i - 1] + (vertices[i] - vertices[i - 1]).normalized * targetDistance;
                vertices[i] = Vector3.SmoothDamp(vertices[i],targetPos,ref verticesVelocity[i],speed);
            }
            else
            {
                vertices[i] = Vector3.SmoothDamp(vertices[i],vertices[i-1] + targetTrans.right * targetDistance,ref verticesVelocity[i],speed + i / trailSpeed);
            }
            
            if(bodyPrefab != null)
                bodyParts[i-1].position = vertices[i];
        }
        lineRenderer.SetPositions(vertices);
    }
}
