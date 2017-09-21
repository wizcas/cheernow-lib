/**************************************
/* Created by Wizcas (http://wizcas.me)
**************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Cheers
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class ClickMe : Attribute
    {
        public string Title;

        public string BgColorString;
        public string TextColorString;

        public Color BackgroundColor {
            get {
                return ColorHelper.ByWeb(BgColorString);
            }
        }

        public Color TextColor {
            get {
                return ColorHelper.ByWeb(TextColorString);
            }
        }

        public ClickMe ()
        {
        }

        public ClickMe (string title, string bgColorString = null, string textColorString = null) : this()
        {
            Title = title;
            BgColorString = bgColorString;
            TextColorString = textColorString;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FlagEnum : PropertyAttribute
    {
    
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnly : PropertyAttribute
    {
    
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Help : PropertyAttribute
    {
        public string Text = "";

        public Help (string text)
        {
            Text = text;
        }

        public bool IsExpanded;
    }
}