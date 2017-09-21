/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class VerticalMarquee : UIBehaviour
    {
        public Text TextItemPrefab;
        public float scrollSpeed;

        private int _bufferSize;
        private Text[] _textBuffers;
        private int _curBufHeadIndex;
        private float _curBufHeadY;

        private List<string> _messages = new List<string>();
        private int _curMsgIndex;

        private RectTransform Rect
        {
            get { return (RectTransform) transform; }
        }

        private int RepeatInt(int v, int count)
        {
            return (int) Mathf.Repeat(v, count);
        }

        private void BuildTextBuffers(bool forceRebuild)
        {
            if (_textBuffers != null || _bufferSize > 0)
            {
                if (!forceRebuild) return;

                DestroyBuffers();
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
            _bufferSize = Mathf.CeilToInt(Rect.rect.height / (TextItemPrefab.transform as RectTransform).rect.height) +
                          1;
            _textBuffers = new Text[_bufferSize];
            for (int i = 0; i < _bufferSize; i++)
            {
                var buffer = Instantiate(TextItemPrefab);
                var bufferRect = buffer.transform as RectTransform;
                bufferRect.SetParent(transform, false);
                bufferRect.pivot = new Vector2(.5f, 1f); // 轴心点在顶部中间
                bufferRect.anchorMin = new Vector2(0f, .5f);
                bufferRect.anchorMax = new Vector2(1f, .5f);
                bufferRect.sizeDelta = new Vector2(0f, bufferRect.sizeDelta.y);
                _textBuffers[i] = buffer;
                buffer.name = "Buffer " + i;
                buffer.text = "";
            }
            LayoutBuffers();
        }

        private void DestroyBuffers()
        {
            if (_textBuffers == null || _bufferSize <= 0) return;

            for (int i = _bufferSize - 1; i >= 0; i--)
            {
                Destroy(_textBuffers[i].gameObject);
            }
            _textBuffers = null;
            _bufferSize = 0;
        }

        private void LoadMessagesToBuffers()
        {
            if (_messages.Count > 0)
            {
                for (int i = 0; i < _bufferSize; i++)
                {
                    var bufIndex = RepeatInt(_curBufHeadIndex + i, _bufferSize);
                    var msgIndex = RepeatInt(_curMsgIndex + i, _messages.Count);
                    var buffer = _textBuffers[bufIndex];
                    buffer.text = _messages[msgIndex];
                }
            }
        }

        private void LayoutBuffers()
        {
            float prevMinY = _curBufHeadY;
            for (var i = 0; i < _bufferSize; i++)
            {
                var actualIndex = RepeatInt(_curBufHeadIndex + i, _bufferSize);
                var bufferRect = _textBuffers[actualIndex].transform as RectTransform;
                bufferRect.anchoredPosition = new Vector2(0f, prevMinY);
                prevMinY -= bufferRect.rect.height;
            }
        }

        private void RecalculateBuffer()
        {
            var curBufHead = _textBuffers[_curBufHeadIndex];
            var curBufHeadRect = curBufHead.rectTransform;

            float upperBoundY = Rect.rect.height * .5f + curBufHeadRect.rect.height;

//            Debug.LogFormat("Head ({0}) @ {1}. Dies on {2}", curBufHead.name, curBufHeadRect.anchoredPosition.y,
//                upperBoundY);

            if (curBufHeadRect.anchoredPosition.y > upperBoundY) // 超出上边界
            {
                if (_messages.Count > 0)
                {
                    _curMsgIndex = (int) Mathf.Repeat(_curMsgIndex + 1, _messages.Count);
                    curBufHead.text = _messages[_curMsgIndex];
                }
                _curBufHeadIndex = (int) Mathf.Repeat(_curBufHeadIndex + 1, _bufferSize);
                _curBufHeadY = _textBuffers[_curBufHeadIndex].rectTransform.anchoredPosition.y;
            }
        }

        void Update()
        {
            if (!IsActive()) return;
            if (_textBuffers == null || _bufferSize <= 0) return;

            _curBufHeadY += scrollSpeed * Time.deltaTime;
            RecalculateBuffer();
            LayoutBuffers();
        }

        public void FeedMessages(IEnumerable<string> messages, bool append)
        {
            if(!append)
                _messages.Clear();
            _messages.AddRange(messages);

            BuildTextBuffers(false);
            LoadMessagesToBuffers();
        }

        public void ClearMessages()
        {
            DestroyBuffers();
            _messages.Clear();
        }
    }
}