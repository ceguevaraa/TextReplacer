using TextReplacer.Logger;
using TextReplacer.Models;

namespace TextReplacer.Services
{
    /// <summary>
    /// Handles reading/writing files and orchestrating replacements.
    /// </summary>
    public class FileTextReplacer
    {
        private readonly ITextReplacerService _service;
        private readonly ILogger _logger;

        public FileTextReplacer(ITextReplacerService service, ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public ReplacementResult ReplaceInFile(AppArguments? args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            _logger.Info($"Leyendo: {args.SourcePath}");

            string content = File.ReadAllText(args.SourcePath);

            var result = _service
                .Replace(content, args.SearchText, args.ReplacementText);

            var directory = Path.GetDirectoryName(args.DestinationPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(args.DestinationPath, result.ResultText);

            _logger.Info($"Guardado en: {args.DestinationPath}");
            _logger.Info($"Buscado: '{args.SearchText}' → Reemplazado por: '{args.ReplacementText}'");

            return result;
        }
    }
}
