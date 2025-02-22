using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DragObject : MonoBehaviour
{
    private bool dragging = false;

    private Vector3 offset;
    [SerializeField]private LayerMask mask;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePosition);
            if (hit && hit.gameObject == gameObject)
            {
                StartDragging(mousePosition);
            }
        }

        

        if (Input.GetMouseButtonUp(0) && dragging)
        {
            CheckDropArea();
            dragging = false;
            transform.position = startPosition;
        }
    }

    private void LateUpdate()
    {
        if (dragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition + (Vector2)offset;
        }
    }

    void StartDragging(Vector2 hitPoint)
    {
        dragging = true;
        offset = transform.position - (Vector3)hitPoint;
    }

    void CheckDropArea()
    {
        Collider2D hit = Physics2D.OverlapPoint(transform.position,mask);
        if (hit)
        {
            //transform.position = hit.transform.position;
            Debug.Log(hit.name);
            if (hit.TryGetComponent<BucketObject>(out BucketObject bucket))
            {
                bucket.Recv(transform);
            }
        }
        
    }
}
