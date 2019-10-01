using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Harmony
{
    /// <summary>
    /// Scene Reference. Used to put references to scenes in a Scriptable Object.
    /// </summary>
    [Serializable]
    public class SceneReference
    {
        [SerializeField] private Object sceneAsset;
        [SerializeField] private string sceneName = "";

#if UNITY_EDITOR
        public SceneAsset SceneAsset => sceneAsset as SceneAsset;
#endif

        /// <summary>
        /// Name.
        /// </summary>
        public string Name => sceneName;

        public static implicit operator string(SceneReference sceneField)
        {
            return sceneField.Name;
        }

        protected bool Equals(SceneReference other)
        {
            return string.Equals(sceneName, other.sceneName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SceneReference) obj);
        }

        public override int GetHashCode()
        {
            return sceneName != null ? sceneName.GetHashCode() : 0;
        }
    }
}