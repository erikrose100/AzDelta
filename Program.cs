// See https://aka.ms/new-console-template for more information
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.ResourceGraph;
using Azure.ResourceManager.ResourceGraph.Models;
using System.Text.Json;
using AzDelta;

Console.WriteLine("Hello, World!");

var json = """
{
    "id": "/subscriptions/913575a3-0000-0000-0000-4d69586885ef/resourceGroups/rgName/providers/Microsoft.App/containerApps/appName/providers/Microsoft.Resources/changes/08584614150393775807_0c5941e8-78ca-4d40-92c7-9a373466716c",
    "tenantId": "1e13d1f5-0000-0000-0000-d4092c853cbd",
    "location": "eastus2",
    "resourceGroup": "container-resources",
    "subscriptionId": "913575a3-0000-0000-0000-4d69586885ef",
    "targetResourceType": "microsoft.app/containerapps",
    "changedBy": "email@gmail.com",
    "clientType": "Azure Portal",
    "timestamp": "2025-02-22T02:47:44.399Z",
    "operation": "Microsoft.App/containerApps/write",
    "changeType": "Update",
    "changes": {
        "tags.eee": {
            "previousValue": "eee",
            "newValue": null
        }
    }
}
""";

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};
var des = JsonSerializer.Deserialize<ResourceChange>(json, options);

Console.WriteLine(des?.ChangedBy);

var credential = new DefaultAzureCredential(includeInteractiveCredentials: true);

var armClient = new ArmClient(credential);

var tenantResource = armClient.GetTenants().FirstOrDefault();

var subscriptionId = armClient.GetDefaultSubscription().Data.SubscriptionId;

Console.WriteLine(subscriptionId);

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
| limit 3
""";

var content = new ResourceQueryContent(query)
{
    Subscriptions =
    {
        subscriptionId
    },
};
var result = await tenantResource.GetResourcesAsync(content);

Console.WriteLine($"Succeeded: {result.Value.Data}");

var desData = JsonSerializer.Deserialize<List<ResourceChange>>(result.Value.Data, options);

foreach (var res in desData!)
{
    var values = res.Changes?.ChangeValues?.Values.FirstOrDefault().Deserialize<ChangeValues>(options);
    Console.WriteLine("{0}: {1}", values.PreviousValue, values.NewValue);
}
