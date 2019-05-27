using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace Service.Models
{
    public class ApiException : ProblemDetails
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "exception")]
        public System.Exception Exception;
    }
}