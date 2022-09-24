using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public class Titan: Ship
    {
        private static Dictionary<string, PropertyInfo> _linePropertyMap;
        public Dictionary<string, PropertyInfo> LinePropertyMap
        {
            get
            {
                if (_linePropertyMap == null || _linePropertyMap.Count == 0)
                {
                    _linePropertyMap = Helpers.BuildLinePropertyMap(typeof(Titan));
                }
                return _linePropertyMap;
            }
        }
        public Titan() : base()
        {
            XP = 350;
        }

        public Titan(string entityName) : base(EntityType.Titan, entityName)
        {
            XP = 350;
        }

        [EntityLine("ArmorPointsFromExperience")]
        public override decimal Armor { get; set; }

        [EntityLine("CommandPoints")]
        public override int CommandPoints { get; set; }

        private string prevLine { get; set; }

        public override void LoadEntityValue(string entityLine, string value)
        {
            // Capitalship/Titan stat level up blocks are defined on the line before, then have a "StartValue".
            if (entityLine == "StartValue")
            {
                entityLine = prevLine;
            }
            prevLine = entityLine;

            LinePropertyMap.TryGetValue(entityLine, out PropertyInfo property);
            if (property != null && !string.IsNullOrEmpty(value))
            {
                property.SetValue(this, Convert.ChangeType(Helpers.SanitizeValue(property.PropertyType, value), property.PropertyType));
            }

            base.LoadEntityValue(entityLine, value);
        }
    }
}
