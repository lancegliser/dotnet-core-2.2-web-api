using System;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Service.Utilities
{
    public class ValidationUtility
    {

        public static ValidationProblemDetails GetValidationProblemDetails(string key, string errorMessage)
        {
            var errors = new Dictionary<string, string[]>()
            {
                {key, new string[] {errorMessage}}
            };
            return new ValidationProblemDetails(errors){
                Status = (int)HttpStatusCode.BadRequest
            };
        }

        public static ValidationProblemDetails GetValidationProblemDetails(string key, string[] errorMessages)
        {
            var errors = new Dictionary<string, string[]>()
            {
                {key, errorMessages}
            };
            return new ValidationProblemDetails(errors){
                Status = (int)HttpStatusCode.BadRequest
            };
        }

    }
}
