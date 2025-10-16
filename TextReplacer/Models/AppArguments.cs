namespace TextReplacer.Models
{
    /// <summary>
    /// Represents command line arguments in a structured way.
    /// </summary>
    public class AppArguments
    {
        public string SourcePath { get; }
        public string DestinationPath { get; }
        public string SearchText { get; }
        public string ReplacementText { get; }

        private AppArguments(string sourcePath, string destinationPath, string searchText, string replacementText)
        {
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            SearchText = searchText;
            ReplacementText = replacementText;
        }

        public static bool TryParse(string[]? args, out AppArguments? result)
        {
            result = null;
            if (args == null || args.Length != 4)
                return false;

            var source = args[0];
            var dest = args[1];
            var search = args[2];
            var replace = args[3];

            if (string.IsNullOrWhiteSpace(search))
            {
                Console.WriteLine("Error: el texto a buscar no puede estar vacío.");
                return false;
            }

            if (!File.Exists(source))
            {
                Console.WriteLine("Error: el fichero origen no existe.");
                return false;
            }

            result = new AppArguments(source, dest, search, replace);
            return true;
        }
    }
}
