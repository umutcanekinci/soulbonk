using UnityEngine;
using UnityEditor;

namespace VectorViolet.Core.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SpritePreviewAttribute))]
    public class SpritePreviewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float fieldHeight = EditorGUIUtility.singleLineHeight;
            Rect fieldRect = new Rect(position.x, position.y, position.width, fieldHeight);
            
            EditorGUI.PropertyField(fieldRect, property, label);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                Sprite sprite = property.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    SpritePreviewAttribute previewAttr = (SpritePreviewAttribute)attribute;
                    
                    Rect previewRect = new Rect(
                        position.x + EditorGUIUtility.labelWidth, 
                        position.y + fieldHeight + 2, 
                        previewAttr.Height, 
                        previewAttr.Height
                    );

                    Rect texCoords = new Rect(
                        sprite.rect.x / sprite.texture.width,
                        sprite.rect.y / sprite.texture.height,
                        sprite.rect.width / sprite.texture.width,
                        sprite.rect.height / sprite.texture.height
                    );

                    GUI.DrawTextureWithTexCoords(previewRect, sprite.texture, texCoords);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = base.GetPropertyHeight(property, label);
            
            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            {
                SpritePreviewAttribute previewAttr = (SpritePreviewAttribute)attribute;
                height += previewAttr.Height + 4; 
            }
            
            return height;
        }
    }
}