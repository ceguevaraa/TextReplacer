using TextReplacer.Models;

namespace TextReplacer.Services
{
    /// <summary>
    /// Abstraction for text replacement operations.
    /// </summary>
    public interface ITextReplacerService
    {
        ReplacementResult Replace(string input, string searchText, string replacementText);
    }
}
