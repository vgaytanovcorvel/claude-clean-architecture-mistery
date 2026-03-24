using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Exceptions;

namespace MisteryApp.Abstractions.Tests;

[TestClass]
public class NotFoundExceptionTests
{
    [TestMethod]
    public void Constructor_ShouldSetMessage_WhenMessageIsProvided()
    {
        // Arrange
        var message = "User not found (UserId: 42).";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [TestMethod]
    public void NotFoundException_ShouldBeException_WhenCreated()
    {
        // Arrange & Act
        var exception = new NotFoundException("Not found");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
