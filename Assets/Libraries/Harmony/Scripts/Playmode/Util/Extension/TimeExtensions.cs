﻿namespace Harmony
{
    /// <summary>
    /// Extension methods for Time.
    /// </summary>
    public static class TimeExtensions
    {
        private static float previousTimeScale = 1;

        /// <summary>
        /// Resume Time.
        /// </summary>
        public static void Resume()
        {
            UnityEngine.Time.timeScale = previousTimeScale;
        }

        /// <summary>
        /// Pause time.
        /// </summary>
        public static void Pause()
        {
            previousTimeScale = UnityEngine.Time.timeScale;
            UnityEngine.Time.timeScale = 0;
        }

        /// <summary>
        /// Is Time running.
        /// </summary>
        public static bool IsRunning()
        {
            return UnityEngine.Time.timeScale > 0;
        }

        /// <summary>
        /// Is Time paused.
        /// </summary>
        public static bool IsPaused()
        {
            return !IsRunning();
        }
    }
}