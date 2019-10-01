using System.Collections.Generic;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for the messaging system.
    /// </summary>
    public static class Messaging
    {
        /// <summary>
        /// Sends message to the first receiver found on the GameObject.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessage<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.GetComponent<T>().SendMessage(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessage<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.GetComponent<T>().SendMessage(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAll<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.GetComponents<T>().SendMessageToAll(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAll<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.GetComponents<T>().SendMessageToAll(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject or one of his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToChild<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.GetComponentInChildren<T>().SendMessage(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject or one of his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToChild<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.GetComponentInChildren<T>().SendMessage(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject or one of his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAllChildren<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.GetComponentsInChildren<T>().SendMessageToAll(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject or one of his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAllChildren<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.GetComponentsInChildren<T>().SendMessageToAll(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject root.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToRoot<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessage(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject root.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToRoot<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessage(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject root.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAllOnRoot<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessageToAll(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject root.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAllOnRoot<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessageToAll(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject root or his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToRootChild<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessageToChild(handler);
        }

        /// <summary>
        /// Sends message to the first receiver found on the GameObject root or his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToRootChild<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessageToChild(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject root or his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAllRootChildren<T>(this GameObject target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessageToAllChildren(handler);
        }

        /// <summary>
        /// Sends message to every receiver found on the GameObject root or his children.
        /// </summary>
        /// <param name="target">Target.</param>
        /// <param name="handler">Callback. Use this to send the message.</param>
        /// <returns>Whether the message has been received.</returns>
        public static bool SendMessageToAllRootChildren<T>(this Component target, MessagingHandler<T> handler)
        {
            return target.Root().SendMessageToAllChildren(handler);
        }
        
        private static bool SendMessage<T>(this T receiver, MessagingHandler<T> handler)
        {
            if (receiver != null)
            {
                handler(receiver);
                return true;
            }

            return false;
        }
        
        private static bool SendMessageToAll<T>(this IReadOnlyCollection<T> receivers, MessagingHandler<T> handler)
        {
            foreach (var receiver in receivers) handler(receiver);
            return receivers.Count > 0;
        }
    }
}