using UnityEngine;
using UnityEditor;
using VectorViolet.Core.Stats;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VectorViolet.Editor
{
    [CustomEditor(typeof(StatHolder))]
    public class StatHolderEditor : UnityEditor.Editor
    {
        private StatHolder _target;
        private List<StatDefinition> _allStats;
        
        private HashSet<string> _requiredStats = new HashSet<string>();

        private void OnEnable()
        {
            _target = (StatHolder)target;
            
            if (!EditorUtility.IsPersistent(_target))
            {
                _allStats = FindAllStatDefinitions();
                CheckRequirements(); 
            }
        }

        private void CheckRequirements()
        {
            if (_target == null) return;
            
            _requiredStats.Clear();

            MonoBehaviour[] scripts = _target.GetComponents<MonoBehaviour>();

            foreach (var script in scripts)
            {
                if (script == null) continue;

                System.Type type = script.GetType();
                var attributes = type.GetCustomAttributes(typeof(RequireStatAttribute), true);

                foreach (RequireStatAttribute attr in attributes)
                {
                    if (attr.StatNames != null)
                    {
                        foreach (string name in attr.StatNames)
                        {
                            _requiredStats.Add(name);
                        }
                    }
                }
            }

            bool changed = false;
            
            if (_allStats == null || _allStats.Count == 0)
                _allStats = FindAllStatDefinitions();

            foreach (var requiredName in _requiredStats)
            {
                var def = _allStats.FirstOrDefault(x => 
                    x.DisplayName.Equals(requiredName, System.StringComparison.OrdinalIgnoreCase) || 
                    x.name.Equals(requiredName, System.StringComparison.OrdinalIgnoreCase)
                );

                if (def != null)
                {
                    bool exists = false;
                    if (def.type == StatType.Attribute)
                        exists = _target.attributes.Any(x => x.definition == def);
                    else
                        exists = _target.resources.Any(x => x.definition == def);

                    if (!exists)
                    {
                        AddStat(def);
                        changed = true;
                    }
                }
                else
                {
                    Debug.LogWarning($"<color=yellow>[RequireStat]</color> '{requiredName}' named stat not found!\n" +
                                     "Please check the StatDefinition file name or DisplayName.");
                }
            }

            if (changed)
            {
                serializedObject.Update();
                EditorUtility.SetDirty(_target);
            }
        }

        public override void OnInspectorGUI()
        {
            if (EditorUtility.IsPersistent(_target))
            {
                EditorGUILayout.HelpBox("PREFAB MODE (Asset)\n\nStats cannot be edited on the Prefab Asset directly.", MessageType.Warning);
                return; 
            }

            serializedObject.Update();

            if (GUILayout.Button("Refresh Requirements & Stats", GUILayout.Height(30)))
            {
                _allStats = FindAllStatDefinitions(); 
                CheckRequirements(); 
            }
            EditorGUILayout.Space();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(StatHolder), false);
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stat Configuration", EditorStyles.boldLabel);

            if (_allStats != null)
            {
                foreach (var statDef in _allStats)
                {
                    if (statDef != null) 
                        DrawStatRow(statDef);
                }
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);

            DrawPropertiesExcluding(serializedObject, "m_Script", "attributes", "resources");

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStatRow(StatDefinition def)
        {
            if (_target.attributes == null) _target.attributes = new List<AttributeStat>();
            if (_target.resources == null) _target.resources = new List<ResourceStat>();

            bool exists = false;
            if (def.type == StatType.Attribute)
                exists = _target.attributes.Any(x => x.definition == def);
            else
                exists = _target.resources.Any(x => x.definition == def);

            bool isRequired = _requiredStats.Contains(def.DisplayName) || _requiredStats.Contains(def.name);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            // --- DEĞİŞİKLİK BAŞLANGICI: Rect Sistemi ---
            // EditorGUILayout yerine Rect kullanarak alanı elle bölüyoruz.
            // Bu sayede pencere küçülse bile sağ taraf sabit kalır, sol taraf sıkışır.

            // 1. Tüm satırın alanını al
            Rect rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            // 2. Sağ taraftaki etiket için genişlik ayır (Örn: 90 piksel)
            float rightLabelWidth = 95f; 
            
            // 3. Sol ve Sağ alanları tanımla
            Rect leftRect = new Rect(rect.x, rect.y, rect.width - rightLabelWidth, rect.height);
            Rect rightRect = new Rect(rect.x + rect.width - rightLabelWidth, rect.y, rightLabelWidth, rect.height);

            // 4. SOL TARAFI ÇİZ (Toggle)
            if (isRequired) GUI.enabled = false;

            string label = $"{def.DisplayName} ({def.name})";
            bool toggled = EditorGUI.ToggleLeft(leftRect, label, exists); // ToggleLeft artık Rect alıyor

            if (isRequired) GUI.enabled = true;

            // 5. SAĞ TARAFI ÇİZ (Renkli Etiket)
            GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
            labelStyle.alignment = TextAnchor.MiddleRight;
            labelStyle.normal.textColor = def.type == StatType.Attribute ? Color.cyan : Color.green;

            string typeLabel = def.type.ToString();
            if (isRequired) typeLabel += " (Req)";

            EditorGUI.LabelField(rightRect, typeLabel, labelStyle);
            
            // --- DEĞİŞİKLİK BİTİŞİ ---

            if (toggled)
            {
                if (!exists)
                {
                    AddStat(def);
                    serializedObject.Update(); 
                }
                
                DrawStatValues(def);
            }
            else if (exists)
            {
                RemoveStat(def);
                serializedObject.Update();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawStatValues(StatDefinition def)
        {
             EditorGUI.indentLevel++;

            if (def.type == StatType.Attribute)
            {
                SerializedProperty list = serializedObject.FindProperty("attributes"); 
                if (list == null) return;

                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty element = list.GetArrayElementAtIndex(i);
                    SerializedProperty defProp = element.FindPropertyRelative("definition");
                    
                    if (defProp.objectReferenceValue == def)
                    {
                        SerializedProperty baseVal = element.FindPropertyRelative("_baseValue");
                        EditorGUILayout.PropertyField(baseVal, new GUIContent("Base Value"));
                        break;
                    }
                }
            }
            else
            {
                SerializedProperty list = serializedObject.FindProperty("resources");
                if (list != null)
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        SerializedProperty element = list.GetArrayElementAtIndex(i);
                        SerializedProperty defProp = element.FindPropertyRelative("definition");

                        if (defProp.objectReferenceValue == def)
                        {
                            SerializedProperty maxVal = element.FindPropertyRelative("MaxValue");
                            SerializedProperty currVal = element.FindPropertyRelative("_currentValue");
                            
                            EditorGUILayout.PropertyField(maxVal, new GUIContent("Max Capacity"));
                            EditorGUILayout.PropertyField(currVal, new GUIContent("Initial Value"));
                            break;
                        }
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        private void AddStat(StatDefinition def)
        {
            if (def.type == StatType.Attribute)
            {
                var newAttr = new AttributeStat();
                newAttr.definition = def;
                _target.attributes.Add(newAttr);
            }
            else
            {
                var newRes = new ResourceStat();
                newRes.definition = def;
                newRes.MaxValue = 100;
                newRes.CurrentValue = 100;
                _target.resources.Add(newRes);
            }
        }

        private void RemoveStat(StatDefinition def)
        {
            if (def.type == StatType.Attribute)
            {
                _target.attributes.RemoveAll(x => x.definition == def);
            }
            else
            {
                _target.resources.RemoveAll(x => x.definition == def);
            }
        }

        private List<StatDefinition> FindAllStatDefinitions()
        {
            string[] guids = AssetDatabase.FindAssets("t:StatDefinition");
            List<StatDefinition> stats = new List<StatDefinition>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                StatDefinition asset = AssetDatabase.LoadAssetAtPath<StatDefinition>(path);
                if (asset != null) stats.Add(asset);
            }
            return stats.OrderBy(x => x.type).ThenBy(x => x.DisplayName).ToList();
        }
    }
}