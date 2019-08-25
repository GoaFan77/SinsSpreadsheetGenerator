using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public static class Helpers
    {
        public static Dictionary<string, PropertyInfo> BuildLinePropertyMap(Type type)
        {
            var linePropertyMap = new Dictionary<string, PropertyInfo>();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(EntityLineAttribute), true);
                if (attributes.Length > 0)
                {
                    linePropertyMap.Add(((EntityLineAttribute)attributes[0]).EntityLine, property);
                }
            }

            return linePropertyMap;
        }

        public static string SanitizeValue(Type type, string value)
        {
            if (type == typeof(Int32))
            {
                value = value.Split('.')[0];
            }
            return value;
        }

        public static Dictionary<string, string> BuildStringMap(string stringFilePath)
        {
            Dictionary<string, string> entityStringDictionary = new Dictionary<string, string>();
            try
            {
                var stringFile = new StreamReader(@stringFilePath);
                string line;
                string stringId = "";
                while ((line = stringFile.ReadLine()) != null)
                {
                    var lineArgs = line.Trim().Split(null, 2);
                    string label = lineArgs[0];
                    if (label == "ID")
                    {
                        stringId = lineArgs[1].Trim('"') ?? "";
                    }
                    if (label == "Value")
                    {
                        entityStringDictionary.TryAdd(stringId, lineArgs[1].Trim('"') ?? "");
                        stringId = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot process string file. Skipping entity lookup. " + ex.Message);
            }

            return entityStringDictionary;
        }
    }
}
