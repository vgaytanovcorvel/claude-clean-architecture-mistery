using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models;
using MisteryApp.Implementation.Validators;

namespace MisteryApp.Implementation.Tests.Validators;

[TestClass]
public class CreateTodoRequestValidatorTests
{
    private CreateTodoRequestValidator validator = null!;

    [TestInitialize]
    public void Setup()
    {
        validator = new CreateTodoRequestValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", 1);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new CreateTodoRequest("", 1);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var longTitle = new string('x', 201);
        var request = new CreateTodoRequest(longTitle, 1);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenTitleIsExactlyMaxLength()
    {
        // Arrange
        var maxTitle = new string('x', 200);
        var request = new CreateTodoRequest(maxTitle, 1);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenUserIdIsZero()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", 0);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenUserIdIsNegative()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", -1);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }
}
