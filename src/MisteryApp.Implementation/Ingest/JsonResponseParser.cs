namespace MisteryApp.Implementation.Ingest;

internal static class JsonResponseParser
{
    internal static string ExtractObject(string response)
    {
        var start = response.IndexOf('{');
        var end = response.LastIndexOf('}');
        return start >= 0 && end >= start ? response[start..(end + 1)] : "{}";
    }

    internal static string ExtractArray(string response)
    {
        var start = response.IndexOf('[');
        var end = response.LastIndexOf(']');
        return start >= 0 && end >= start ? response[start..(end + 1)] : "[]";
    }
}
