using System;
using System.Collections.Generic;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    /// <summary>
    /// Base class for Frigates, Corvettes, Capitalships, and Titans.
    /// </summary>
    public abstract class Ship : Unit
    {
        public Ship() : base()
        {
        }

        public Ship(EntityType type, string entityName) : base(type, entityName)
        {
        }

        [EntityLine("slotCount")]
        public int FleetSupply { get; set; }

        [EntityLine("maxNumCommandPoints")]
        public virtual int CommandPoints { get; set; }

        [EntityLine("MaxShieldPoints")]
        public decimal Shields { get; set; }

        [EntityLine("ShieldPointRestoreRate")]
        public decimal ShieldRestore { get; set; }

        [EntityLine("maxSpeedLinear")]
        public decimal MaxSpeed { get; set; }

        #region Bombing
        [EntityLine("canBomb")]
        public bool CanBomb { get; set; }

        [EntityLine("baseDamage")]
        public decimal BombingDamage { get; set; }

        [EntityLine("bombingFreqTime")]
        /// <summary>
        /// bombingFreqTime
        /// </summary>
        public decimal BombingCooldown { get; set; }

        [EntityLine("basePopulationKilled")]
        public decimal BombingPopulationKilled { get; set; }

        public decimal BombingDPS
        {
            get
            {
                return BombingCooldown == 0 ? 0 : BombingDamage / BombingCooldown;
            }
        }

        public decimal BombingPopKilledPerSecond
        {
            get
            {
                return BombingCooldown == 0 ? 0 : BombingPopulationKilled / BombingCooldown;
            }
        } 
        #endregion

        public decimal GetValuePerUnit(decimal value)
        {
            if (FleetSupply == 0)
            {
                return 0;
            }
            return value / FleetSupply;
        }

        public override decimal TotalSurviveability
        {
            get
            {
                return base.TotalSurviveability + Shields;
            }
        }

        public override string ExportToCSV()
        {
            return $@"{EntityName},{Name},{Type},{StatCountType},{ArmorType},{FleetSupply},{Credits},{GetValuePerUnit(Credits)},{Metal},{GetValuePerUnit(Metal)},{Crystal},{GetValuePerUnit(Crystal)}," +
                $@"{BuildTime},{GetValuePerUnit(BuildTime)},{XP},{GetValuePerUnit(XP)},{Hull},{GetValuePerUnit(Hull)},{HullRestore},{GetValuePerUnit(HullRestore)},{Shields},{GetValuePerUnit(Shields)}," +
                $@"{ShieldRestore},{GetValuePerUnit(ShieldRestore)},{Armor},{TotalSurviveability},{GetValuePerUnit(TotalSurviveability)},{Antimatter},{GetValuePerUnit(Antimatter)},{AntimatterRestore},{GetValuePerUnit(AntimatterRestore)}," +
                $@"{DPS},{GetValuePerUnit(DPS)},{MaxSingleTargetDPS},{GetValuePerUnit(MaxSingleTargetDPS)},{MaxRange},{BombingDPS},{GetValuePerUnit(BombingDPS)},{BombingPopKilledPerSecond},{GetValuePerUnit(BombingPopKilledPerSecond)},{CommandPoints},{GetValuePerUnit(CommandPoints)},{MaxSpeed}";
        }
    }
}
