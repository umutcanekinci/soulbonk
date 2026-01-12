using UnityEngine;
using UnityEditor; // Editor kütüphanesi şart
using VectorViolet.Core.Audio; // SoundData'nın olduğu namespace

namespace VectorViolet.Editor
{
    // Bu etiketi koymazsak Unity bu editörü SoundData için kullanmaz
    [CustomEditor(typeof(SoundData))]
    public class SoundDataEditor : UnityEditor.Editor
    {
        // Property'leri tanımlıyoruz
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
            // ScriptableObject içindeki değişkenleri bulup bağlıyoruz
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
            // Unity'nin güncellemelerini al
            serializedObject.Update();

            // --- GENERAL SETTINGS ---
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(clip);
            EditorGUILayout.PropertyField(volume);
            EditorGUILayout.PropertyField(pitch);
            EditorGUILayout.PropertyField(loop);

            EditorGUILayout.Space(); // Boşluk bırak

            // --- RANDOMIZATION ---
            EditorGUILayout.LabelField("Randomization", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useRandomPitch);

            // EĞER Random Pitch seçili ise Range'i göster
            if (useRandomPitch.boolValue)
            {
                // Biraz içeri girintili yapalım ki hiyerarşi belli olsun
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(randomPitchRange);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // --- 3D SETTINGS ---
            EditorGUILayout.LabelField("3D Settings", EditorStyles.boldLabel);
            
            // Spatial Blend'i her zaman göster (Çünkü 2D/3D ayrımını bu yapıyor)
            EditorGUILayout.PropertyField(spatialBlend);

            // MANTIK BURADA: 
            // Eğer Spatial Blend 0'dan büyükse (yani az da olsa 3D etkisi varsa) ayarları aç.
            // Tamamen 2D (0) ise gizle.
            if (spatialBlend.floatValue > 0.01f)
            {
                EditorGUI.indentLevel++;
                
                // İsteğe bağlı: Yardımcı bir bilgi kutusu
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
                // Tamamen 2D ise ufak bir info gösterebilirsin (Opsiyonel)
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Sound is strictly 2D. Distance settings are ignored.", MessageType.None);
                EditorGUI.indentLevel--;
            }

            // Değişiklikleri kaydet
            serializedObject.ApplyModifiedProperties();
        }
    }
}