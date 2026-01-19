using UnityEngine;

namespace VectorViolet.Core.Attributes
{
    public class SpritePreviewAttribute : PropertyAttribute
    {
        public float Height { get; private set; }

        public SpritePreviewAttribute(float height = 64)
        {
            Height = height;
        }
    }
}