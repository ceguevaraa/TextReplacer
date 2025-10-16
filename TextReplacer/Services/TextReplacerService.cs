using TextReplacer.Models;

namespace TextReplacer.Services
{
    /// <summary>
    /// Default implementation using string.Replace.
    /// </summary>
    public class TextReplacerService : ITextReplacerService
    {
        public ReplacementResult Replace(string input, string searchText, string replacementText)
        {
            if (string.IsNullOrEmpty(searchText))
                throw new ArgumentException("El texto a buscar no puede estar vacío.");

            int count = 0;
            int index = 0;
            while ((index = input.IndexOf(searchText, index, StringComparison.Ordinal)) != -1)
            {
                count++;
                index += searchText.Length;
            }

            string result = input.Replace(searchText, replacementText);
            return new ReplacementResult(result, count);
        }
    }
}
