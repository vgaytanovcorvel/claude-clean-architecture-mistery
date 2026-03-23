using System.Net;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Abstractions.Tests.Models;

[TestClass]
public class ApiResponseTests
{
    [TestMethod]
    public void Ok_ShouldReturnSuccessResponse_WhenDataIsProvided()
    {
        // Arrange
        var data = "test data";

        // Act
        var response = ApiResponse<string>.Ok(data);

        // Assert
        response.Success.Should().BeTrue();
        response.Data.Should().Be(data);
        response.Error.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public void Fail_ShouldReturnFailureResponse_WhenErrorAndStatusCodeAreProvided()
    {
        // Arrange
        var error = "Something went wrong";
        var statusCode = HttpStatusCode.BadRequest;

        // Act
        var response = ApiResponse<string>.Fail(error, statusCode);

        // Assert
        response.Success.Should().BeFalse();
        response.Error.Should().Be(error);
        response.StatusCode.Should().Be(statusCode);
        response.Data.Should().BeNull();
    }

    [TestMethod]
    public void Ok_ShouldSetStatusCodeToOk_WhenCreated()
    {
        // Arrange
        var data = 42;

        // Act
        var response = ApiResponse<int>.Ok(data);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestMethod]
    public void Fail_ShouldReturnNotFoundStatusCode_WhenNotFoundIsSpecified()
    {
        // Arrange & Act
        var response = ApiResponse<string>.Fail("Not found", HttpStatusCode.NotFound);

        // Assert
        response.Success.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Error.Should().Be("Not found");
    }
}
