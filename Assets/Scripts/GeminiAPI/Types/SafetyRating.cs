using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeminiAPI.Types
{
    public partial record SafetyRating
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HarmCategory category;

        [JsonConverter(typeof(StringEnumConverter))]
        public HarmProbability probability;

        public bool? blocked;
    }

    partial record SafetyRating
    {
        public enum HarmCategory
        {
            [EnumMember(Value = "HARM_CATEGORY_UNSPECIFIED")]
            Unspecified,

            [EnumMember(Value = "HARM_CATEGORY_DEROGATORY")]
            Derogatory,

            [EnumMember(Value = "HARM_CATEGORY_TOXICITY")]
            Toxicity,

            [EnumMember(Value = "HARM_CATEGORY_VIOLENCE")]
            Violence,

            [EnumMember(Value = "HARM_CATEGORY_SEXUAL")]
            Sexual,

            [EnumMember(Value = "HARM_CATEGORY_MEDICAL")]
            Medical,

            [EnumMember(Value = "HARM_CATEGORY_DANGEROUS")]
            Dangerous,

            [EnumMember(Value = "HARM_CATEGORY_HARASSMENT")]
            Harassment,

            [EnumMember(Value = "HARM_CATEGORY_HATE_SPEECH")]
            HateSpeech,

            [EnumMember(Value = "HARM_CATEGORY_SEXUALLY_EXPLICIT")]
            SexuallyExplicit,

            [EnumMember(Value = "HARM_CATEGORY_DANGEROUS_CONTENT")]
            DangerousContent,
        }

        public enum HarmProbability
        {
            [EnumMember(Value = "HARM_PROBABILITY_UNSPECIFIED")]
            Unspecified,
            [EnumMember(Value = "NEGLIGIBLE")] Negligible,
            [EnumMember(Value = "LOW")] Low,
            [EnumMember(Value = "MEDIUM")] Medium,
            [EnumMember(Value = "HIGH")] High,
        }
    }
}