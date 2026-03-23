using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Tests.Models;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Success_ShouldReturnSuccessResult_WhenValueIsProvided()
    {
        // Arrange
        var value = "test value";

        // Act
        var result = Result<string>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
    }

    [TestMethod]
    public void Failure_ShouldReturnFailureResult_WhenErrorIsProvided()
    {
        // Arrange
        var error = "Something failed";

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.Value.Should().BeNull();
    }
}
