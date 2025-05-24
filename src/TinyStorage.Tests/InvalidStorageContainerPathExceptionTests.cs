namespace TinyStorage.Tests;

using System;
using Xunit;

public sealed class InvalidStorageContainerPathExceptionTests
{
    [Fact]
    public void ConstructorWithoutArguments_HasExpectedProperties()
    {
        var ex = new InvalidStorageContainerPathException();
        Assert.NotEmpty(ex.Message);
        Assert.Null(ex.InnerException);
    }

    [Fact]
    public void ConstructorWithMessage_HasExpectedProperties()
    {
        var message = "Custom Message";
        var ex = new InvalidStorageContainerPathException(message);
        Assert.Equal(message, ex.Message);
    }

    [Fact]
    public void ConstructorWithMessageAndInner_HasExpectedProperties()
    {
        var message = "Custom Message";
        var innerException = new Exception();
        var ex = new InvalidStorageContainerPathException(message, innerException);
        Assert.Equal(message, ex.Message);
        Assert.Equal(innerException, ex.InnerException);
    }
}
