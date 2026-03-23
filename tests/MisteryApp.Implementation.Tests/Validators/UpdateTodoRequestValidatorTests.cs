using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models.Requests;
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
    public async Task Validate_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateTodoRequest("Updated Title", "Updated Description", true);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var request = new UpdateTodoRequest("", "Some description", false);

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
        var request = new UpdateTodoRequest(new string('A', 201), "Some description", false);

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
        var request = new UpdateTodoRequest("Valid Title", new string('A', 2001), false);

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
