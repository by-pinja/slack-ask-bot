using System;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;

namespace AzureFunctions
{
    public static class JsonExtensions
    {
        private static string? Get(this JObject json, Func<dynamic, dynamic> selector, [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            try
            {
                var result = selector((dynamic)json);

                if (result == null)
                    return null;

                return result switch
                {
                    JObject o => o.Value<string>(),
                    JToken o => o.Value<string>(),
                    _ => throw new InvalidOperationException($"No defined conversion for JSON object {result?.GetType()}"),
                };
            }
            catch (RuntimeBinderException)
            {
                throw new InvalidOperationException($"Invalid path before last from JSON {callerFile}:{callerLine}");
            }
        }

        public static string? GetString(this JObject json, Func<dynamic, dynamic> selector, [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            return Get(json, selector, callerFile, callerLine);
        }

        public static string RequireString(this JObject json, Func<dynamic, dynamic> selector, [CallerFilePath] string callerFile = "", [CallerLineNumber] int callerLine = 0)
        {
            return Get(json, selector, callerFile, callerLine) ?? throw new InvalidOperationException($"Invalid last select from JSON {callerFile}:{callerLine}");
        }
    }
}