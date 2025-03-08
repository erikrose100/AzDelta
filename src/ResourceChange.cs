using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzDelta
{
    public struct ResourceChange
    {
        public required string Id { get; set; }
        public Guid TenantId { get; set; }
        public required string Location { get; set; }
        public required string ResourceGroup { get; set; }
        public Guid SubscriptionId { get; set; }
        public required string TargetResourceType { get; set; }
        public required string ChangedBy { get; set; }
        public required string ClientType { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Operation { get; set; }
        public required string ChangeType { get; set; }
        public Changes? Changes { get; set; }
    }

    public struct Changes
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ChangeValues { get; set; }
    }

    public class ChangeValues
    {
        private string? _prev;
        public string PreviousValue { get => StripWhiteSpace(_prev) ?? "null"; set { _prev = value; } }

        private string? _new;
        public string NewValue { get => StripWhiteSpace(_new) ?? "null"; set { _new = value; } }

        private static string? StripWhiteSpace(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input; // Return null or empty directly
            }

            int newLength = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsWhiteSpace(input[i]))
                {
                    newLength++;
                }
            }

            if (newLength == input.Length)
            {
                return input; // No whitespace, return original
            }

            return string.Create(newLength, input, (buffer, source) =>
            {
                int bufferIndex = 0;
                for (int i = 0; i < source.Length; i++)
                {
                    if (!char.IsWhiteSpace(source[i]))
                    {
                        buffer[bufferIndex++] = source[i];
                    }
                }
            });
        }
    }
}