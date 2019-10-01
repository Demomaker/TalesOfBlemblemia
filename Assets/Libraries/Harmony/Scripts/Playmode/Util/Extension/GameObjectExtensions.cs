using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for GameObjects.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Does this object still exists ?
        /// </summary>
        public static bool Exists(this Object obj)
        {
            return obj != null;
        }

        /// <summary>
        /// Is this object destroyed ?
        /// </summary>
        public static bool DoesntExists(this Object obj)
        {
            return !obj.Exists();
        }

        /// <summary>
        /// Return the topmost parent of this GameObject.
        /// </summary>
        public static GameObject Root(this GameObject gameObject)
        {
            return gameObject.transform.root.gameObject;
        }

        /// <summary>
        /// Return the parent of this GameObject.
        /// </summary>
        public static GameObject Parent(this GameObject gameObject)
        {
            var parentTransform = gameObject.transform.parent;
            return parentTransform == null ? null : parentTransform.gameObject;
        }

        /// <summary>
        /// Return the children of this GameObject.
        /// </summary>
        public static GameObject[] Children(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var childCount = transform.childCount;

            var children = new GameObject[childCount];
            for (var i = 0; i < childCount; i++)
                children[i] = transform.GetChild(i).gameObject;

            return children;
        }

        /// <summary>
        /// Return the siblings of this GameObject.
        /// </summary>
        public static GameObject[] Siblings(this GameObject gameObject)
        {
            var parent = gameObject.Parent();
            return parent == null ? gameObject.scene.GetRootGameObjects() : parent.Children();
        }

        /// <summary>
        /// Returns the asked component in the GameObject, or create it if it doesn't exists.
        /// </summary>
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null) component = gameObject.AddComponent<T>();
            return component;
        }

        /// <summary>
        /// Returns the asked component in the GameObject.
        /// </summary>
        public static T GetComponentInObject<T>(this GameObject gameObject) where T : class
        {
            return gameObject.GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObject.
        /// </summary>
        public static T[] GetComponentsInObject<T>(this GameObject gameObject) where T : class
        {
            return gameObject.GetComponents<T>();
        }

        /// <summary>
        /// Returns the asked component in the GameObject root.
        /// </summary>
        public static T GetComponentInRoot<T>(this GameObject gameObject) where T : class
        {
            return gameObject.Root().GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObject root.
        /// </summary>
        public static T[] GetComponentsInRoot<T>(this GameObject gameObject) where T : class
        {
            return gameObject.Root().GetComponents<T>();
        }

        /// <summary>
        /// Returns the asked component in the GameObject root and his children.
        /// </summary>
        public static T GetComponentInRootChildren<T>(this GameObject gameObject) where T : class
        {
            return gameObject.Root().GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObject root and his children.
        /// </summary>
        public static T[] GetComponentsInRootChildren<T>(this GameObject gameObject) where T : class
        {
            return gameObject.Root().GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Returns the asked component in the GameObject or his siblings.
        /// </summary>
        public static T GetComponentInSiblings<T>(this GameObject gameObject) where T : class
        {
            foreach (var sibling in gameObject.Siblings())
            {
                var componentToGet = sibling.GetComponent<T>();
                if (componentToGet != null) return componentToGet;
            }

            return null;
        }

        /// <summary>
        /// Returns the asked components in the GameObject or his siblings.
        /// </summary>
        public static T[] GetComponentsInSiblings<T>(this GameObject gameObject) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var sibling in gameObject.Siblings())
            foreach (var componentToGet in sibling.GetComponents<T>())
                components.AddLast(componentToGet);

            return components.ToArray();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag.
        /// </summary>
        public static T GetComponentInTaggedObject<T>(string tag) where T : class
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            return taggedGameObject == null ? null : taggedGameObject.GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag.
        /// </summary>
        public static T GetComponentInTaggedObject<T>(this GameObject gameObject, string tag) where T : class
        {
            return GetComponentInTaggedObject<T>(tag);
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag. Creates it if not found.
        /// </summary>
        public static T AddOrGetComponentInTaggedObject<T>(string tag) where T : Component
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            return taggedGameObject == null ? null : taggedGameObject.AddOrGetComponent<T>();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag. Creates it if not found.
        /// </summary>
        public static T AddOrGetComponentInTaggedObject<T>(this GameObject gameObject, string tag) where T : Component
        {
            return AddOrGetComponentInTaggedObject<T>(tag);
        }

        /// <summary>
        /// Returns the asked components in the GameObjects with the provided tag.
        /// </summary>
        public static T[] GetComponentsInTaggedObject<T>(string tag) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var taggedGameObject in GameObject.FindGameObjectsWithTag(tag))
            foreach (var componentToGet in taggedGameObject.GetComponents<T>())
                components.AddLast(componentToGet);

            return components.ToArray();
        }

        /// <summary>
        /// Returns the asked components in the GameObjects with the provided tag.
        /// </summary>
        public static T[] GetComponentsInTaggedObject<T>(this GameObject gameObject, string tag) where T : class
        {
            return GetComponentsInTaggedObject<T>(tag);
        }

        /// <summary>
        /// Returns the asked components in the first GameObject with the provided tag.
        /// </summary>
        public static T GetComponentInTaggedObjectChildren<T>(string tag) where T : class
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            return taggedGameObject == null ? null : taggedGameObject.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the asked components in the first GameObject with the provided tag.
        /// </summary>
        public static T GetComponentInTaggedObjectChildren<T>(this GameObject gameObject, string tag) where T : class
        {
            return GetComponentInTaggedObjectChildren<T>(tag);
        }

        /// <summary>
        /// Returns the asked components in the GameObject children with the provided tag.
        /// </summary>
        public static T[] GetComponentsInTaggedObjectChildren<T>(string tag) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var taggedGameObject in GameObject.FindGameObjectsWithTag(tag))
            foreach (var componentToGet in taggedGameObject.GetComponentsInChildren<T>())
                components.AddLast(componentToGet);

            return components.ToArray();
        }

        /// <summary>
        /// Returns the asked components in the GameObject children with the provided tag.
        /// </summary>
        public static T[] GetComponentsInTaggedObjectChildren<T>(this GameObject gameObject, string tag) where T : class
        {
            return GetComponentsInTaggedObjectChildren<T>(tag);
        }
    }
}