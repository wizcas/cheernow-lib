/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

[System.Serializable]
public class NumberWheelValueChangedEvent : UnityEvent<int, NumberWheel> {
}

public class NumberWheel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler {
    private const float kEpsilon = 1E-2f;
    private const int defaultLoopLength = 10;
    /// <summary>
    /// 滚动速度
    /// </summary>
    public float speed = 10;
    /// <summary>
    /// 阻力值
    /// </summary>
    [Range(0, 100)]
    public float drag = 2;
    /// <summary>
    /// 鼠标滚轮倍数（滚动一格相当于移动了多少距离）
    /// </summary>
    public float scrollWheelMultiplier = 20f;

    /// <summary>
    /// 拨盘中的值集合，个数为0或设置为null时，默认使用0-9的值
    /// </summary>
    public int[] ValueCollection;

    private Text[] _bufferTexts = new Text[3];
    private Text[] BufferTexts
    {
        get
        {
            if (_bufferTexts[0] == null)
            {
                _bufferTexts = transform.Find("Mask").GetComponentsInChildren<Text>();
            }
            return _bufferTexts;
        }
    }
    public NumberWheelValueChangedEvent onValueChanged = new NumberWheelValueChangedEvent();

    private readonly int[] _bufferNumbers = new int[3]{ -1, -1, -1 };
    private int _valueIndex = -1;
    private int _curBufferIndex;
    private float _bufferHeight;
    private float _curBufferTargetY;
    private float _motion;
    private bool _isDragging;
    private int _prevValueIndexStop = -1;

    public int Value
    {
        get{ return _bufferNumbers[_curBufferIndex]; }
        set {
            ValueIndex = GetIndexByValue(value);
        }
    }

    public int ValueIndex
    {
        get
        {
            return _valueIndex;
        }
        set
        {
            _valueIndex = value;
            NavigateTo(_valueIndex);
//            if (IsStopped) // 停止滚动时才更新上次停住的值
//            {
//                FireValueChangeEvent();
//            }
        }
    }

    private bool IsStopped
    {
        get { return Mathf.Abs(_motion) < kEpsilon && !_isDragging; }
    }

    private bool HasValueCollection
    {
        get { return ValueCollection != null && ValueCollection.Length > 0; }
    }

    private int prevBufferIndex
    {
        get{ return RepeatInt(_curBufferIndex - 1, BufferTexts.Length); }
    }

    private int nextBufferIndex
    {
        get{ return RepeatInt(_curBufferIndex + 1, BufferTexts.Length); }
    }

    void Awake()
    {
        _bufferHeight = BufferTexts[0].rectTransform.rect.height;
        Value = 0;
        PlacePrevAndNext();
    }

    void Start()
    {
        if (ValueIndex < 0)
            ValueIndex = 0;
    }

    private int RepeatInt(int val, int total) {
        return (int)Mathf.Repeat(val, total);
    }

    private int GetIndexByValue(int value)
    {
        if (!HasValueCollection)
        {
            return RepeatInt(value, defaultLoopLength);
        }

        int minEpsilon = int.MaxValue;
        int index = -1;
        for(int i = 0; i < ValueCollection.Length; i++)
        {
            var val = ValueCollection[i];
            if (val == value)
            {
                index = i;
                break;
            }
            int e = Mathf.Abs(val - value);
            if (e < minEpsilon)
            {
                minEpsilon = e;
                index = i;
            }
        }
        return index;
    }

    private int GetValueByIndex(int index)
    {
        if (!HasValueCollection)
            return RepeatInt(index, defaultLoopLength);

        var valueIndex = RepeatInt(index, ValueCollection.Length);
        return ValueCollection[valueIndex];
    }

    private void NavigateTo(int valueIndex)
    {
        int loopLength = HasValueCollection ? ValueCollection.Length : defaultLoopLength;

        _bufferNumbers[_curBufferIndex] = GetValueByIndex(RepeatInt(valueIndex, loopLength));
        _bufferNumbers[prevBufferIndex] = GetValueByIndex(RepeatInt(valueIndex - 1, loopLength));
        _bufferNumbers[nextBufferIndex] = GetValueByIndex(RepeatInt(valueIndex + 1, loopLength));

        for (int i = 0; i < BufferTexts.Length; i++) {
            BufferTexts[i].text = _bufferNumbers[i].ToString();
        }
    }

    private void PlacePrevAndNext(){
        var prevBuf = BufferTexts[prevBufferIndex];
        var nextBuf = BufferTexts[nextBufferIndex];
        prevBuf.rectTransform.anchoredPosition = BufferTexts[_curBufferIndex].rectTransform.anchoredPosition + Vector2.up * _bufferHeight;
        nextBuf.rectTransform.anchoredPosition = BufferTexts[_curBufferIndex].rectTransform.anchoredPosition + Vector2.down * _bufferHeight;
    }

    private void FireValueChangeEvent()
    {
        if (_prevValueIndexStop != ValueIndex && onValueChanged != null)
        {
            _prevValueIndexStop = ValueIndex;
            onValueChanged.Invoke(_bufferNumbers[_curBufferIndex], this);
        }
    }

    void Update() {

        var curBuf = BufferTexts[_curBufferIndex];

        var curBufferTargetPos = Vector2.zero + Vector2.up * _curBufferTargetY;
        if (AreVectorsSame(curBufferTargetPos, curBuf.rectTransform.anchoredPosition))
            return;

        // 平滑移动数字盘到目标滚动位置
        curBuf.rectTransform.anchoredPosition = Vector2.Lerp(curBuf.rectTransform.anchoredPosition, curBufferTargetPos, Time.deltaTime * speed);
        if (AreVectorsSame(curBufferTargetPos, curBuf.rectTransform.anchoredPosition)) {
            curBuf.rectTransform.anchoredPosition = curBufferTargetPos;
        }

        // 调整上下两个数字位置
        PlacePrevAndNext();

        // 检查是否当前正对的数字已经切换，若已切换，则将下标进行移位，调整目标滚动位置，并刷新数字盘
        if (_isDragging || Mathf.Abs(_motion) > drag * .5f) { // 速度实在太慢的情况下不切换，否则会出现突然跳一下的情况
            var curY = curBuf.rectTransform.anchoredPosition.y;
            bool isValueChanged = false;
            if (curY > _bufferHeight * .5f) {
                _curBufferTargetY -= _bufferHeight - (curY - _bufferHeight * .5f);
                _curBufferIndex = nextBufferIndex;
                isValueChanged = true;
            }
            else if (curY < _bufferHeight * -.5f) {
                _curBufferTargetY += _bufferHeight - (curY - _bufferHeight * -.5f);
                _curBufferIndex = prevBufferIndex;
                isValueChanged = true;
            }
            if (isValueChanged) {
                Value = Value; // 刷新数字
            }
        }
         
        // 惯性滑动
        if (!_isDragging) {
            if (_motion != 0f) {
                _motion = Mathf.Lerp(_motion, 0f, Time.deltaTime * drag);
                if (Mathf.Abs(_motion) < kEpsilon) {
                    _motion = 0f;
                }
                else {
                    _curBufferTargetY = _motion;
                }
            }
            else {
                _curBufferTargetY = 0;
            }
        }

        FireValueChangeEvent();
    }

    private bool AreVectorsSame(Vector2 a, Vector2 b) {
        return (a - b).sqrMagnitude <= kEpsilon * kEpsilon;
    }

    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData) {
        _isDragging = true;
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData) {
        _motion = eventData.delta.y;
//        Debug.LogFormat("Dragging: {0}", motion);
        _curBufferTargetY += _motion;
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData) {
        _motion = eventData.delta.y;
//        Debug.LogFormat("Drag End: {0}", motion);
        _curBufferTargetY += _motion;
        _isDragging = false;
    }

    #endregion

    #region IScrollHandler implementation

    public void OnScroll(PointerEventData eventData) {
        _motion = eventData.scrollDelta.y * scrollWheelMultiplier;
//        Debug.LogFormat("Scrolling: {0}", motion);
        _curBufferTargetY += _motion;
    }

    #endregion

    #region For Editor Use

    #endregion
}
