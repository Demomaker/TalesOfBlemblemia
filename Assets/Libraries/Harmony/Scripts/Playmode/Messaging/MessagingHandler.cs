namespace Harmony
{
    /// <summary>
    /// Callback called to send the message on the target.
    /// </summary>
    public delegate void MessagingHandler<in T>(T target);
}