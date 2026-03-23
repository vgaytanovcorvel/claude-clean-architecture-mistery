using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Exceptions;

namespace MisteryApp.Abstractions.Tests.Exceptions;

[TestClass]
public class NotFoundExceptionTests
{
    [TestMethod]
    public void Constructor_ShouldSetMessage_WhenMessageIsProvided()
    {
        // Arrange
        var message = "User not found (UserId: 123).";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [TestMethod]
    public void Constructor_ShouldInheritFromException_WhenCreated()
    {
        // Arrange & Act
        var exception = new NotFoundException("test");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
