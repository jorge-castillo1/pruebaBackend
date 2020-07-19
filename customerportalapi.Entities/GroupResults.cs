using Newtonsoft.Json;
using System.Collections.Generic;

namespace customerportalapi.Entities
{
    public class GroupResults
    {
        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }
        [JsonProperty("Resources")]
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
