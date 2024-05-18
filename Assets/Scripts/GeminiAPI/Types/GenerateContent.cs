#nullable enable

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeminiAPI.Types
{
    public record GenerateContentRequest
    {
        public ICollection<Content>? contents;
        [JsonProperty("system_instruction")] public Content? systemInstruction;

        public static implicit operator GenerateContentRequest(Content[] contents) => new() { contents = contents };
        public static implicit operator GenerateContentRequest(List<Content> contents) => new() { contents = contents };
    }

    public partial record GenerateContentResponse
    {
        public Candidate[] candidates;
        public PromptFeedback? promptFeedback;

        public GenerateContentResponse(Candidate[] candidates, PromptFeedback? promptFeedback)
        {
            this.candidates = candidates;
            this.promptFeedback = promptFeedback;
        }
    }

    partial record GenerateContentResponse
    {
        public enum BlockReason
        {
            BLOCK_REASON_UNSPECIFIED,
            SAFETY,
            OTHER,
        }

        public record PromptFeedback
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public BlockReason blockReason;
        }
    }
}