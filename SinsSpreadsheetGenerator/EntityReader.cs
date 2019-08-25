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
        public async Task<Entity> BuildEntity(Dictionary<string, string> modStringDictionary = null)
        {
            var openedFile = new StreamReader(@FilePath);
            Entity entity = null;
            bool parseWeapon = false;
            Weapon addWeapon = new Weapon();
            string line;
            string prevLine = "";
            while ((line = await openedFile.ReadLineAsync()) != null) {
                var lineArgs = line.Trim().Split();
                if (lineArgs.Length == 1)
                {
                    prevLine = lineArgs[0];
                    continue;
                }

                string key = lineArgs[0];
                string value = lineArgs[1].Trim('"') ?? "";

                // Capitalship/Titan stat level up blocks are defined on the line before, then have a "StartValue".
                if (key == "StartValue")
                {
                    key = prevLine;
                }

                // Build the entity object to work with.
                if (key == "entityType")
                {
                    entity = EntityFactory.GetEntity(value, EntityName);

                    // Cannot process entity type, return.
                    if (entity == null)
                    {
                        return null;
                    }
                    continue;
                }

                // Use the string dictionary to look up the proper name, if available.
                if (key == "NameStringID")
                {
                    if (modStringDictionary != null)
                    {
                        entity.Name = modStringDictionary[value];
                    }
                }

                if (parseWeapon)
                {
                    if (key == "WeaponType" || key == "projectileTravelEffectName")
                    {
                        entity.AddWeapon(addWeapon);
                        addWeapon = new Weapon();
                        parseWeapon = false;
                        continue;
                    }
                    addWeapon.LoadEntityValue(key, value);
                }

                if (key == "WeaponType")
                {
                    parseWeapon = true;
                }

                if (entity == null)
                {
                    continue;
                }

                entity.LoadEntityValue(key, value);
            }

            return entity;
        }
    }
}
