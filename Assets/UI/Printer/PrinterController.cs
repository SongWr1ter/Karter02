using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterController : MonoBehaviour
{
    [TextArea]public string content;
    private TextPanel textPanel;
    bool flag;//死循环等待
    private Coroutine typingCoroutine; // 存储协程引用
    // Start is called before the first frame update
    void Start()
    {
        textPanel = transform.GetChild(0).gameObject.GetComponent<TextPanel>();
        textPanel.StartTyping(" ");
        
        typingCoroutine = StartCoroutine(sequenceShow());
    }

    private void Update()
    {
        if (flag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                flag = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Skip();
            typingCoroutine = StartCoroutine(sequenceShow());
        }
    }

    public void Skip()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            flag = false;
        }
    }

    IEnumerator sequenceShow()
    {
        textPanel.StartTyping(content);
        flag = true;
        while(flag) yield return new WaitForSeconds(0.2f);
    }
}
