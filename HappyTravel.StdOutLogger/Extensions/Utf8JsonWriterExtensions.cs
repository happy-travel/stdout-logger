﻿using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HappyTravel.StdOutLogger.Extensions
{
    public static class Utf8JsonWriterExtensions
    {
        public static void WriteDictionary(this Utf8JsonWriter writer, string propertyName, IDictionary<string, object> dictionary)
        {
            if (!dictionary.Any())
                return;
            
            writer.WritePropertyName(propertyName);
            writer.WriteStartObject();

            foreach (var (key, value) in dictionary)
            {
                writer.WritePropertyName(key);
                
                switch (value)
                {
                    case string str:
                        writer.WriteStringValue(str);
                        break;
                    
                    case ICollection<KeyValuePair<string, string>> collection:
                        writer.WriteStartObject();
                        foreach (var (k, v) in collection)
                            writer.WriteString(k, v);
                        writer.WriteEndObject();
                        break;
                    
                    default:
                        writer.WriteStringValue(value?.ToString());
                        break;
                }
            }

            writer.WriteEndObject();
        }
    }
}