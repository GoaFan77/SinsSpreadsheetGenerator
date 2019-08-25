using SinsSpreadsheetGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public class Weapon : ILinePropertyMapped
    {
        private static Dictionary<string, PropertyInfo> _linePropertyMap;
        public Dictionary<string, PropertyInfo> LinePropertyMap
        {
            get
            {
                if (_linePropertyMap == null || _linePropertyMap.Count == 0)
                {
                    _linePropertyMap = Helpers.BuildLinePropertyMap(typeof(Weapon));
                }
                return _linePropertyMap;
            }
        }

        [EntityLine("AttackType")]
        public string DamageType { get; set; }

        [EntityLine("DamagePerBank:FRONT")]
        public decimal DamageFront { get; set; }

        [EntityLine("DamagePerBank:BACK")]
        public decimal DamageBack { get; set; }

        [EntityLine("DamagePerBank:LEFT")]
        public decimal DamageLeft { get; set; }

        [EntityLine("DamagePerBank:RIGHT")]
        public decimal DamageRight { get; set; }

        [EntityLine("Range")]
        public decimal Range { get; set; }

        [EntityLine("PreBuffCooldownTime")]
        public decimal Cooldown { get; set; }

        public void LoadEntityValue(string entityLine, string value)
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
