using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzDelta
{
    public class ResourceChange
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

    public class Changes
    {
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ChangeValues { get; set; }
    }

    public class ChangeValues
    {
        private string? _prev;
        public string PreviousValue { get { return _prev ?? "null"; } set { _prev = value; } }

        private string? _new;
        public string NewValue { get { return _new ?? "null"; } set { _new = value; } }
    }
}