using UnityEngine;

namespace VectorViolet.Core.Stats
{
    [CreateAssetMenu(menuName = "Pro Stat Manager/Stat Category")]
    public class StatCategory : ScriptableObject
    {
        [TextArea] public string description;
    }
}