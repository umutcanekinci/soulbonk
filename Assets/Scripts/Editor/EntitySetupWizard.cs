using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class EntitySetupWizard : EditorWindow
{
    private string entityName = "NewEntity";
    private AnimatorController templateController;
    private GameObject parentPrefab; 
    private bool shouldSplitSprites = true;
    private Vector2 globalPivot = new Vector2(0.5f, 0.5f); 
    private SpriteSheetConfig idleConfig = new SpriteSheetConfig() { typeName = "Idle" };
    private SpriteSheetConfig runConfig = new SpriteSheetConfig() { typeName = "Run" };
    private SpriteSheetConfig attack1Config = new SpriteSheetConfig() { typeName = "Attack1" };
    private SpriteSheetConfig attack2Config = new SpriteSheetConfig() { typeName = "Attack2" };
    
    private string basePrefabPath = "Assets/Prefabs/Entities";
    private string baseAnimPath = "Assets/Animation/Entities";

    private Sprite previewSprite;

    [MenuItem("VectorViolet Tools/Entity Auto-Setup")]
    public static void ShowWindow()
    {
        GetWindow<EntitySetupWizard>("Entity Wizard");
    }

    private void OnGUI()
    {
        GUILayout.Label("Entity Setup Wizard", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        entityName = EditorGUILayout.TextField("Entity Name", entityName);
        templateController = (AnimatorController)EditorGUILayout.ObjectField("Template Controller", templateController, typeof(AnimatorController), false);
        
        parentPrefab = (GameObject)EditorGUILayout.ObjectField("Parent Prefab (Optional)", parentPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        GUILayout.Label("Global Settings", EditorStyles.boldLabel);
        
        shouldSplitSprites = EditorGUILayout.Toggle("Split / Slice Sprites", shouldSplitSprites);

        if (shouldSplitSprites)
        {
            globalPivot = EditorGUILayout.Vector2Field("Global Pivot (0.5, 0.5 = Center)", globalPivot);
        }
        else
        {
            EditorGUILayout.HelpBox("Slicing disabled. Tool will use existing sprites for animation.", MessageType.Info);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Sprite Sheets", EditorStyles.boldLabel);
        
        DrawConfigField(idleConfig);
        DrawConfigField(runConfig);
        DrawConfigField(attack1Config);
        DrawConfigField(attack2Config);

        EditorGUILayout.Space(20);

        if (GUILayout.Button("CREATE ENTITY", GUILayout.Height(40)))
        {
            if (string.IsNullOrEmpty(entityName) || templateController == null)
            {
                EditorUtility.DisplayDialog("Error", "Entity Name ve Template Controller zorunludur!", "OK");
                return;
            }
            CreateEntity();
        }
    }

    private void DrawConfigField(SpriteSheetConfig config)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label(config.typeName + " Configuration", EditorStyles.miniBoldLabel);
        
        config.texture = (Texture2D)EditorGUILayout.ObjectField("Texture", config.texture, typeof(Texture2D), false);
        
        if (config.texture != null)
        {
            EditorGUILayout.BeginHorizontal();
            config.rows = EditorGUILayout.IntField("Rows (Dir)", config.rows);
            config.cols = EditorGUILayout.IntField("Cols (Frames)", config.cols);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }

    private void CreateEntity()
    {
        previewSprite = null;

        if (!Directory.Exists(baseAnimPath)) Directory.CreateDirectory(baseAnimPath);
        string entityAnimFolder = $"{baseAnimPath}/{entityName}";
        if (!Directory.Exists(entityAnimFolder)) Directory.CreateDirectory(entityAnimFolder);
        
        string idlePath = $"{entityAnimFolder}/Idle";
        string runPath = $"{entityAnimFolder}/Run";
        string attack1Path = $"{entityAnimFolder}/Attack1";
        string attack2Path = $"{entityAnimFolder}/Attack2";

        if (!Directory.Exists(idlePath)) Directory.CreateDirectory(idlePath);
        if (!Directory.Exists(runPath)) Directory.CreateDirectory(runPath);
        if (attack1Config.texture != null && !Directory.Exists(attack1Path)) Directory.CreateDirectory(attack1Path);
        if (attack2Config.texture != null && !Directory.Exists(attack2Path)) Directory.CreateDirectory(attack2Path);
        
        AssetDatabase.Refresh();
        
        Dictionary<string, Motion> createdClips = new Dictionary<string, Motion>();

        ProcessSpriteSheet(idleConfig, idlePath, createdClips);
        ProcessSpriteSheet(runConfig, runPath, createdClips);
        if (attack1Config.texture != null) ProcessSpriteSheet(attack1Config, attack1Path, createdClips);
        if (attack2Config.texture != null) ProcessSpriteSheet(attack2Config, attack2Path, createdClips);

        string controllerPath = $"{entityAnimFolder}/{entityName}_Controller.controller";
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateController), controllerPath);
        AnimatorController newController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

        UpdateBlendTree(newController, "Idle", createdClips, "Idle");
        UpdateBlendTree(newController, "Movement", createdClips, "Run");
        
        if (attack1Config.texture != null) UpdateBlendTree(newController, "Attack1", createdClips, "Attack1");
        if (attack2Config.texture != null) UpdateBlendTree(newController, "Attack2", createdClips, "Attack2");

        CreatePrefab(newController);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
 
    private void ProcessSpriteSheet(SpriteSheetConfig config, string savePath, Dictionary<string, Motion> clipRegistry)
    {
        if (config.texture == null) return;

        string path = AssetDatabase.GetAssetPath(config.texture);
        
        if (shouldSplitSprites)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            List<SpriteMetaData> metas = new List<SpriteMetaData>();
            int cellWidth = config.texture.width / config.cols;
            int cellHeight = config.texture.height / config.rows;
            
            string[] directions = { "Down", "Left", "Right", "Up" }; 

            for (int r = 0; r < config.rows; r++) 
            {
                string dirName = (r < directions.Length) ? directions[r] : "Dir" + r;
                for (int c = 0; c < config.cols; c++) 
                {
                    SpriteMetaData meta = new SpriteMetaData();
                    meta.name = $"{entityName}_{config.typeName}_{dirName}_{c}";
                    float yPos = config.texture.height - ((r + 1) * cellHeight);
                    
                    meta.rect = new Rect(c * cellWidth, yPos, cellWidth, cellHeight);
                    meta.alignment = 9; 
                    meta.pivot = globalPivot; 
                    
                    metas.Add(meta);
                }
            }
            importer.spritesheet = metas.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> slicedSprites = new List<Sprite>();
        foreach (var asset in assets) if (asset is Sprite) slicedSprites.Add(asset as Sprite);

        slicedSprites = slicedSprites.OrderByDescending(s => Mathf.RoundToInt(s.rect.y)) 
                                     .ThenBy(s => Mathf.RoundToInt(s.rect.x))
                                     .ToList();

        if (config.typeName == "Idle" && slicedSprites.Count > 0)
        {
            previewSprite = slicedSprites[0];
        }

        string[] dirs = { "Down", "Left", "Right", "Up" }; 

        for (int r = 0; r < config.rows; r++)
        {
            string dirName = (r < dirs.Length) ? dirs[r] : "Dir" + r;
            
            List<Sprite> directionSprites = new List<Sprite>();

            for (int c = 0; c < config.cols; c++)
            {
                int spriteIndex = (r * config.cols) + c;
                if (spriteIndex < slicedSprites.Count)
                {
                    directionSprites.Add(slicedSprites[spriteIndex]);
                }
            }

            if (directionSprites.Count == 0) continue;

            AnimationClip clip = new AnimationClip();
            clip.frameRate = 12; 
            
            bool isLooping = config.typeName != "Attack1" && config.typeName != "Attack2";
            if (isLooping)
            {
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
            }

            EditorCurveBinding binding = new EditorCurveBinding();
            binding.type = typeof(SpriteRenderer);
            binding.path = ""; 
            binding.propertyName = "m_Sprite";

            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[directionSprites.Count];
            for (int i = 0; i < directionSprites.Count; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].time = i * (1f / 12f);
                keyframes[i].value = directionSprites[i];
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
            
            string clipPath = $"{savePath}/{entityName}_{config.typeName}_{dirName}.anim";
            AssetDatabase.CreateAsset(clip, clipPath);
            
            clipRegistry.Add($"{config.typeName}_{dirName}", clip);
        }
    }
    
    private void UpdateBlendTree(AnimatorController controller, string stateName, Dictionary<string, Motion> clips, string clipPrefix)
    {
        var layer = controller.layers[0];
        var stateMachine = layer.stateMachine;
        
        var state = stateMachine.states.FirstOrDefault(s => s.state.name == stateName).state;
        if (state == null) return;
    
        if (state.motion is BlendTree blendTree)
        {
            var children = blendTree.children;
            for (int i = 0; i < children.Length; i++)
            {
                Vector2 pos = children[i].position;
                string dirSuffix = "";

                if (IsClose(pos, new Vector2(0, -1))) dirSuffix = "Down";
                else if (IsClose(pos, new Vector2(1, 0))) dirSuffix = "Right";
                else if (IsClose(pos, new Vector2(-1, 0))) dirSuffix = "Left"; 
                else if (IsClose(pos, new Vector2(0, 1))) dirSuffix = "Up";

                if (dirSuffix != "")
                {
                    string key = $"{clipPrefix}_{dirSuffix}";
                    if (clips.ContainsKey(key))
                    {
                        children[i].motion = clips[key];
                    }
                }
            }
            blendTree.children = children;
        }
    }
    
    private bool IsClose(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b) < 0.1f;
    }

    private void CreatePrefab(AnimatorController controller)
    {
        GameObject root;
        GameObject spriteObj;
        if (parentPrefab != null)
        {
            root = (GameObject)PrefabUtility.InstantiatePrefab(parentPrefab);
            root.name = entityName;

            Transform artT = root.transform.Find("Art");
            if (artT == null)
            {
                GameObject art = new GameObject("Art");
                artT = art.transform;
                artT.SetParent(root.transform);
                artT.localPosition = Vector3.zero;
            }

            Transform spriteT = artT.Find("Sprite");
            if (spriteT == null)
            {
                spriteObj = new GameObject("Sprite");
                spriteObj.transform.SetParent(artT);
                spriteObj.transform.localPosition = Vector3.zero;
            }
            else
            {
                spriteObj = spriteT.gameObject;
            }
        }
        else
        {
            root = new GameObject(entityName);
            GameObject art = new GameObject("Art");
            spriteObj = new GameObject("Sprite");

            art.transform.SetParent(root.transform);
            spriteObj.transform.SetParent(art.transform);
            
            art.transform.localPosition = Vector3.zero;
            spriteObj.transform.localPosition = Vector3.zero;
        }

        SpriteRenderer sr = spriteObj.GetComponent<SpriteRenderer>();
        if (sr == null) sr = spriteObj.AddComponent<SpriteRenderer>();
        
        if (previewSprite != null) sr.sprite = previewSprite;

        Animator anim = spriteObj.GetComponent<Animator>();
        if (anim == null) anim = spriteObj.AddComponent<Animator>();
        
        anim.runtimeAnimatorController = controller;
        
        if (!Directory.Exists(basePrefabPath)) Directory.CreateDirectory(basePrefabPath);

        PrefabUtility.SaveAsPrefabAsset(root, $"{basePrefabPath}/{entityName}.prefab");
        
        DestroyImmediate(root); 
    }
}

public class SpriteSheetConfig
{
    public string typeName;
    public Texture2D texture;
    public int rows = 4;
    public int cols = 6;
}