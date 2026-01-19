using UnityEngine;
using VectorViolet.Core.Stats;
using UnityEditor;

namespace VectorViolet.ProStatManager.Editor
{
    [CustomEditor(typeof(StatDefinition))]
    public class StatDefinitionEditor : UnityEditor.Editor
    {
        SerializedProperty displayName;
        SerializedProperty description;
        SerializedProperty icon;
        SerializedProperty isRangedStat;
        SerializedProperty gizmosColor;
        SerializedProperty type;

        private void OnEnable()
        {
            displayName = serializedObject.FindProperty("displayName");
            description = serializedObject.FindProperty("description");
            icon = serializedObject.FindProperty("icon");
            isRangedStat = serializedObject.FindProperty("isRangedStat");
            gizmosColor = serializedObject.FindProperty("gizmosColor");
            type = serializedObject.FindProperty("type");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(displayName);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(icon);
            EditorGUILayout.PropertyField(isRangedStat);

            if (isRangedStat.boolValue)
                EditorGUILayout.PropertyField(gizmosColor);

            EditorGUILayout.PropertyField(type);

            serializedObject.ApplyModifiedProperties();
        }

    }
}