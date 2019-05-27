using Xunit;
using System;    
using System.Linq;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Service.Extensions;
using Service.Models;
using Service.UnitTests.Utilities;
using Newtonsoft.Json;

namespace Service.UnitTests.Tests.Extensions {
    public class ExceptionMiddlewareShould
    {
        protected readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddlewareShould()
        {
            _logger = TestingUtility.GetNullLogger<ExceptionMiddleware>();
        }

        [Fact]
        public async Task WhenAnUnExpectedExceptionIsRaised_ExceptionMiddlewareShouldHandleItToApiExceptionResponseAndInternalServerErrorHttpStatus()
        {
            // Arrange
            var errorMessage = "Testing unhandled exceptions";
            var middleware = new ExceptionMiddleware(next: (innerHttpContext) =>
            {
                throw new Exception(errorMessage);
            }, logger: _logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await middleware.InvokeAsync(context);

            // Assert
            var expectedStatus = (int)HttpStatusCode.InternalServerError;
            Assert.Equal(context.Response.StatusCode, expectedStatus);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var exception = JsonConvert.DeserializeObject<ApiException>(streamText);
            Assert.NotEmpty(exception.Title);
            Assert.Equal(errorMessage, exception.Detail);
            Assert.Equal(expectedStatus, exception.Status);
        }
    }
}
