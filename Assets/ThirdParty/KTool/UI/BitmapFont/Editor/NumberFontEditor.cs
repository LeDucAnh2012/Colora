using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KTool.UI.BitmapFont
{
    [CustomEditor(typeof(NumberFont))]
    [CanEditMultipleObjects]
    public class NumberFontEditor : Editor
    {
        SerializedProperty prefabText = null;
        SerializedProperty bitmaps = null;
        SerializedProperty alignment = null;
        SerializedProperty sizeHight = null;
        SerializedProperty space = null;
        SerializedProperty number = null;
        private NumberFont numberFont = null;
        private void OnEnable()
        {

            prefabText = serializedObject.FindProperty("prefabText");
            bitmaps = serializedObject.FindProperty("bitmaps");
            alignment = serializedObject.FindProperty("alignment");
            sizeHight = serializedObject.FindProperty("sizeHight");
            space = serializedObject.FindProperty("space");
            number = serializedObject.FindProperty("number");
            //
            numberFont = serializedObject.targetObject as NumberFont;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //
            EditorGUILayout.PropertyField(prefabText, new GUIContent("Prefab Text"));
            EditorGUILayout.PropertyField(bitmaps, new GUIContent("Bitmaps"));
            EditorGUILayout.PropertyField(alignment, new GUIContent("Alignment"));
            EditorGUILayout.PropertyField(sizeHight, new GUIContent("Size Hight"));
            EditorGUILayout.PropertyField(space, new GUIContent("Space"));
            EditorGUILayout.PropertyField(number, new GUIContent("Number"));
            if (GUILayout.Button("Rebuild"))
                numberFont.RebuildText();
            //
            serializedObject.ApplyModifiedProperties();

        }
    }
}
