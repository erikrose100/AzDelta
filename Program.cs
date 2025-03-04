using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ResourceGraph;
using Azure.ResourceManager.ResourceGraph.Models;
using System.Text.Json;
using AzDelta;
using System.Collections.Immutable;

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

var credential = new DefaultAzureCredential(includeInteractiveCredentials: true);

var armClient = new ArmClient(credential);

var tenantResource = armClient.GetTenants().FirstOrDefault();

var allSubs = await armClient.GetSubscriptions().GetAllAsync().ToListAsync();

var subIds = allSubs.Select(x => x.Data.SubscriptionId).ToImmutableArray();

var query = """
resourcechanges
| extend targetResourceType = properties.targetResourceType
| extend changedBy = properties.changeAttributes.changedBy
| extend clientType = properties.changeAttributes.clientType
| extend timestamp = properties.changeAttributes.timestamp
| extend operation = properties.changeAttributes.operation
| extend changeType = properties.changeType
| extend changes = properties.changes
| project id, tenantId, location, resourceGroup, subscriptionId, targetResourceType, changedBy, clientType, timestamp, operation, changeType, changes
""";

const int groupSize = 100;
for (var i = 0; i <= subIds.Length / groupSize; ++i)
{
    var currSubscriptionGroup = subIds.Skip(i * groupSize).Take(groupSize);

    var content = new ResourceQueryContent(query);

    foreach (var sub in currSubscriptionGroup)
    {
        content.Subscriptions.Add(sub);
    }

    var result = await tenantResource.GetResourcesAsync(content);

    if (result.GetRawResponse().IsError)
    {
        throw new HttpRequestException($"Query returned {result.GetRawResponse()} error code");
    }

    var desData = JsonSerializer.Deserialize<List<ResourceChange>>(result.Value.Data, options);

    foreach (var res in desData!)
    {
        var values = res.Changes?.ChangeValues?.Values.FirstOrDefault().Deserialize<ChangeValues>(options);
        Console.WriteLine("Property: {0}\n\tPrevious value: {1}\n\tNew value: {2}", res.Changes?.ChangeValues?.Keys.FirstOrDefault(), values!.PreviousValue, values.NewValue);
    }
}
