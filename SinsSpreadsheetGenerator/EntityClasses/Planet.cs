using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    public class Planet : Entity
    {
        private static Dictionary<string, PropertyInfo> _linePropertyMap;
        public Dictionary<string, PropertyInfo> LinePropertyMap
        {
            get
            {
                if (_linePropertyMap == null || _linePropertyMap.Count == 0)
                {
                    _linePropertyMap = Helpers.BuildLinePropertyMap(typeof(Planet));
                }
                return _linePropertyMap;
            }
        }

        public Planet() : base()
        {
            StatCountType = "Planet";
        }

        public Planet(string entityName) : base(EntityType.Planet, entityName)
        {
            StatCountType = "Planet";
        }

        public AsteroidSetup MetalAsteroids { get; set; } = new AsteroidSetup();

        public AsteroidSetup CrystalAsteroids { get; set; } = new AsteroidSetup();

        [EntityLine("healthRegenRate")]
        public decimal HealthRegenRate { get; set; }

        [EntityLine("maxPopulation")]
        public int MaxPopulation
        {
            set
            {
                switch(Path)
                {
                    case "path:Population":
                        Population = value;
                        break;
                    case "path:Social":
                        MaxSocialPopulationBonus = value;
                        break;
                    case "path:Industry":
                        MaxIndustryPopulationPenalty = value;
                        break;
                }
            }
        }

        [EntityLine("populationGrowthRate")]
        public decimal PopulationGrowthRate
        {
            set
            {
                switch (Path)
                {
                    case "path:Population":
                        GrowthRate = value;
                        break;
                    case "path:Social":
                        MaxSocialPopGrowthBonus = value;
                        break;
                    case "path:Industry":
                        MaxIndustryPopGrowthPenalty = value;
                        break;
                }
            }
        }

        [EntityLine("tradeIncomeModifier")]
        public decimal TradeIncomeModifier
        {
            set
            {
                switch (Path)
                {
                    case "path:Social":
                        MaxSocialTradePenalty = value;
                        break;
                    case "path:Industry":
                        MaxIndustryTradeBonus = value;
                        break;
                }
            }
        }

        [EntityLine("shipBuildRateModifier")]
        public decimal ShipBuildRateModifier
        {
            set
            {
                switch (Path)
                {
                    case "path:Social":
                        MaxSocialShipBuildPenalty = value;
                        break;
                    case "path:Industry":
                        MaxIndustryShipBuildBonus = value;
                        break;
                }
            }
        }

        [EntityLine("cultureSpreadRateModifier")]
        public decimal CultureSpreadRateModifier
        {
            set
            {
                switch (Path)
                {
                    case "path:Social":
                        MaxSocialCultureBonus = value;
                        break;
                    case "path:Industry":
                        MaxIndustryCulturePenalty = value;
                        break;
                }
            }
        }

        public int Population { get; set; }

        public int MaxTotalPopulation
        {
            get
            {
                return Population + MaxSocialPopulationBonus;
            }
        }

        public decimal GrowthRate { get; set; }

        #region Social Specialization
        public int MaxSocialPopulationBonus { get; set; }

        public decimal MaxSocialPopGrowthBonus { get; set; }

        public decimal MaxSocialCultureBonus { get; set; }

        public decimal MaxSocialTradePenalty { get; set; }

        public decimal MaxSocialShipBuildPenalty { get; set; }
        #endregion

        #region Industry Specialization
        public int MaxIndustryPopulationPenalty { get; set; }

        public decimal MaxIndustryPopGrowthPenalty { get; set; }

        public decimal MaxIndustryCulturePenalty { get; set; }

        public decimal MaxIndustryTradeBonus { get; set; }

        public decimal MaxIndustryShipBuildBonus { get; set; } 
        #endregion

        [EntityLine("maxModuleSlotCount:Civilian")]
        public byte MaxCivilianModules { get; set; }

        [EntityLine("maxModuleSlotCount:Tactical")]
        public byte MaxTacticalModules { get; set; }

        [EntityLine("homePlanetTaxRateBonus")]
        public decimal HomePlanetTaxBonus { get; set; }

        [EntityLine("homePlanetMetalIncomeBonus")]
        public decimal HomePlanetMetalBonus { get; set; }

        [EntityLine("homePlanetCrystalIncomeBonus")]
        public decimal HomePlanetCrystalBonus { get; set; }

        [EntityLine("maxHealth")]
        public int MaxHealth { get; set; }

        [EntityLine("corruptionPercent")]
        public decimal MaxSmugglingCorruption { get; set; }

        [EntityLine("smugglingPercent")]
        public decimal MaxSmugglingAmount { get; set; }

        private bool _IsColonizeable;
        [EntityLine("isColonizable")]
        public bool IsColonizable
        {
            get => _IsColonizeable;
            set
            {
                _IsColonizeable = value;               
                Type = _IsColonizeable ? EntityType.Planet : EntityType.PlanetUncolonizeable;
            }
        }

        [EntityLine("planetTypeForResearch")]
        public string PlanetType { get; set; }

        [EntityLine("possibleRandomPlanetBonusesCount")]
        public int NumPlanetBonuses { get; set; }

        [EntityLine("ruinPlanetType")]
        public string RuinPlanetType { get; set; }

        [EntityLine("creditsOnStrippedToTheCore")]
        public int CreditsOnRuin { get; set; }

        [EntityLine("metalOnStrippedToTheCore")]
        public int MetalOnRuin { get; set; }

        [EntityLine("crystalOnStrippedToTheCore")]
        public int CrystalOnRuin { get; set; }

        [EntityLine("extraScuttleTimeOnStrip")]
        public int ExtraScuttleTimeOnRuin { get; set; }

        [EntityLine("moveAreaRadius")]
        public int MoveArea { get; set; }

        [EntityLine("hyperspaceExitRadius")]
        public int HyperspaceArea { get; set; }

        [EntityLine("maxStarBaseCountPerPlayer")]
        public byte MaxStarbases { get; set; }

        [EntityLine("maxSpaceMineCountPerPlayer")]
        public byte MaxSpaceMines { get; set; }


        private string Path { get; set; }

        private AsteroidType? CurrentAsteroidType { get; set; } = null;

        public override void LoadEntityValue(string entityLine, string value)
        {
            // Set what upgrade type we're to accomidate lines with the same label.
            if (entityLine.StartsWith("path"))
            {
                Path = entityLine;
                return;
            }

            LinePropertyMap.TryGetValue(entityLine, out PropertyInfo property);
            if (property != null && !string.IsNullOrEmpty(value))
            {
                property.SetValue(this, Convert.ChangeType(Helpers.SanitizeValue(property.PropertyType, value), property.PropertyType));
            }

            // End of Asteroid Setup section.
            if (entityLine == "isAsteroid")
            {
                CurrentAsteroidType = null;
            }

            if (CurrentAsteroidType == AsteroidType.Metal)
            {
                MetalAsteroids.LoadEntityValue(entityLine, value);
            }
            else if (CurrentAsteroidType == AsteroidType.Crystal)
            {
                CrystalAsteroids.LoadEntityValue(entityLine, value);
            }

            if (entityLine == "metalResourceAsteroidSetup") 
            {
                CurrentAsteroidType = AsteroidType.Metal;
            }
            else if (entityLine == "crystalResourceAsteroidSetup")
            {
                CurrentAsteroidType = AsteroidType.Crystal;
            }
            else if (entityLine == "neutralMetalResourceAsteroidSetup")
            {
                CurrentAsteroidType = AsteroidType.NeutralMetal;
            }
            else if (entityLine == "neutralCrystalResourceAsteroidSetup")
            {
                CurrentAsteroidType = AsteroidType.NeutralCrystal;
            }
        }

        public override string ExportToCSV()
        {

            return $@"{EntityName},{Name},{PlanetType},{IsColonizable},{MetalAsteroids.ExportToCSV()},{CrystalAsteroids.ExportToCSV()},{Population},{GrowthRate},{MaxSocialPopulationBonus},{MaxTotalPopulation}," +
                        $"{MaxSocialPopGrowthBonus},{MaxSocialCultureBonus},{MaxSocialTradePenalty},{MaxSocialShipBuildPenalty},{MaxIndustryTradeBonus},{MaxIndustryShipBuildBonus},{MaxIndustryPopulationPenalty}," +
                        $"{MaxIndustryPopGrowthPenalty},{MaxIndustryCulturePenalty},{MaxSmugglingAmount},{MaxSmugglingCorruption},{MaxCivilianModules},{MaxTacticalModules},{MaxStarbases},{MaxSpaceMines}," +
                        $"{MaxHealth},{HealthRegenRate},{NumPlanetBonuses},{HomePlanetTaxBonus},{HomePlanetMetalBonus},{HomePlanetCrystalBonus},{HyperspaceArea},{MoveArea}," +
                        $"{RuinPlanetType},{ExtraScuttleTimeOnRuin},{CreditsOnRuin},{MetalOnRuin},{CrystalOnRuin}";
        }

        public class AsteroidSetup
        {
            private static Dictionary<string, PropertyInfo> _linePropertyMap;
            public Dictionary<string, PropertyInfo> LinePropertyMap
            {
                get
                {
                    if (_linePropertyMap == null || _linePropertyMap.Count == 0)
                    {
                        _linePropertyMap = Helpers.BuildLinePropertyMap(typeof(AsteroidSetup));
                    }
                    return _linePropertyMap;
                }
            }

            public string ExportToCSV()
            {
                return $"{ExtractionRate},{MinCount},{MaxCount},{RefineryRate},{MaxRefineryShipCount}";
            }

            [EntityLine("minCount")]
            public byte MinCount { get; set; }

            [EntityLine("maxCount")]
            public byte MaxCount { get; set; }

            [EntityLine("extractionRate")]
            public decimal ExtractionRate { get; set; }

            [EntityLine("refineryRate")]
            public decimal RefineryRate { get; set; }

            [EntityLine("maxRefineryCount")]
            public byte MaxRefineryShipCount { get; set; }

            public void LoadEntityValue(string entityLine, string value)
            {
                LinePropertyMap.TryGetValue(entityLine, out PropertyInfo property);
                if (property != null)
                {
                    property.SetValue(this, Convert.ChangeType(Helpers.SanitizeValue(property.PropertyType, value), property.PropertyType));
                }
            }
        }
    }

    public enum AsteroidType
    {
        Metal,
        Crystal,
        NeutralMetal,
        NeutralCrystal
    }
}
