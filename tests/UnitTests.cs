using System.Text.Json;
using Xunit;
using AzDelta;
using Microsoft.Extensions.Azure;

namespace Tests;

public class UnitTests
{
    [Fact]
    public void JsonUnitTest()
    {
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

        Assert.NotNull(des);
        Assert.NotNull(des.Changes);
        Assert.NotNull(des.Changes.ChangeValues);
        Assert.NotEmpty(des.Changes.ChangeValues);
        Assert.NotNull(des.Changes.ChangeValues.Keys.FirstOrDefault());

        var values = des.Changes.ChangeValues.Values.FirstOrDefault().Deserialize<ChangeValues>(options);
        
        Assert.NotNull(values);
        Assert.NotNull(values.NewValue);
        Assert.NotNull(values.PreviousValue);
    }
}
