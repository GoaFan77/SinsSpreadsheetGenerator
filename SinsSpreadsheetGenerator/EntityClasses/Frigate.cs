using SinsSpreadsheetGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public class Frigate : Ship, ILinePropertyMapped, ICVSExportable
    {
        private static Dictionary<string, PropertyInfo> _linePropertyMap;
        public Dictionary<string, PropertyInfo> LinePropertyMap
        {
            get
            {
                if (_linePropertyMap == null || _linePropertyMap.Count == 0)
                {
                    _linePropertyMap = Helpers.BuildLinePropertyMap(typeof(Frigate));
                }
                return _linePropertyMap;
            }
        }

        public Frigate() : base()
        {
        }

        public Frigate(string entityName) : base(EntityType.Frigate, entityName)
        {
        }

        public override void LoadEntityValue(string entityLine, string value)
        {
            LinePropertyMap.TryGetValue(entityLine, out PropertyInfo property);
            if (property != null && !string.IsNullOrEmpty(value))
            {
                property.SetValue(this, Convert.ChangeType(Helpers.SanitizeValue(property.PropertyType, value), property.PropertyType));
            }

            base.LoadEntityValue(entityLine, value);
        }
    }
}
