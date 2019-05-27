using Xunit;
using System;    
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Service.Extensions;
using Service.Models;
using Service.Utilities;

namespace Service.UnitTests.Tests.Utilities {
    public class ValidationUtilityShould
    {

        [Fact]
        public void WhenAnErrorKeyAndMessageAreProvided_ValidationUtilityShouldReturnValidationProblemDetailsWithBadRequestStatusCode()
        {
            // Arrange
            string key = "name";
            string errorMessage = "Must be defined";

            //Act
            var details = ValidationUtility.GetValidationProblemDetails(key, errorMessage);

            // Assert
            Assert.IsType<ValidationProblemDetails>(details);
            Assert.Equal((int)HttpStatusCode.BadRequest, details.Status);
            Assert.NotEmpty(details.Title);
            Assert.NotEmpty(details.Errors);
            string[] detailsErrorMessages;
            details.Errors.TryGetValue(key, out detailsErrorMessages);
            Assert.Equal(errorMessage, detailsErrorMessages.GetValue(0));
        }

        [Fact]
        public void WhenAnErrorKeyAndMessagesAreProvided_ValidationUtilityShouldReturnValidationProblemDetailsWithBadRequestStatusCode()
        {
            // Arrange
            string key = "name";
            string[] errorMessages = {"Must be defined", "Must not be null", "Must be cool"};

            //Act
            var details = ValidationUtility.GetValidationProblemDetails(key, errorMessages);

            // Assert
            Assert.IsType<ValidationProblemDetails>(details);
            Assert.Equal((int)HttpStatusCode.BadRequest, details.Status);
            Assert.NotEmpty(details.Title);
            Assert.NotEmpty(details.Errors);
            string[] detailsErrorMessages;
            details.Errors.TryGetValue(key, out detailsErrorMessages);
            for (int i = 0; i < errorMessages.Length; i++ )
            {
                Assert.Equal(errorMessages.GetValue(i), detailsErrorMessages.GetValue(i));
            }
        }
    }
}
