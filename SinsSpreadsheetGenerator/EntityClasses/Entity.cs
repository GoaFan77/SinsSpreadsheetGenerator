using SinsSpreadsheetGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    /// <summary>
    /// Base class for common functionality in all Entity files.
    /// </summary>
    public abstract class Entity : ICVSExportable
    {

        public Entity()
        {
        }

        public Entity(EntityType type, string entityName)
        {
            Type = type;
            EntityName = entityName;
        }

        // Used for setting priority when parsing multiple mods to determine which version of the entity to use.
        public short ModOrder { get; set; }

        public EntityType Type { get; set; }

        public string EntityName { get; set; }

        public string Name { get; set; }

        [EntityLine("statCountType")]
        public string StatCountType { get; set; }

        public virtual void AddWeapon(Weapon weapon)
        {
            return;
        }

        public abstract string ExportToCSV();

        public abstract void LoadEntityValue(string entityLine, string value);
    }
}
