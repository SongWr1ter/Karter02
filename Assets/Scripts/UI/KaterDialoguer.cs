using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using NodeCanvas.DialogueTrees;
using TMPro;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class KaterDialoguer : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
        public class SubtitleDelays
        {
            public float characterDelay = 0.05f;
            public float sentenceDelay = 0.5f;
            public float commaDelay = 0.1f;
            public float finalDelay = 1.2f;
        }

        //Options...
        [Header("Input Options")]
        public bool skipOnInput;
        public bool waitForInput;

        //Group...
        [Header("Subtitles")]
        public RectTransform subtitlesGroup;
        private TMP_Text actorSpeech;
        private TMP_Text actorName;
        public RectTransform waitInputIndicator;
        public SubtitleDelays subtitleDelays = new SubtitleDelays();
        public List<AudioClip> typingSounds;
        public GameObject bubblePrefab;
        private List<GameObject> bubblesList = new List<GameObject>();
        [SerializeField]public int MAX_SUBTITLES = 20;
        private AudioSource playSource;
        [SerializeField]private ScrollRect scrollRect;
        private RectTransform content;
        //Group...
        [Header("Multiple Choice")]
        public RectTransform optionsGroup;
        public Button optionButton;
        private Dictionary<Button, int> cachedButtons;
        private Vector2 originalSubsPosition;
        private bool isWaitingChoice;
        
        //rich text
        private List<(string content, string tagLeft, string tagRight)> remarks;

        private AudioSource _localSource;
        private AudioSource localSource {
            get { return _localSource != null ? _localSource : _localSource = gameObject.AddComponent<AudioSource>(); }
        }


        private bool anyKeyDown;
        public void OnPointerClick(PointerEventData eventData)=>
            anyKeyDown = true;
        void LateUpdate() => anyKeyDown = false;


        void Awake()
        {
            // content = GetComponentInChildren<VerticalLayoutGroup>().transform as RectTransform;
            scrollRect = GetComponentInChildren<ScrollRect>();
            content = scrollRect.content;
            Subscribe(); Hide(); 
        }
        void OnEnable() { UnSubscribe(); Subscribe(); }
        void OnDisable() { UnSubscribe(); }

        void Subscribe() {
            DialogueTree.OnDialogueStarted += OnDialogueStarted;
            DialogueTree.OnDialoguePaused += OnDialoguePaused;
            DialogueTree.OnDialogueFinished += OnDialogueFinished;
            DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
            DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
        }

        void UnSubscribe() {
            DialogueTree.OnDialogueStarted -= OnDialogueStarted;
            DialogueTree.OnDialoguePaused -= OnDialoguePaused;
            DialogueTree.OnDialogueFinished -= OnDialogueFinished;
            DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
            DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;
        }

        void Hide() {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            waitInputIndicator.gameObject.SetActive(false);
            originalSubsPosition = subtitlesGroup.transform.position;
        }

        void OnDialogueStarted(DialogueTree dlg) {
            //nothing special...
        }

        void OnDialoguePaused(DialogueTree dlg) {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            StopAllCoroutines();
            if ( playSource != null ) playSource.Stop();
        }

        void OnDialogueFinished(DialogueTree dlg) {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            if ( cachedButtons != null ) {
                foreach ( var tempBtn in cachedButtons.Keys ) {
                    if ( tempBtn != null ) {
                        Destroy(tempBtn.gameObject);
                    }
                }
                cachedButtons = null;
            }
            StopAllCoroutines();
            if ( playSource != null ) playSource.Stop();
        }

        ///----------------------------------------------------------------------------------------------

        void OnSubtitlesRequest(SubtitlesRequestInfo info) {
            StartCoroutine(Internal_OnSubtitlesRequestInfo(info));
        }

        IEnumerator Internal_OnSubtitlesRequestInfo(SubtitlesRequestInfo info) {

            var text = info.statement.text;
            var audio = info.statement.audio;
            var actor = info.actor;
            var bubble = AddBubble();
            actorSpeech = bubble.Text;
            actorName = bubble.Name;
            
            subtitlesGroup.gameObject.SetActive(true);
            subtitlesGroup.position = originalSubsPosition;
            actorSpeech.text = "";

            actorName.text = actor.name;
            actorSpeech.color = actor.dialogueColor;
  
            if ( audio != null ) {//语音？
                var actorSource = actor.transform != null ? actor.transform.GetComponent<AudioSource>() : null;
                playSource = actorSource != null ? actorSource : localSource;
                playSource.clip = audio;
                playSource.Play();
                actorSpeech.text = text;
                var timer = 0f;
                while ( timer < audio.length ) {
                    if ( skipOnInput && anyKeyDown ) {
                        playSource.Stop();
                        break;
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            if ( audio == null ) {
                var tempText = "";
                var inputDown = false;
                bool fflag = false;//是否出现标签
                int remarksIndex = 0;
                //对文本进行解析，哪里有标签
                string fullTextOriginal = String.Copy(text);
                remarks = GetNestedTaggedContentPositions(ref text);
                
                if ( skipOnInput ) {
                    StartCoroutine(CheckInput(() => { inputDown = true; }));
                }

                for ( int i = 0; i < text.Length; i++ ) {

                    if ( skipOnInput && inputDown ) {
                        actorSpeech.text = fullTextOriginal;
                        yield return null;
                        break;
                    }

                    if ( subtitlesGroup.gameObject.activeSelf == false ) {
                        yield break;
                    }
                    fflag = false;
                    char c = text[i];
                    if (c == TAG_LABEL[0])//遇见了富文本
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
                            string tmp = tempText + tmpwrappedContent;
                            actorSpeech.text = tmp;
                            yield return StartCoroutine(DelayPrint(subtitleDelays.characterDelay));
                            PlayTypeSound();
                        }
                        remarksIndex++;
                        tempText += tmpwrappedContent;
                        fflag = true;
                    }
                    if(!fflag)
                        tempText += c;
                    yield return StartCoroutine(DelayPrint(subtitleDelays.characterDelay));
                    PlayTypeSound();
                    if ( c == '.' || c == '!' || c == '?' ) {
                        yield return StartCoroutine(DelayPrint(subtitleDelays.sentenceDelay));
                        PlayTypeSound();
                    }
                    if ( c == ',' ) {
                        yield return StartCoroutine(DelayPrint(subtitleDelays.commaDelay));
                        PlayTypeSound();
                    }

                    actorSpeech.text = tempText;
                }

                if ( !waitForInput ) {
                    yield return StartCoroutine(DelayPrint(subtitleDelays.finalDelay));
                }
            }

            if ( waitForInput ) {
                waitInputIndicator.gameObject.SetActive(true);
                while ( !anyKeyDown ) {
                    yield return null;
                }
                waitInputIndicator.gameObject.SetActive(false);
            }

            yield return null;
            subtitlesGroup.gameObject.SetActive(false);
            info.Continue();
        }

        void PlayTypeSound() {
            if ( typingSounds.Count > 0 ) {
                var sound = typingSounds[Random.Range(0, typingSounds.Count)];
                if ( sound != null ) {
                    localSource.PlayOneShot(sound, Random.Range(0.6f, 1f));
                }
            }
        }

        IEnumerator CheckInput(System.Action Do) {
            while ( !anyKeyDown ) {
                yield return null;
            }
            Do();
        }

        IEnumerator DelayPrint(float time) {
            var timer = 0f;
            while ( timer < time ) {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        ///----------------------------------------------------------------------------------------------

        void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info) {

            optionsGroup.gameObject.SetActive(true);
            //parent relation
            optionsGroup.transform.SetParent(content);
            optionsGroup.transform.SetAsLastSibling();
            
            var buttonHeight = optionButton.GetComponent<RectTransform>().rect.height;
            optionsGroup.sizeDelta = new Vector2(optionsGroup.sizeDelta.x, ( info.options.Values.Count * buttonHeight ) + 20);

            cachedButtons = new Dictionary<Button, int>();
            int i = 0;

            foreach ( KeyValuePair<IStatement, int> pair in info.options ) {
                var btn = (Button)Instantiate(optionButton);
                btn.gameObject.SetActive(true);
                btn.transform.SetParent(optionsGroup.transform, false);
                btn.transform.localPosition = (Vector3)optionButton.transform.localPosition - new Vector3(0, buttonHeight * i, 0);
                btn.GetComponentInChildren<TMP_Text>().text = pair.Key.text;
                cachedButtons.Add(btn, pair.Value);
                btn.onClick.AddListener(() => { Finalize(info, cachedButtons[btn]); });
                i++;
                RefreshContentUI();
            }

            if ( info.showLastStatement ) {
                subtitlesGroup.gameObject.SetActive(true);
                var newY = optionsGroup.position.y + optionsGroup.sizeDelta.y + 1;
                //subtitlesGroup.position = new Vector3(subtitlesGroup.position.x, newY, subtitlesGroup.position.z);
            }

            if ( info.availableTime > 0 ) {
                StartCoroutine(CountDown(info));
            }
        }

        IEnumerator CountDown(MultipleChoiceRequestInfo info) {
            isWaitingChoice = true;
            var timer = 0f;
            while ( timer < info.availableTime ) {
                if ( isWaitingChoice == false ) {
                    yield break;
                }
                timer += Time.deltaTime;
                SetMassAlpha(optionsGroup, Mathf.Lerp(1, 0, timer / info.availableTime));
                yield return null;
            }

            if ( isWaitingChoice ) {
                Finalize(info, info.options.Values.Last());
            }
        }

        void Finalize(MultipleChoiceRequestInfo info, int index) {
            isWaitingChoice = false;
            SetMassAlpha(optionsGroup, 1f);
            optionsGroup.gameObject.SetActive(false);
            subtitlesGroup.gameObject.SetActive(false);
            foreach ( var tempBtn in cachedButtons.Keys ) {
                Destroy(tempBtn.gameObject);
            }
            info.SelectOption(index);
            //parent relation
            optionsGroup.transform.SetParent(transform, false);
            optionsGroup.transform.SetAsLastSibling();
            RefreshContentUI();
        }

        void SetMassAlpha(RectTransform root, float alpha)
        {
            foreach (var graphic in root.GetComponentsInChildren<CanvasRenderer>())
            {
                graphic.SetAlpha(alpha);
            }
        }

        #region richText
        
        private const string TAG_LABEL = "&";//插入对话中判断是否带有标签
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
                input = input.Remove(match.Index, match.Length).Insert(match.Index, TAG_LABEL);;
            }
            print(input);
            return results;
        }
        //把原文本中的特殊符号为转化为标签
        static void OriginalTextTransformer(ref string text)
        {
            //xxxx$yy$xxx -> xxxx<1><2>yy<2><1>xxx
        }

        #endregion

        #region bubble

        bubble AddBubble()
        {
            GameObject bubble;
            if (bubblesList.Count >= MAX_SUBTITLES)
            {
                bubble = bubblesList[0];
                bubblesList.RemoveAt(0);
                bubble.SetActive(false);
                bubble.transform.SetAsLastSibling();
            }
            else
            {
                bubble = Instantiate(bubblePrefab, content.position, Quaternion.identity);
                bubble.transform.SetParent(content);
            }
            bubblesList.Add(bubble);
            
            
            RefreshContentUI(bubble);
            return bubble.GetComponent<bubble>();
        }

        private void RefreshContentUI(GameObject bubble = null)
        {
            // 强制立即更新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            // 使用协程确保在UI渲染后滚动
            StartCoroutine(ScrollToBottom(bubble));
        }
        
        private IEnumerator ScrollToBottom(GameObject bubble)
        {
            // 等待当前帧结束
            yield return new WaitForSeconds(0.2f);
            // 滚动到底部
            // DialogueUI.GetComponentInChildren<ScrollRect>().verticalNormalizedPosition = 0;
            // 替换直接设置position的代码
            if (bubble != null && bubble.activeSelf == false)
            {
                
                bubble.SetActive(true);
            }
            DOTween.To(() => scrollRect.verticalNormalizedPosition,
                x => scrollRect.verticalNormalizedPosition = x,
                0, .3f);
        }
    

        #endregion
}

