using SinsSpreadsheetGenerator.EntityClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinsSpreadsheetGenerator
{
    /// <summary>
    /// Takes a file, reads it and builds an appropriate entity class line by line.
    /// </summary>
    public class EntityReader
    {
        public string FilePath { get; set; }

        public string EntityName
        {
            get
            {
                return FilePath.Split('/').Last().Replace(".entity", "");
            }
        }

        public EntityReader(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Builds an Entity Class from the current FilePath.
        /// </summary>
        public async Task<Entity> BuildEntity(EntityType[] types, Dictionary<string, string> modStringDictionary = null)
        {
            Entity entity = null;
            string line;
            try
            {
                using (var openedFile = new StreamReader(@FilePath))
                {
                    while ((line = await openedFile.ReadLineAsync()) != null)
                    {
                        var lineArgs = line.Trim().Split();
                        string key = lineArgs[0];
                        string value = lineArgs.Length == 1 ? "" : lineArgs[1].Trim('"');

                        // Build the entity object to work with.
                        if (key == "entityType")
                        {
                            entity = EntityFactory.GetEntity(value, EntityName);

                            // Cannot process entity type, return.
                            if (entity == null || !types.Contains(entity.Type))
                            {
                                return null;
                            }
                            continue;
                        }

                        if (entity == null)
                        {
                            continue;
                        }

                        // Use the string dictionary to look up the proper name, if available.
                        if (key == "NameStringID" || key == "typeNameStringID")
                        {
                            if (modStringDictionary != null)
                            {
                                try
                                {
                                    entity.Name = modStringDictionary[value];
                                }
                                catch (Exception ex)
                                {
                                    entity.Name = " ";
                                }
                            }
                        }

                        entity.LoadEntityValue(key, value);

                        // It is possible for the entity type to change during parsing for certain items that cannot be fully determined at creation, like planets.
                        if (entity != null && !types.Contains(entity.Type))
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("FilePath", @FilePath);
            }

            return entity;
        }
    }
}
