using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Harmony
{
    /// <summary>
    /// Extension methods for ScriptableObjects.
    /// </summary>
    public static class ScriptableObjectExtensions
    {
        private static GlobalRoutinesHolder routinesHolder;

        private static GlobalRoutinesHolder RoutinesHolder
        {
            get
            {
                if (routinesHolder == null)
                {
                    var routineHelperGameObject = new GameObject("GlobalRoutinesHolder");
                    routinesHolder = routineHelperGameObject.AddComponent<GlobalRoutinesHolder>();

                    SceneManager.MoveGameObjectToScene(routineHelperGameObject, SceneManager.GetSceneAt(0));
                }

                return routinesHolder;
            }
        }

        /// <summary>
        /// Start a coroutine.
        /// </summary>
        /// <param name="obj">ScriptableObject.</param>
        /// <param name="routine">Coroutine.</param>
        /// <returns>Coroutine handle.</returns>
        public static Coroutine StartCoroutine(this ScriptableObject obj, IEnumerator routine)
        {
            return RoutinesHolder.StartCoroutine(routine);
        }

        /// <summary>
        /// Stop a coroutine.
        /// </summary>
        /// <param name="obj">ScriptableObject.</param>
        /// <param name="routine">Coroutine.</param>
        public static void StopCoroutine(this ScriptableObject obj, Coroutine routine)
        {
            RoutinesHolder.StopCoroutine(routine);
        }

        /// <summary>
        /// Stop all coroutines.
        /// </summary>
        /// <param name="obj">ScriptableObject.</param>
        public static void StopAllCoroutines(this ScriptableObject obj)
        {
            RoutinesHolder.StopAllCoroutines();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag.
        /// </summary>
        /// <param name="obj">ScriptableObject to search in. Ignored.</param>
        /// <param name="tag">Tag to search.</param>
        /// <typeparam name="T">Type of the component to find.</typeparam>
        /// <returns>
        /// Component found, or null if the GameObject doesn't have the asked component,
        /// or null if no GameObject with this tag exists.
        /// </returns>
        public static T GetComponentInTaggedObject<T>(this ScriptableObject obj, string tag) where T : class
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            if (taggedGameObject == null) return null;

            return taggedGameObject.GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag. Creates it if not found.
        /// </summary>
        /// <param name="obj">ScriptableObject to search in. Ignored.</param>
        /// <param name="tag">Tag to search.</param>
        /// <typeparam name="T">Type of the component to find.</typeparam>
        /// <returns>
        /// Component found, or null if the GameObject doesn't have the asked component,
        /// or null if no GameObject with this tag exists.
        /// </returns>
        public static T AddOrGetComponentInTaggedObject<T>(this ScriptableObject obj, string tag) where T : Component
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            if (taggedGameObject == null) return null;

            return taggedGameObject.AddOrGetComponent<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObjects with the provided tag.
        /// </summary>
        /// <param name="obj">ScriptableObject to search in. Ignored.</param>
        /// <param name="tag">Tag to search.</param>
        /// <typeparam name="T">Type of the component to find.</typeparam>
        /// <returns>
        /// Components found.
        /// </returns>
        public static T[] GetComponentsInTaggedObject<T>(this ScriptableObject obj, string tag) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var taggedGameObject in GameObject.FindGameObjectsWithTag(tag))
            {
                foreach (var componentToGet in taggedGameObject.GetComponents<T>())
                {
                    components.AddLast(componentToGet);
                }
            }

            return components.ToArray();
        }

        /// <summary>
        /// Returns the asked components in the first GameObject with the provided tag.
        /// </summary>
        /// <param name="obj">ScriptableObject to search in. Ignored.</param>
        /// <param name="tag">Tag to search.</param>
        /// <typeparam name="T">Type of the component to find.</typeparam>
        /// <returns>Components found, an empty array, or null if no GameObject with this tag exists.</returns>
        public static T GetComponentInTaggedObjectChildrens<T>(this ScriptableObject obj, string tag) where T : class
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            if (taggedGameObject == null) return null;

            return taggedGameObject.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObject childrens with the provided tag.
        /// </summary>
        /// <param name="obj">ScriptableObject to search in. Ignored.</param>
        /// <param name="tag">Tag to search.</param>
        /// <typeparam name="T">Type of the component to find.</typeparam>
        /// <returns>Components found.</returns>
        public static T[] GetComponentsInTaggedObjectChildrens<T>(this ScriptableObject obj, string tag) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var taggedGameObject in GameObject.FindGameObjectsWithTag(tag))
            {
                foreach (var componentToGet in taggedGameObject.GetComponentsInChildren<T>())
                {
                    components.AddLast(componentToGet);
                }
            }

            return components.ToArray();
        }

        private class GlobalRoutinesHolder : MonoBehaviour
        {
            //Used as a dummy MonoBehaviour.
        }
    }
}