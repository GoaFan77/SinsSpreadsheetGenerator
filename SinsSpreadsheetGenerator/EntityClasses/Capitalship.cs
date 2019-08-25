using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public class Capitalship : Ship
    {
        private static Dictionary<string, PropertyInfo> _linePropertyMap;
        public Dictionary<string, PropertyInfo> LinePropertyMap
        {
            get
            {
                if (_linePropertyMap == null || _linePropertyMap.Count == 0)
                {
                    _linePropertyMap = Helpers.BuildLinePropertyMap(typeof(Capitalship));
                }
                return _linePropertyMap;
            }
        }

        public Capitalship() : base()
        {
            XP = 100;
        }

        public Capitalship(string entityName) : base(EntityType.CapitalShip, entityName)
        {
            XP = 100;
        }

        [EntityLine("ArmorPointsFromExperience")]
        public override decimal Armor { get; set; }

        [EntityLine("CommandPoints")]
        public override int CommandPoints { get; set; }

        public override void LoadEntityValue(string entityLine, string value)
        {
            PropertyInfo property;
            LinePropertyMap.TryGetValue(entityLine, out property);
            if (property != null)
            {
                property.SetValue(this, Convert.ChangeType(Helpers.SanitizeValue(property.PropertyType, value), property.PropertyType));
            }
        }
    }
}
