using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ExpenseReportDialogflow.Util
{
    public class ConfigurationSerializer
    {
        public readonly IConfiguration Config;

        public ConfigurationSerializer(IConfiguration cfg)
        {
            Config = cfg;
        }

        private void ReplaceWithArray(ExpandoObject parent, string key, ExpandoObject input)
        {
            if (input == null)
                return;

            var dict = input as IDictionary<string, object>;
            var keys = dict.Keys.ToArray();

            // it's an array if all keys are integers
            if (keys.All(k => int.TryParse(k, out var dummy))) {
                var array = new object[keys.Length];
                foreach (var kvp in dict) {
                    array[int.Parse(kvp.Key)] = kvp.Value;
                }

                var parentDict = parent as IDictionary<string, object>;
                parentDict.Remove(key);
                parentDict.Add(key, array);
            }
            else {
                foreach (var childKey in dict.Keys.ToList()) {
                    ReplaceWithArray(input, childKey, dict[childKey] as ExpandoObject);
                }
            }
        }

        public IDictionary<string, object> ToDictionary()
        {
            var result = new ExpandoObject();
            foreach (var kv in Config.AsEnumerable()) {
                var node = result as IDictionary<string, object>;
                var path = kv.Key.Split(':');

                for (int i = 0; i < path.Length - 1; i++) {
                    if (!node.ContainsKey(path[i]))
                        node.Add(path[i], new ExpandoObject());
                    node = node[path[i]] as IDictionary<string, object>;
                }

                if (kv.Value == null)
                    continue;
                node.Add(path.Last(), kv.Value);
            }
            // at this stage, all arrays are seen as dictionaries with integer keys
            ReplaceWithArray(null, null, result);
            return result as IDictionary<string, object>;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(ToDictionary(), Formatting.Indented);
        }
    }
}
