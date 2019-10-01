﻿using System.Collections.Generic;
using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Messaging group (like a mailing list).
    /// </summary>
    public abstract class MessagingGroup<T> : MonoBehaviour
    {
        private readonly HashSet<T> receivers = new HashSet<T>();
        private int nbIteratorLocks;
        private HashSet<T> receiversToAdd;
        private HashSet<T> receiversToRemove;

        /// <summary>
        /// Register as a receiver of this mailing list.
        /// </summary>
        public void Register(T receiver)
        {
            AddReceiver(receiver);
        }

        /// <summary>
        /// Unregister as a receiver of this mailing list.
        /// </summary>
        public void Unregister(T receiver)
        {
            RemoveReceiver(receiver);
        }

        /// <summary>
        /// Sends a message to the group.
        /// </summary>
        /// <returns>Whether the message has been received.</returns>
        public bool SendMessage(MessagingHandler<T> handler)
        {
            var hasReceivers = receivers.Count > 0; //Receivers can change while looping through it.
            foreach (var receiver in GetReceivers()) handler(receiver);
            return hasReceivers;
        }

        //Theses methods are needed to be able to iterate over the receivers while
        //adding or removing them. There's a caveat though : changes will only be applied
        //after all iterations are finished.

        private IEnumerable<T> GetReceivers()
        {
            if (nbIteratorLocks == 0)
            {
                receiversToAdd = new HashSet<T>();
                receiversToRemove = new HashSet<T>();
            }

            nbIteratorLocks++;
            foreach (var receiver in receivers) yield return receiver;
            nbIteratorLocks--;

            if (nbIteratorLocks == 0)
            {
                foreach (var receiver in receiversToRemove) receivers.Remove(receiver);
                foreach (var receiver in receiversToAdd) receivers.Add(receiver);
                receiversToAdd = null;
                receiversToRemove = null;
            }
        }

        private void AddReceiver(T receiver)
        {
            if (nbIteratorLocks > 0) receiversToAdd.Add(receiver);
            else receivers.Add(receiver);
        }

        private void RemoveReceiver(T receiver)
        {
            if (nbIteratorLocks > 0) receiversToRemove.Add(receiver);
            else receivers.Remove(receiver);
        }
    }
}