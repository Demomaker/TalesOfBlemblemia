﻿using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Event Channel.
    /// </summary>
    public abstract class EventChannel : MonoBehaviour
    {
        /// <summary>
        /// Publish an event on this channel.
        /// </summary>
        public abstract void Publish();
    }

    /// <summary>
    /// EventChannel.
    /// </summary>
    public abstract class EventChannel<T> : MonoBehaviour
    {
        /// <summary>
        /// Publish an event on this channel.
        /// </summary>
        public abstract void Publish(T eventParam);
    }

    /// <summary>
    /// EventChannel.
    /// </summary>
    public abstract class EventChannel<T, U> : MonoBehaviour
    {
        /// <summary>
        /// Publish an event on this channel.
        /// </summary>
        public abstract void Publish(T eventParam1, U eventParam2);
    }
}