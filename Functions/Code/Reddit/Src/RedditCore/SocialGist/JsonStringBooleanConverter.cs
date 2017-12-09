using System;
using Newtonsoft.Json;

namespace RedditCore.SocialGist
{
    /// <summary>
    ///     SocialGist SDK returns 1s and 0s for true/false instead of standard JSON "true" or "false".  This converts those
    ///     values to normal booleans.
    /// </summary>
    internal class JsonStringBooleanConverter : JsonConverter
    {
        public override bool CanWrite => false;

        /// <summary>
        ///     We always want to write out as standard JSON format so do not change the format.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            switch (reader.Value.ToString().ToLower().Trim())
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                    return true;
                case "false":
                case "no":
                case "n":
                case "0":
                    return false;
            }

            // If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-fied error message.
            return new JsonSerializer().Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(bool?);
        }
    }
}
