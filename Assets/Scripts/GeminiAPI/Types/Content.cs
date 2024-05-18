using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeminiAPI.Types
{
    public enum Role
    {
        [EnumMember(Value = "user")] User,
        [EnumMember(Value = "model")] Model
    }

    public partial record Content
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Role? role;

        public List<Part> parts;

        [JsonConstructor]
        public Content(Role role, params Part[] parts)
        {
            this.parts = new List<Part>(parts);
            this.role = role;
        }

        public Content(Role role, Part part)
        {
            parts = new List<Part> { part };
            this.role = role;
        }

        public Content(Part part)
        {
            parts = new List<Part> { part };
        }
    }

    public partial record Content
    {
        public record Part
        {
            public string text;

            public static implicit operator Part(string text) => new() { text = text };
        }
    }
}