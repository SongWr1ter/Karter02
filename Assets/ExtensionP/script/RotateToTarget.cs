using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public float rotateSpeed;
    public float moveSpeed;
    private Vector3 direction;
    [UnityEngine.Header("statics")]
    [SerializeField]private float angle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //direction.z = 0;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;//tan 唯一确定一个弧度?yes,but start on x's positive side(right side)
        //angle = Vector3.SignedAngle(transform.up, direction, Vector3.forward);
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, rotateSpeed * Time.deltaTime);
        
        Vector3 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, cursorPos, moveSpeed * Time.deltaTime);
    }
}
