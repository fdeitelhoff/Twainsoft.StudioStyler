using System.Collections.Generic;
using Newtonsoft.Json;

namespace Twainsoft.StudioStyler.Services.StudioStyles.Model
{
    public class Schemes
    {
        [JsonProperty(PropertyName = "schemes")]
        public IList<Scheme> AllSchemes { get; set; }
    }
}
