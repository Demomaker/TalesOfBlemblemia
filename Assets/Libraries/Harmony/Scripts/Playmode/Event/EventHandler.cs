﻿namespace Harmony
{
    /// <summary>
    /// EventHandler.
    /// </summary>
    public delegate void EventHandler();

    /// <summary>
    /// EventHandler.
    /// </summary>
    public delegate void EventHandler<in T>(T eventParam);

    /// <summary>
    /// EventHandler.
    /// </summary>
    public delegate void EventHandler<in T, in U>(T eventParam1, U eventParam2);
}