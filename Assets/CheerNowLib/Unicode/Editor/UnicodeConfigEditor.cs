using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

namespace CheerNow
{
    [CustomEditor(typeof(UnicodeConfig))]
    public class UnicodeConfigEditor : Editor
    {
        private Object obj;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            var t = target as UnicodeConfig;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();

            obj = AssetDatabase.LoadAssetAtPath(t.illegalExcluesFilePath, typeof(TextAsset));
            var tmpObj = EditorGUILayout.ObjectField("拖拽文件到这里", obj, typeof(TextAsset), false, null);
            if (tmpObj != obj)
            {
                t.illegalExcluesFilePath = AssetDatabase.GetAssetPath(tmpObj);
            }
            EditorGUILayout.TextField("排除项列表文件", AssetDatabase.GetAssetPath(obj));
            if (GUILayout.Button("解析排除项列表\n(不会包含在非法Unicode列表中的)"))
            {
                t.ParseIllegalExcludes();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("illegalExcludes"), true);

            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            obj = AssetDatabase.LoadAssetAtPath(t.illegalRangesFilePath, typeof(TextAsset));
            tmpObj = EditorGUILayout.ObjectField("拖拽文件到这里", obj, typeof(TextAsset), false, null);
            if (tmpObj != obj)
            {
                t.illegalRangesFilePath = AssetDatabase.GetAssetPath(tmpObj);
            }
            EditorGUILayout.TextField("非法Unicode列表文件", AssetDatabase.GetAssetPath(obj));
            if (GUILayout.Button("解析非法Unicode列表"))
            {
                t.ParseIllegalRanges();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("illegalRanges"), true);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorUtility.SetDirty(target);
        }
    }
}