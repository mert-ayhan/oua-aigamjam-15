#nullable enable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeminiAPI.Types
{
    public partial record Candidate
    {
        public Content content;

        [JsonConverter(typeof(StringEnumConverter))]
        public FinishReason finishReason;

        public int index;
        public SafetyRating[]? safetyRatings;

        public Candidate(Content content)
        {
            this.content = content;
        }
    }

    partial record Candidate
    {
        public enum FinishReason
        {
            [EnumMember(Value = "FINISH_REASON_UNSPECIFIED")]
            Unspecified,
            [EnumMember(Value = "STOP")] Stop,
            [EnumMember(Value = "MAX_TOKENS")] MaxTokens,
            [EnumMember(Value = "SAFETY")] Safety,
            [EnumMember(Value = "RECITATION")] Recitation,
            [EnumMember(Value = "OTHER")] Other,
        }

        public record GroundingAttribution
        {
            public AttributionSourceId? sourceId;
            public Content? content;
        }

        public record AttributionSourceId
        {
            public GroundingPassageId? groundingPassage;
            public SemanticRetrieverChunk? semanticRetrieverChunk;
        }

        public record GroundingPassageId
        {
            public string? passageId;
            public int partIndex;
        }

        public record SemanticRetrieverChunk
        {
            public string? source;
            public string? chunk;
        }
    }
}