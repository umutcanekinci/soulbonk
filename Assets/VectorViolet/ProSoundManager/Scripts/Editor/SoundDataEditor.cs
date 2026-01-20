using UnityEngine;
using UnityEditor;
using VectorViolet.Core.Audio;

namespace VectorViolet.Editor
{
    [CustomEditor(typeof(SoundData))]
    public class SoundDataEditor : UnityEditor.Editor
    {
        SerializedProperty clip;
        SerializedProperty volume;
        SerializedProperty pitch;
        SerializedProperty loop;
        
        SerializedProperty useRandomPitch;
        SerializedProperty randomPitchRange;
        
        SerializedProperty spatialBlend;
        SerializedProperty rolloffMode;
        SerializedProperty minDistance;
        SerializedProperty maxDistance;

        private void OnEnable()
        {
            
            clip = serializedObject.FindProperty("clip");
            volume = serializedObject.FindProperty("volume");
            pitch = serializedObject.FindProperty("pitch");
            loop = serializedObject.FindProperty("loop");
            
            useRandomPitch = serializedObject.FindProperty("useRandomPitch");
            randomPitchRange = serializedObject.FindProperty("randomPitchRange");
            
            spatialBlend = serializedObject.FindProperty("spatialBlend");
            rolloffMode = serializedObject.FindProperty("rolloffMode");
            minDistance = serializedObject.FindProperty("minDistance");
            maxDistance = serializedObject.FindProperty("maxDistance");
        }

        public override void OnInspectorGUI()
        {
            
            serializedObject.Update();

            
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(clip);
            EditorGUILayout.PropertyField(volume);
            EditorGUILayout.PropertyField(pitch);
            EditorGUILayout.PropertyField(loop);

            EditorGUILayout.Space(); 

            
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useRandomPitch);

            
            if (useRandomPitch.boolValue)
            {
                
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(randomPitchRange);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            
            EditorGUILayout.LabelField("3D Settings", EditorStyles.boldLabel);
            
            
            EditorGUILayout.PropertyField(spatialBlend);

            
            
            
            if (spatialBlend.floatValue > 0.01f)
            {
                EditorGUI.indentLevel++;
                
                
                if (spatialBlend.floatValue < 1.0f)
                {
                    EditorGUILayout.HelpBox("Sound is partially 2D and 3D.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(rolloffMode);
                EditorGUILayout.PropertyField(minDistance);
                EditorGUILayout.PropertyField(maxDistance);
                
                EditorGUI.indentLevel--;
            }
            else
            {
                
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Sound is strictly 2D. Distance settings are ignored.", MessageType.None);
                EditorGUI.indentLevel--;
            }

            
            serializedObject.ApplyModifiedProperties();
        }
    }
}