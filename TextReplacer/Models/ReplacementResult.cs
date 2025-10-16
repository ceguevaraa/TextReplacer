namespace TextReplacer.Models
{
    /// <summary>
    /// Encapsulates the result of a replacement operation.
    /// </summary>
    public class ReplacementResult
    {
        public string ResultText { get; }
        public int ReplacementCount { get; }

        public ReplacementResult(string resultText, int replacementCount)
        {
            ResultText = resultText;
            ReplacementCount = replacementCount;
        }
    }
}
