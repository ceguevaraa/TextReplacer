using FluentAssertions;
using TextReplacer.Logger;
using TextReplacer.Models;
using TextReplacer.Services;

namespace TextReplacer.Tests
{
    public class FileTextReplacerTests
    {
        private readonly ITextReplacerService _service = new TextReplacerService();
        private readonly ILogger _logger = new SilentLogger();

        [Fact]
        public void ReplaceInFile_ShouldCreateDestinationFile()
        {
            string source = Path.GetTempFileName();
            string destination = Path.Combine(Path.GetTempPath(), "output.txt");
            File.WriteAllText(source, "foo bar foo");

            var canProcess = AppArguments.TryParse(new[] { source, destination, "foo", "baz" }, out var args);
            var replacer = new FileTextReplacer(_service, _logger);

            var result = replacer.ReplaceInFile(args);

            File.Exists(destination).Should().BeTrue();
            result.ReplacementCount.Should().Be(2);
        }

        private class SilentLogger : ILogger
        {
            public void Info(string message) { }
            public void Error(string message) { }
        }
    }
}
