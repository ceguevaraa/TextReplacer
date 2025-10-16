namespace TextReplacer.Logger
{
    /// <summary>
    /// Console logger implementation.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
