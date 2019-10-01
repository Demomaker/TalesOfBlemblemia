using UnityEngine;

namespace Harmony
{
    /// <summary>
    /// Extension methods for Renderers.
    /// </summary>
    public static class RendererExtensions
    {
        /// <summary>
        /// Register to the OnBecameVisible event of the Renderer.
        /// </summary>
        public static void AddOnBecameVisible(this Renderer renderer, EventHandler onBecameVisible)
        {
            renderer.gameObject.AddOrGetComponent<RendererEvents>().OnBecameVisibleEvent += onBecameVisible;
        }
        
        /// <summary>
        /// Unregister to the OnBecameVisible event of the Renderer.
        /// </summary>
        public static void RemoveOnBecameVisible(this Renderer renderer, EventHandler onBecameVisible)
        {
            renderer.gameObject.AddOrGetComponent<RendererEvents>().OnBecameVisibleEvent -= onBecameVisible;
        }

        /// <summary>
        /// Register to the OnBecameInvisible event of the Renderer.
        /// </summary>
        public static void AddOnBecameInvisible(this Renderer renderer, EventHandler onBecameInvisible)
        {
            renderer.gameObject.AddOrGetComponent<RendererEvents>().OnBecameInvisibleEvent += onBecameInvisible;
        }
        
        /// <summary>
        /// Unregister to the OnBecameInvisible event of the Renderer.
        /// </summary>
        public static void RemoveOnBecameInvisible(this Renderer renderer, EventHandler onBecameInvisible)
        {
            renderer.gameObject.AddOrGetComponent<RendererEvents>().OnBecameInvisibleEvent -= onBecameInvisible;
        }

        private class RendererEvents : MonoBehaviour
        {
            public event EventHandler OnBecameVisibleEvent;
            public event EventHandler OnBecameInvisibleEvent;

            private void OnBecameVisible()
            {
                OnBecameVisibleEvent?.Invoke();
            }

            private void OnBecameInvisible()
            {
                OnBecameInvisibleEvent?.Invoke();
            }
        }
    }
}