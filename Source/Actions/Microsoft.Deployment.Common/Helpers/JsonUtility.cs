using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Common.Helpers
{
    public static class JsonUtility
    {
        public static JObject CreateJObjectWithValueFromObject(object response)
        {
            dynamic obj = new ExpandoObject();
            obj.value = response;
            return JObject.FromObject(obj);
        }

        public static T Deserialize<T>(string json)
        {
            return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        public static T Deserialize<T>(JToken json)
        {
            return json == null ? default(T) : Deserialize<T>(json.ToString());
        }

        public static T DeserializeContent<T>(string content)
        {
            T value = default(T);

            JObject obj = GetJsonObjectFromJsonString(content);

            if (obj != null && obj["value"] != null)
            {
                value = JsonUtility.Deserialize<T>(obj["value"].ToString());
            }

            return value;
        }

        public static List<string> DeserializeEntities(string entitiesJson, string entitiesJsonAdditional = "")
        {
            List<string> entities = new List<string>();

            try
            {
                Dictionary<string, string> dictionary = Deserialize<Dictionary<string, string>>(entitiesJson);

                if (dictionary != null)
                {
                    foreach (KeyValuePair<string, string> pair in dictionary)
                    {
                        entities.Add(pair.Key);
                    }
                }
            }
            catch
            {
                entities = entitiesJson.SplitByCommaSpaceTabReturnList();
            }

            if (!string.IsNullOrEmpty(entitiesJsonAdditional))
            {
                entities.AddRange(entitiesJsonAdditional.SplitByCommaSpaceTabReturnList());
            }

            return entities;
        }

        public static dynamic GetDynamicFromJObject(JObject json)
        {
            var jsonString = json.Root.ToString();
            var converter = new ExpandoObjectConverter();
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, converter);
            return obj;
        }

        public static JObject GetEmptyJObject()
        {
            return GetJsonObjectFromJsonString("{}");
        }

        public static JObject GetJObjectFromJsonString(string json)
        {
            JObject templatefileContent = new JObject();

            if (string.IsNullOrEmpty(json) || json.EqualsIgnoreCase("null"))
            {
                json = GetEmptyJObject().ToString();
            }

            templatefileContent = (JObject)JToken.Parse(json);
            return templatefileContent;
        }

        public static JObject GetJObjectFromObject(object json)
        {
            if (json == null)
            {
                return new JObject();
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            var obj = JObject.FromObject(json);
            return obj;
        }

        public static JObject GetJObjectFromStringValue(string value)
        {
            return GetJsonObjectFromJsonString("{\"value\":" + JsonConvert.ToString(value) + "}");
        }

        public static string GetJObjectProperty(JObject obj, string property)
        {
            return obj[property] == null ? null : obj[property].ToString();
        }

        public static JObject GetJsonObjectFromJsonString(string json)
        {
            var obj = JObject.Parse(json);
            return obj;
        }

        public static string GetJsonStringFromObject(object json)
        {
            if (json == null)
            {
                return JsonUtility.GetEmptyJObject().ToString();
            }

            var obj = JObject.FromObject(json);
            return obj.Root.ToString();
        }

        public static string GetWebToken(string token, string property)
        {
            string webToken = null;

            if (token != null)
            {
                foreach (Claim c in new JwtSecurityToken(token).Claims)
                {
                    if (c.Type.ToLowerInvariant().EqualsIgnoreCase(property))
                    {
                        webToken = c.Value;
                        break;
                    }
                }
            }

            return webToken;
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return token == null
                || token.Type == JTokenType.Array && !token.HasValues
                || token.Type == JTokenType.Object && !token.HasValues
                || token.Type == JTokenType.String && token.ToString() == String.Empty
                || token.Type == JTokenType.Null;
        }

        public static string Serialize<T>(T value)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new JsonUtilityLowercaseSerializer();
            return JsonConvert.SerializeObject(value, settings);
        }

        public static string SerializeTable(DataTable table)
        {
            string result;

            JsonSerializer json = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            json.Converters.Add(new DataTableConverter());

            StringWriter sw = null;
            try
            {
                sw = new StringWriter(CultureInfo.InvariantCulture);
                using (JsonTextWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented, QuoteChar = '"' })
                {
                    json.Serialize(writer, table);
                    result = sw.ToString();
                    sw = null;
                }
            }
            finally
            {
                if (sw != null)
                    sw.Dispose();
            }

            return result;
        }
    }
}