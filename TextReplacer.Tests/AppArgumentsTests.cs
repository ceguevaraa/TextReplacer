using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextReplacer.Models;
using FluentAssertions;
using Xunit;

namespace TextReplacer.Tests
{
    public class AppArgumentsTests
    {
        [Fact]
        public void TryParse_ShouldFail_WhenInvalidNumberOfArgs()
        {
            var success = AppArguments.TryParse(new[] { "one", "two" }, out var result);
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParse_ShouldFail_WhenSearchTextEmpty()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "test");

            var success = AppArguments.TryParse(new[] { path, "out.txt", "", "x" }, out var result);
            success.Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void TryParse_ShouldReturnValid_WhenCorrectArgs()
        {
            string path = Path.GetTempFileName();
            File.WriteAllText(path, "test");

            var success = AppArguments.TryParse(new[] { path, "out.txt", "a", "b" }, out var result);

            success.Should().BeTrue();
            result.Should().NotBeNull();
            result!.SearchText.Should().Be("a");
        }
    }
}
