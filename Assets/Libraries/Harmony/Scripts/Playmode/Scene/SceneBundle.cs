using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Harmony
{
    /// <summary>
    /// Scene Bundle. Useful to load a bunch of scenes at once. 
    /// </summary>
    [CreateAssetMenu(fileName = "New Scene Bundle", menuName = "Game/Scene Bundle")]
    public class SceneBundle : ScriptableObject
    {
        [SerializeField] private List<SceneReference> scenes = new List<SceneReference>();
        [SerializeField] private bool setFirstAsActive = false;

        /// <summary>
        /// Scenes. First scene in this list is the active scene.
        /// </summary>
        public IReadOnlyList<SceneReference> Scenes => scenes;

        /// <summary>
        /// When loading this bundle, should the first scene be marked as active ?
        /// </summary>
        public bool SetFirstAsActive => setFirstAsActive;

        /// <summary>
        /// Is this Scene Bundle loaded ?
        /// </summary>
        public bool IsLoaded => scenes.All(it => SceneManager.GetSceneByName(it.Name).isLoaded);
    }
}