using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class ApiKeyMiddlewareTests
{
    [Fact]
    public async Task Middleware_ValidApiKey_AllowsRequest()
    {
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var middleware = new ApiKeyMiddleware(mockRequestDelegate.Object);

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DatabaseContext(options);

        var validApiKey = "test-api-key";
        var hashedApiKey = BCrypt.Net.BCrypt.HashPassword(validApiKey);
        dbContext.Api_Keys.Add(new Api_Key
        {
            Id = 6,
            ApiKey = hashedApiKey,
            Warehouse_Id = 3,
            App = "dashboard"
        });
        await dbContext.SaveChangesAsync();

        var context = new DefaultHttpContext();
        context.Request.Headers["API_KEY"] = validApiKey;

        await middleware.InvokeAsync(context, dbContext);

        mockRequestDelegate.Verify(next => next(context), Times.Once);
        Assert.Equal(200, context.Response.StatusCode); 
    }

    [Fact]
    public async Task Middleware_InvalidApiKey_ReturnsForbidden()
    {
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var middleware = new ApiKeyMiddleware(mockRequestDelegate.Object);

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DatabaseContext(options);

        var validApiKey = "test-api-key";
        var hashedApiKey = BCrypt.Net.BCrypt.HashPassword(validApiKey);
        dbContext.Api_Keys.Add(new Api_Key
        {
            Id = 7,
            ApiKey = hashedApiKey,
            Warehouse_Id = 4,
            App = "dashboard"
        });
        await dbContext.SaveChangesAsync();

        var context = new DefaultHttpContext();
        context.Request.Headers["API_KEY"] = "invalid-api-key";

        await middleware.InvokeAsync(context, dbContext);

        mockRequestDelegate.Verify(next => next(context), Times.Never);
        Assert.Equal(403, context.Response.StatusCode); 
    }

    [Fact]
    public async Task Middleware_MissingApiKey_ReturnsUnauthorized()
    {
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var middleware = new ApiKeyMiddleware(mockRequestDelegate.Object);

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DatabaseContext(options);

        dbContext.Api_Keys.Add(new Api_Key
        {
            Id = 8,
            ApiKey = BCrypt.Net.BCrypt.HashPassword("test-api-key"),
            Warehouse_Id = 5,
            App = "dashboard"
        });
        await dbContext.SaveChangesAsync();

        var context = new DefaultHttpContext(); 

        await middleware.InvokeAsync(context, dbContext);

        mockRequestDelegate.Verify(next => next(context), Times.Never);
        Assert.Equal(401, context.Response.StatusCode); 
    }
}