using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models.Requests;
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
    public async Task Validate_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTodoRequest("Buy groceries", "Milk, eggs, bread");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new CreateTodoRequest("", "Some description");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTitleExceedsMaxLength()
    {
        // Arrange
        var request = new CreateTodoRequest(new string('A', 201), "Some description");

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenDescriptionExceedsMaxLength()
    {
        // Arrange
        var request = new CreateTodoRequest("Valid Title", new string('A', 2001));

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
