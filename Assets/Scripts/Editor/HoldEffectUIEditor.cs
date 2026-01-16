using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HoldEffectUI))]
public class HoldEffectUIEditor : Editor
{ 
    SerializedProperty changeScale;
    SerializedProperty targetObject;
    SerializedProperty useDefaultScale;
    SerializedProperty originalScale;
    SerializedProperty activeScale;

    SerializedProperty changeAlpha;
    SerializedProperty targetCanvasGroup;
    SerializedProperty targetImage;
    SerializedProperty useDefaultAlpha;
    SerializedProperty originalAlpha;
    SerializedProperty activeAlpha;

    public void OnEnable()
    {
        changeScale = serializedObject.FindProperty("changeScale");
        targetObject = serializedObject.FindProperty("targetObject");
        useDefaultScale = serializedObject.FindProperty("useDefaultScale");
        originalScale = serializedObject.FindProperty("originalScale");
        activeScale = serializedObject.FindProperty("activeScale");

        changeAlpha = serializedObject.FindProperty("changeAlpha");
        targetCanvasGroup = serializedObject.FindProperty("targetCanvasGroup");
        targetImage = serializedObject.FindProperty("targetImage");
        useDefaultAlpha = serializedObject.FindProperty("useDefaultAlpha");
        originalAlpha = serializedObject.FindProperty("originalAlpha");
        activeAlpha = serializedObject.FindProperty("activeAlpha");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(changeScale);
        if (changeScale.boolValue)
        {
            EditorGUILayout.PropertyField(targetObject);
            EditorGUILayout.PropertyField(useDefaultScale);
            if (!useDefaultScale.boolValue)
                EditorGUILayout.PropertyField(originalScale);
            EditorGUILayout.PropertyField(activeScale);
        }

        EditorGUILayout.PropertyField(changeAlpha);
        if (changeAlpha.boolValue)
        {
            EditorGUILayout.PropertyField(targetCanvasGroup);
            EditorGUILayout.PropertyField(targetImage);
            EditorGUILayout.PropertyField(useDefaultAlpha);
            if (!useDefaultAlpha.boolValue)
            {
                EditorGUILayout.PropertyField(originalAlpha);    
            }
            EditorGUILayout.PropertyField(activeAlpha);
        }

        serializedObject.ApplyModifiedProperties();
    }
}