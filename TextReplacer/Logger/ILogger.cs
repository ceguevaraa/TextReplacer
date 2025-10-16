namespace TextReplacer.Logger
{
    /// <summary>
    /// Simple interface for logging messages.
    /// </summary>
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);
    }
}
