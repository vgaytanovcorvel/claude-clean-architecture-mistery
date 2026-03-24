using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Tests;

[TestClass]
public class ResultTests
{
    [TestMethod]
    public void Success_ShouldReturnSuccessResult_WhenValueIsProvided()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Error.Should().BeNull();
    }

    [TestMethod]
    public void Failure_ShouldReturnFailureResult_WhenErrorIsProvided()
    {
        // Arrange
        var error = "Validation failed";

        // Act
        var result = Result<int>.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
        result.Value.Should().Be(default(int));
    }

    [TestMethod]
    public void Success_ShouldReturnSuccessResultWithNullableType_WhenNullValueIsProvided()
    {
        // Arrange
        string? value = null;

        // Act
        var result = Result<string?>.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}
