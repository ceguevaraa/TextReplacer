using TextReplacer.Logger;
using TextReplacer.Models;
using TextReplacer.Services;

namespace TextReplacer
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var logger = new ConsoleLogger();

            try
            {
                //  Parse arguments
                if (!AppArguments.TryParse(args, out var appArgs))
                {
                    logger.Error("Uso: TextReplacer <origen> <destino> <texto_buscar> <texto_reemplazo>");
                    return 1;
                }

                //  Execute replacement
                var replacer = new FileTextReplacer(new TextReplacerService(), logger);
                var result = replacer.ReplaceInFile(appArgs);

                logger.Info($"Reemplazos realizados: {result.ReplacementCount}");
                return 0;
            }
            catch (Exception ex)
            {
                logger.Error($"Error inesperado: {ex.Message}");
                return 1;
            }
        }
    }
}
