using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public abstract class Unit : Entity
    {
        public const int MAX_WEAPONS = 5;

        public Unit()
        {
            WeaponList = new List<Weapon>();
        }

        public Unit(EntityType type, string entityName) : base(type, entityName)
        {
            WeaponList = new List<Weapon>();
        }

        [EntityLine("armorType")]
        public string ArmorType { get; set; }

        [EntityLine("MaxHullPoints")]
        public decimal Hull { get; set; }

        [EntityLine("HullPointRestoreRate")]
        public decimal HullRestore { get; set; }

        [EntityLine("BaseArmorPoints")]
        public virtual decimal Armor { get; set; }

        [EntityLine("MaxAntiMatter")]
        public decimal Antimatter { get; set; }

        [EntityLine("AntiMatterRestoreRate")]
        public decimal AntimatterRestore { get; set; }

        [EntityLine("BuildTime")]
        public decimal BuildTime { get; set; }

        /// <summary>
        /// Represents the maxiumum total DPS of a unit.
        /// </summary>
        public decimal DPS
        {
            get
            {
                decimal DPS = 0;
                foreach (Weapon weapon in WeaponList)
                {
                    if (weapon.Cooldown == 0)
                    {
                        continue;
                    }
                    DPS += (weapon.DamageFront / weapon.Cooldown) * NumTargetsFront;
                    DPS += (weapon.DamageBack / weapon.Cooldown) * NumTargetsBack;
                    DPS += (weapon.DamageLeft / weapon.Cooldown) * NumTargetsLeft;
                    DPS += (weapon.DamageRight / weapon.Cooldown) * NumTargetsRight;
                }

                return DPS;
            }
        }

        public decimal MaxSingleTargetDPS
        {
            get
            {
                decimal[] dpsPerSide = new decimal[4];
                foreach (Weapon weapon in WeaponList)
                {
                    if (weapon.Cooldown == 0)
                    {
                        continue;
                    }
                    dpsPerSide[0] += (weapon.DamageFront / weapon.Cooldown);
                    dpsPerSide[1] += (weapon.DamageBack / weapon.Cooldown);
                    dpsPerSide[2] += (weapon.DamageLeft / weapon.Cooldown);
                    dpsPerSide[3] += (weapon.DamageRight / weapon.Cooldown);
                }

                return dpsPerSide.Max();
            }
        }

        [EntityLine("ExperiencePointsForDestroying")]
        public decimal XP { get; set; }

        [EntityLine("credits")]
        public decimal Credits { get; set; }

        [EntityLine("metal")]
        public decimal Metal { get; set; }

        [EntityLine("crystal")]
        public decimal Crystal { get; set; }

        [EntityLine("TargetCountPerBank:FRONT")]
        public int NumTargetsFront { get; set; }

        [EntityLine("TargetCountPerBank:BACK")]
        public int NumTargetsBack { get; set; }

        [EntityLine("TargetCountPerBank:LEFT")]
        public int NumTargetsLeft { get; set; }

        [EntityLine("TargetCountPerBank:RIGHT")]
        public int NumTargetsRight { get; set; }

        public decimal MaxRange
        {
            get
            {
                if (WeaponList.Count == 0)
                {
                    return 0;
                }
                return WeaponList.Select(w => w.Range).Max();
            }
        }

        public List<Weapon> WeaponList { get; set; } = new List<Weapon>();

        public decimal ArmorMultiplier
        {
            get
            {
                return 1 + (Armor * 0.05M);
            }
        }

        public virtual decimal TotalSurviveability
        {
            get
            {
                return Hull * ArmorMultiplier;
            }
        }

        private bool parseWeapon { get; set; }

        private Weapon addWeapon { get; set; } = new Weapon();

        public override void LoadEntityValue(string key, string value)
        {
            if (parseWeapon)
            {
                if (key == "fireConstraintType")
                {
                    AddWeapon(addWeapon);
                    addWeapon = new Weapon();
                    parseWeapon = false;
                    return;
                }
                addWeapon.LoadEntityValue(key, value);
            }

            if (key == "WeaponType")
            {
                parseWeapon = true;
            }
        }

        public override void AddWeapon(Weapon weapon)
        {
            WeaponList.Add(weapon);
            if (WeaponList.Count > MAX_WEAPONS)
            {
                throw new ArgumentException("Too many weapons added to entity.");
            }
        }
    }
}
