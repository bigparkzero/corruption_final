using UnityEngine;

namespace CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Hit Reaction Data", menuName = "Scriptable Object/Hit Reaction Data")]
    public class SO_HitReactionData : ScriptableObject
    {
        public string clipName;
        public float eventTime;
        public HitReactionData hitReactionData;

        public void OnEnable()
        {

        }

        public SO_HitReactionData Clone()
        {
            SO_HitReactionData clone = CreateInstance<SO_HitReactionData>();
            clone.clipName = this.clipName;
            clone.eventTime = this.eventTime;
            clone.hitReactionData = this.hitReactionData;
            clone.name = $"{clone.name} (clone)";

            return clone;
        }
    }
}