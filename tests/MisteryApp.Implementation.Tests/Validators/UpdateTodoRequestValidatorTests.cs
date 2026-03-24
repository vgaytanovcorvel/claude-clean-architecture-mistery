using FluentAssertions;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models;
using MisteryApp.Implementation.Validators;

namespace MisteryApp.Implementation.Tests.Validators;

[TestClass]
public class UpdateTodoRequestValidatorTests
{
    private UpdateTodoRequestValidator validator = null!;

    [TestInitialize]
    public void Setup()
    {
        validator = new UpdateTodoRequestValidator();
    }

    [TestMethod]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateTodoRequest("Updated title");

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [TestMethod]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new UpdateTodoRequest("");

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
        var request = new UpdateTodoRequest(longTitle);

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
        var request = new UpdateTodoRequest(maxTitle);

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }
}
