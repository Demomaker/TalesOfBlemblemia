using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Components.
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Return the topmost parent GameObject of this component.
        /// </summary>
        public static GameObject Root(this Component component)
        {
            return component.gameObject.Root();
        }

        /// <summary>
        /// Return the parent GameObject of this component.
        /// </summary>
        public static GameObject Parent(this Component component)
        {
            return component.gameObject.Parent();
        }

        /// <summary>
        /// Return the children of this component's GameObject.
        /// </summary>
        public static GameObject[] Children(this Component component)
        {
            return component.gameObject.Children();
        }

        /// <summary>
        /// Return the siblings of this component's GameObject.
        /// </summary>
        public static GameObject[] Siblings(this Component component)
        {
            return component.gameObject.Siblings();
        }

        /// <summary>
        /// Returns the asked component in the component's GameObject, or create it if it doesn't exists.
        /// </summary>
        public static T AddOrGetComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.AddOrGetComponent<T>();
        }

        /// <summary>
        /// Returns the asked component in the component's GameObject.
        /// </summary>
        public static T GetComponentInObject<T>(this Component component) where T : class
        {
            return component.GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked components in the component's GameObject.
        /// </summary>
        public static T[] GetComponentsInObject<T>(this Component component) where T : class
        {
            return component.GetComponents<T>();
        }

        /// <summary>
        /// Returns the asked component in the component's GameObject root.
        /// </summary>
        public static T GetComponentInRoot<T>(this Component component) where T : class
        {
            return component.Root().GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked components in the component's GameObject root.
        /// </summary>
        public static T[] GetComponentsInRoot<T>(this Component component) where T : class
        {
            return component.Root().GetComponents<T>();
        }

        /// <summary>
        /// Returns the asked component in the component's GameObject root and his children.
        /// </summary>
        public static T GetComponentInRootChildren<T>(this Component component) where T : class
        {
            return component.Root().GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the asked components in the component's GameObject root and his children.
        /// </summary>
        public static T[] GetComponentsInRootChildren<T>(this Component component) where T : class
        {
            return component.Root().GetComponentsInChildren<T>();
        }

        /// <summary>
        /// Returns the asked component in the component's GameObject or his siblings.
        /// </summary>
        public static T GetComponentInSiblings<T>(this Component component) where T : class
        {
            foreach (var sibling in component.Siblings())
            {
                var componentToGet = sibling.GetComponent<T>();
                if (componentToGet != null) return componentToGet;
            }

            return null;
        }

        /// <summary>
        /// Returns the asked components in the component's GameObject or his siblings.
        /// </summary>
        public static T[] GetComponentsInSiblings<T>(this Component component) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var sibling in component.Siblings())
            foreach (var componentToGet in sibling.GetComponents<T>())
                components.AddLast(componentToGet);

            return components.ToArray();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag.
        /// </summary>
        public static T GetComponentInTaggedObject<T>(this Component component, string tag) where T : class
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            return taggedGameObject == null ? null : taggedGameObject.GetComponent<T>();
        }

        /// <summary>
        /// Returns the asked component in the first GameObject with the provided tag. Creates it if not found.
        /// </summary>
        public static T AddOrGetComponentInTaggedObject<T>(this Component component, string tag) where T : Component
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            return taggedGameObject == null ? null : taggedGameObject.AddOrGetComponent<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObjects with the provided tag.
        /// </summary>
        public static T[] GetComponentsInTaggedObject<T>(this Component component, string tag) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var taggedGameObject in GameObject.FindGameObjectsWithTag(tag))
            foreach (var componentToGet in taggedGameObject.GetComponents<T>())
                components.AddLast(componentToGet);

            return components.ToArray();
        }

        /// <summary>
        /// Returns the asked components in the first GameObject with the provided tag.
        /// </summary>
        public static T GetComponentInTaggedObjectChildren<T>(this Component component, string tag) where T : class
        {
            var taggedGameObject = GameObject.FindGameObjectWithTag(tag);
            return taggedGameObject == null ? null : taggedGameObject.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Returns the asked components in the GameObject children with the provided tag.
        /// </summary>
        public static T[] GetComponentsInTaggedObjectChildren<T>(this Component component, string tag) where T : class
        {
            var components = new LinkedList<T>();

            foreach (var taggedGameObject in GameObject.FindGameObjectsWithTag(tag))
            foreach (var componentToGet in taggedGameObject.GetComponentsInChildren<T>())
                components.AddLast(componentToGet);

            return components.ToArray();
        }
    }
}