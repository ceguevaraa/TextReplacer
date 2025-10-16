using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextReplacer.Services;

namespace TextReplacer.Tests
{
    public class TextReplacerServiceTests
    {
        private readonly TextReplacerService _service = new();

        [Fact]
        public void Replace_ShouldReplaceAllOccurrences()
        {
            string input = "Hello world. Hello Cesar.";
            var result = _service.Replace(input, "Hello", "Hola");

            result.ResultText.Should().Be("Hola world. Hola Cesar.");
            result.ReplacementCount.Should().Be(2);
        }

        [Fact]
        public void Replace_ShouldReturnZero_WhenNoMatches()
        {
            var result = _service.Replace("abc", "xyz", "qwe");
            result.ReplacementCount.Should().Be(0);
            result.ResultText.Should().Be("abc");
        }

        [Fact]
        public void Replace_ShouldThrow_WhenSearchTextIsEmpty()
        {
            var act = () => _service.Replace("hola", "", "adios");
            act.Should().Throw<ArgumentException>().WithMessage("*vacío*");
        }
    }
}
