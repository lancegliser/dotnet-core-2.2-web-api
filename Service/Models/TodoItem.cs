using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Service.Models
{
    public class TodoItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public long Id { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [DefaultValue(false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isComplete")]
        public bool IsComplete { get; set; }
    }
}