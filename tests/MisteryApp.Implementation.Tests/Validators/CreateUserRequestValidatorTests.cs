using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models.Requests;
using MisteryApp.Implementation.Validators;

namespace MisteryApp.Implementation.Tests.Validators;

[TestClass]
public class CreateUserRequestValidatorTests
{
    private CreateUserRequestValidator validator = null!;

    [TestInitialize]
    public void Setup()
    {
        validator = new CreateUserRequestValidator();
    }

    [TestMethod]
    public async Task Validate_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateUserRequest("john_doe-123", "John Doe");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenUsernameIsEmpty()
    {
        // Arrange
        var request = new CreateUserRequest("", "John Doe");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenUsernameContainsInvalidCharacters()
    {
        // Arrange
        var request = new CreateUserRequest("john doe!", "John Doe");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenDisplayNameIsEmpty()
    {
        // Arrange
        var request = new CreateUserRequest("johndoe", "");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayName");
    }
}
