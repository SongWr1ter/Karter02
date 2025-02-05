using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextPanel : MonoBehaviour,IPointerClickHandler
{
    public TMP_Text dialogueText; // 用于显示对话框的Text组件
    private string fullText; // 要显示的完整文本
    public float delay = 0.1f; // 每个字符的显示间隔时间
    private bool isTyping = false; // 标记是否正在逐字显示文本
    private Coroutine typingCoroutine; // 存储协程引用
    private const string TagLabel = "$";
    private string currentText = "";
    private List<(string content, string tagLeft, string tagRight)> remarks;
    private string fullTextOriginal;
    public void StartTyping(string text)
    {
        if(isTyping) StopTyping();
        fullTextOriginal = String.Copy(text);
        remarks = GetNestedTaggedContentPositions(ref text);
        // for (int i = 0; i < resl.Count; i++)
        // {
        //     var value = resl[i];
        //     print(value.content);
        // }
        fullText = text;
        StartTyping();
    }
    void Update()
    {
        // 检测空格键是否被按下
        if (Input.GetMouseButtonDown(0) && isTyping)
        {
            StopTyping(); // 停止逐字显示
            dialogueText.text = fullTextOriginal; // 立即显示全部文本
        }
    }
 
    public void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(ShowText());
    }
 
    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }
    }
 
    IEnumerator ShowText()
    {
        isTyping = true;
        currentText = "";
        int remarksIndex = 0;
        bool fflag = false;//是否出现标签
        for (int i = 0; i < fullText.Length; i++)
        {
            fflag = false;
            char character = fullText[i];
            if (character == TagLabel[0])
            {
                var tagLeft = remarks[remarksIndex].tagLeft;
                var tagRight = remarks[remarksIndex].tagRight;
                var fullContent = remarks[remarksIndex].content;
                string wrappedContent = tagLeft + tagRight;
                int writeIndex = tagLeft.Length;
                string tmpwrappedContent = "";
                for (int j = 0; j <= fullContent.Length; j++)
                {
                    string cc = fullContent.Substring(0, j);
                    tmpwrappedContent = wrappedContent.Insert(writeIndex, cc);
                    string tmp = currentText + tmpwrappedContent;
                    dialogueText.text = tmp;
                    yield return new WaitForSeconds(delay);
                }
                remarksIndex++;
                currentText += tmpwrappedContent;
                fflag = true;
            }
            if(!fflag)
                currentText += character;
            //先输完标签里的，再输标签外面的
            //里面的:获取位置l,r.
            //外面的:获取位置outside
            //currentText = fullText.Substring(0, i);
            dialogueText.text = currentText;
            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
    }
    
    
    static List<(string content, string tagLeft, string tagRight)> GetNestedTaggedContentPositions(ref string input)
    {
        var results = new List<(string content, string tagLeft, string tagRight)>();
        string pattern = @"(<[^>]+>)+(.*?)(<\/[^>]+>)+"; // 匹配最外层标签及其内容的正则表达式

        while (true)
        {
            var match = Regex.Match(input, pattern);

            if (!match.Success)
                break;

            // 获取内容及其位置信息
            string content = match.Groups[2].Value; // 标签包裹的内容
            int contentStart = match.Groups[2].Index; // 内容起始位置
            int contentEnd = contentStart + content.Length - 1; // 内容终止位置
            string tagLeft = input.Substring(match.Groups[0].Index, contentStart - match.Groups[0].Index);//左标签的内容
            //右标签的左
            int tagRightStart = contentEnd + 1;
            int tagRightEnd = match.Index + match.Length - 1;
            string tagRight = input.Substring(tagRightStart, tagRightEnd - tagRightStart + 1);
            // print(tagLeft);
            // print(tagRight);
            // print(content);
            
            results.Add((content, tagLeft, tagRight));

            // 将匹配部分替换为占位符，避免重复处理
            input = input.Remove(match.Index, match.Length).Insert(match.Index, TagLabel);;
            //  print(match.Groups[0].Value + "|||" + match.Groups[0].Name);
            //  print(match.Groups[1].Value + "|||" + match.Groups[1].Name);
            //  print(match.Groups[2].Value + "|||" + match.Groups[2].Name);
            //  print(contentStart + "|||" + contentEnd);
            // print("=================================");
            //print(input+"\n"+content);
        }
        print(input);
        return results;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
 
        var pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(dialogueText, pos, eventData.enterEventCamera);
        if (linkIndex > -1)
        {
 
            Debug.Log("点击" + dialogueText.textInfo.linkInfo[linkIndex].GetLinkText()+": 神话中blabla的所在地,具有blabla的功能");
        }
 
    }
}
