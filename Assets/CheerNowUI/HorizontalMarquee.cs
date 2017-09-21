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
using UnityEngine.Events;
using System.Linq;

namespace Components.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalMarquee : UIBehaviour
    {
        #region Fields
        public MarqueeItem MarqueeItemPrefab;
        public float scrollSpeed = 50f;
        public int maxBufferSize = 5;
        public float spacing = 30f;

        public RectTransform marqueeArea;

        [SerializeField]
        private List<MarqueeItem> _messageBuffers = new List<MarqueeItem>();
        private int _curBufHeadIndex;

        private List<object> _messages = new List<object>();
        [SerializeField]
        private int _lastLoadedMessageIndex;

        private bool _isLayoutDirty;
        [SerializeField]
        private bool _isPlaying;
        private int _prevActiveBufferCount;
        #endregion

        #region Events
        /// <summary>
        /// 当第一条滚动消息出现时，触发此事件
        /// </summary>
        public UnityEvent onStartPlay;
        /// <summary>
        /// 当最后一条滚动消息播放完毕后，触发此事件
        /// </summary>
        public UnityEvent onEndPlay;
        #endregion

        #region Properties
        private int BufferSize
        {
            get
            {
                return _messageBuffers.Count;
            }
        }

        private Vector2 initMessagePosition
        {
            get
            {
                return Vector2.zero;
            }
        }
        #endregion

        #region Init
        protected override void Awake()
        {
            base.Awake();
            ResetBuffers();
        }
        #endregion

        #region Helpers
        private int RepeatInt(int v, int count)
        {
            return (int)Mathf.Repeat(v, count);
        }
        #endregion

        #region Buffer Controlling
        private void ResetBuffers()
        {
            for (int i = marqueeArea.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(marqueeArea.transform.GetChild(i).gameObject);
            }
            _messageBuffers.Clear();
            _curBufHeadIndex = 0;
        }

        private void LoadMessagesToBuffers()
        {
            if (_messages.Count <= 0) return;

            // 在允许范围内，如果消息数大于当前缓存数，则添加新的缓存
            while (BufferSize < maxBufferSize)
            {
                if (_messages.Count > BufferSize)
                {
                    var buffer = CreateMessageBuffer(BufferSize);
                    _messageBuffers.Add(buffer);
                }
                else // 当前Buffer数量能够容纳所有消息
                {
                    break;
                }
            }

            // 设置缓存区的每条消息
            for (int i = 0; i < BufferSize; i++)
            {
                var bufIndex = RepeatInt(_curBufHeadIndex + i, BufferSize);
                var buffer = _messageBuffers[bufIndex];
                if (i < _messages.Count)
                {
                    SetBuffer(buffer, _messages[i]);
                    _lastLoadedMessageIndex = i;
                }
                else
                {
                    SetBuffer(buffer, null);
                }
            }
        }

        private MarqueeItem CreateMessageBuffer(int index)
        {
            var buffer = Instantiate(MarqueeItemPrefab);
            buffer.Rect.SetParent(marqueeArea, false);
            buffer.Rect.pivot = new Vector2(0f, .5f); // 轴心点在左侧中间
            buffer.Rect.anchorMin = new Vector2(1f, 0f); // 锚定到右端，上下占满
            buffer.Rect.anchorMax = new Vector2(1f, 1f);
            buffer.name = "Buffer " + index;
            SetBuffer(buffer, null);
            return buffer;
        }

        private void SetBuffer(MarqueeItem buffer, object content)
        {
            if (buffer.SetContent(content))
            {
                buffer.Position = initMessagePosition;
                StartCoroutine(SetLayoutDirtyCo());
            }
        }

        private bool IsBufferLoaded(int index)
        {
            return IsBufferLoaded(_messageBuffers[index]);
        }

        private bool IsBufferLoaded(MarqueeItem buffer)
        {
            return buffer.isActiveAndEnabled;
        }

        private void RecalculateHeadBuffer()
        {
            var curBufHead = _messageBuffers[_curBufHeadIndex];
            float leftBoundX = -marqueeArea.rect.width - curBufHead.Rect.rect.width;

            if (curBufHead.Position.x < leftBoundX) // 当前第一条消息整条超出左边界
            {
                // 从队列头弹出一条消息
                _messages.RemoveAt(0);
                if (_messages.Count - _lastLoadedMessageIndex > 0) // 如果还有后续没放得下的消息
                {
                    SetBuffer(curBufHead, _messages[_lastLoadedMessageIndex]); // 将飘出范围的Buffer设置为下一条消息
                }
                else // 否则重置飘出范围的Buffer，并更新最后一条消息下标（因为消息列表长度改变了）
                {
                    _lastLoadedMessageIndex--;
                    SetBuffer(curBufHead, null);
                }
                _curBufHeadIndex = RepeatInt(_curBufHeadIndex + 1, BufferSize);
            }
        }
        #endregion

        #region Layouting & Playing

        private IEnumerator SetLayoutDirtyCo()
        {
            yield return new WaitForEndOfFrame();
            _isLayoutDirty = true;
        }

        private void LayoutBuffers()
        {
            float minLeft = float.MinValue;
            for (var i = 0; i < BufferSize; i++)
            {
                var actualIndex = RepeatInt(_curBufHeadIndex + i, BufferSize);
                var buffer = _messageBuffers[actualIndex];
                if (IsBufferLoaded(buffer)) // 有内容的Buffer按顺序排列,并保证后一条不会覆盖到前一条
                {
                    float leftX = Mathf.Max(minLeft, buffer.Position.x);
                    buffer.Position = new Vector2(leftX + 1, 0f);
                    minLeft = leftX + buffer.Rect.rect.width + spacing;
                }
                else // 没内容的Buffer全都堆在右边边框外
                {
                    buffer.Position = initMessagePosition;
                }
            }
        }

        private IEnumerator TogglePlayingStateCo(bool isPlay)
        {
            var e = isPlay ? onStartPlay : onEndPlay;
            e.Invoke();
            yield return new WaitForSeconds(.5f); // 播放缓冲事件/等待广播跑马灯显示完
            _isPlaying = isPlay;
        }

        void Update()
        {
            if (_isLayoutDirty)
            {
                LayoutBuffers();
                _isLayoutDirty = false;
                // 因为消息缓冲区发生变化，所以检查是否处于开始播放/停止播放的状态
                int activeBufferCount = _messageBuffers.Count(b => b.isActiveAndEnabled);
                Debug.LogFormat("activeBuffer: {0}, prev: {1}", activeBufferCount, _prevActiveBufferCount);
                if (activeBufferCount != _prevActiveBufferCount)
                {
                    if (activeBufferCount > 0 && _prevActiveBufferCount == 0)
                    {
                        StartCoroutine(TogglePlayingStateCo(true));
                    }
                    else if (activeBufferCount == 0 && _prevActiveBufferCount > 0)
                    {
                        StartCoroutine(TogglePlayingStateCo(false));
                    }
                    _prevActiveBufferCount = activeBufferCount;
                }
            }

            if (!_isPlaying) return;

            foreach (var buffer in _messageBuffers)
            {
                if (buffer.isActiveAndEnabled)
                {
                    buffer.Position = Vector2.Lerp(buffer.Position, buffer.Position + Vector2.left * scrollSpeed, Time.deltaTime);
                }
            }

            RecalculateHeadBuffer();
        }
        #endregion

        #region Public APIs
        public void FeedMessage(object message, bool append)
        {
            if (!append)
                _messages.Clear();
            _messages.Add(message);
            LoadMessagesToBuffers();
        }

        public void FeedMessages(IEnumerable<object> messages, bool append)
        {
            if (!append)
                _messages.Clear();
            _messages.AddRange(messages);
            LoadMessagesToBuffers();
        }

        public void ClearMessages()
        {
            ResetBuffers();
            _messages.Clear();
        }
        #endregion

        #region Editor Debugging

        [ContextMenu("Add 1 message")]
        public void AddOneMsg()
        {
            FeedMessages(new[] { "This is a test message (add)" }, true);
        }

        [ContextMenu("Add 5 message")]
        public void AddFiveMsgs()
        {
            FeedMessages(new[] {
                "This is a test message 1 (add)",
                "This is a test message 2 (add)",
                "This is a test message 3 (add)",
                "This is a test message 4 (add)",
                "This is a test message 5 (add)",
            }, true);
        }

        [ContextMenu("Reset 1 message")]
        public void ResetOneMsg()
        {
            FeedMessages(new[] { "This is a test message (reset)" }, false);
        }

        [ContextMenu("Reset 5 message")]
        public void ResetFiveMsgs()
        {
            FeedMessages(new[] {
                "This is a test message 1 (reset)",
                "This is a test message 2 (reset)",
                "This is a test message 3 (reset)",
                "This is a test message 4 (reset)",
                "This is a test message 5 (reset)",
            }, false);
        }

        #endregion
    }
}