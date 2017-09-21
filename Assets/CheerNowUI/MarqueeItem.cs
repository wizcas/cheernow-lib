/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Components.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class MarqueeItem : UIBehaviour
    {
        public object content { get; private set; }

        public RectTransform Rect
        {
            get { return (RectTransform)transform; }
        }

        public Vector2 Position
        {
            get
            {
                return Rect.anchoredPosition;
            }
            set
            {
                Rect.anchoredPosition = value;
            }
        }

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <returns>如果内容改变了则返回<c>true</c>；否则返回<c>false</c></returns>
        /// <param name="content">要设置的内容对象</param>
        public bool SetContent(object content)
        {
            if (IsSameContent(content))
                return false;
            
            this.content = content;
            gameObject.SetActive(content != null);
            if (content != null)
            {
                InnerSetContent(content);
            }
            return true;
        }

        protected virtual bool IsSameContent(object content)
        {
            return this.content == content;
        }

        protected virtual void InnerSetContent(object content)
        {
            GetComponent<Text>().text = content.ToString();
        }
    }
}
