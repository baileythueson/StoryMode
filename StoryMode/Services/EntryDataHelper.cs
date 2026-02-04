using System.Text.Json.Nodes;

namespace StoryMode.Services;

public class EntryDataHelper
{
    public static T? GetValue<T>(string json, string key)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;

        try
        {
            var node = JsonNode.Parse(json);
            if (node?[key] is not null)
            {
                var value = node[key];
                if (value is null) return default;
                return value.GetValue<T>() ?? default;
            }
        }
        catch 
        {
            return default;
        }
        return default; // only reached if parsing fails
    }

    public static string SetValue<T>(string json, string key, T value)
    {
        var node = JsonNode.Parse(json ?? "{}") ?? new JsonObject();
        node[key] = JsonValue.Create(value);
        return node.ToJsonString();
    }
}