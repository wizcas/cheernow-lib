/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;
using CheerNow;

namespace CheerNow
{
    [CustomPropertyDrawer(typeof(UnicodeRange))]
    public class UnicodeRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            Rect contentPosition = EditorGUI.PrefixLabel(position, label);

            contentPosition.width *= .5f;
            var prop = property.FindPropertyRelative("min");
            DrawValueField(contentPosition, prop);

            contentPosition.x += contentPosition.width;
            GUI.Label(contentPosition, "..");
            prop = property.FindPropertyRelative("max");
            DrawValueField(contentPosition, prop);

            EditorGUI.EndProperty();
        }

        private void DrawValueField(Rect rect, SerializedProperty valueProp)
        {
            string tmpVal = valueProp.intValue.ToString("X4");
            tmpVal = EditorGUI.TextField(rect, tmpVal);
            try
            {
                var intVal = System.Convert.ToInt32(tmpVal, 16);

                valueProp.intValue = intVal;
            }
            catch(System.FormatException)
            {
                Debug.LogErrorFormat("Invalid Unicode: {0}", tmpVal);
            }
        }
    }
}