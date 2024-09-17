using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HomelessAnimals.Shared.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SignUpRequestStatus
    {
        Pending,
        Rejected,
        Accepted
    }
}
