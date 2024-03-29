﻿using McMaster.Extensions.CommandLineUtils;
using SinsSpreadsheetGenerator.EntityClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SinsSpreadsheetGenerator
{
    [Command(Name = "SinsSpreadsheetGenerator", Description = "A tool that parses entity files and produces a spreadsheet with stat data on each entity.")]
    [HelpOption("-?")]
    class SinsSpreadsheetGeneratorMain
    {
        const string CSV_UNIT_HEADER = "Entity Name,Name,Type,StatCountType,Armor Type,Fleet Supply (FS),Credits,Credits/FS,Metal,Metal/FS,Crystal,Crystal/FS,Build Time,Build Time/FS,XP,XP/FS,Hull,Hull/FS,Hull Restore," +
            "Hull Restore/FS,Shields,Shields/FS,Shield Restore,Shield Restore/FS,Armor,Total Surviveability,Total Surviveability/FS,Antimatter,Antimatter/FS,Antimatter Restore, Antimatter Restore/FS," +
            "DPS,DPS/FS,Max Single Target DPS,Max Single Target DPS/FS,Max Range,Bombing DPS,Bombing DPS/FS,Population Killed per Second,Population Killed per Second/FS,Command Points,Command Points/FS,Max Speed";

        const string CSV_PLANET_HEADER = "Entity Name,Name,Type,Colonizable,Metal Extraction Rate,Min Metal Asteroids,Max Metal Asteroids,Metal Refinery Rate,Max Metal Refinery Ships,Crystal Extraction Rate,Min Crystal Asteroids,Max Crystal Asteroids,Crystal Refinery Rate,Max Crystal Refinery Ships," +
            "Population,Growth Rate,Social Population,Max Total Population,Social Growth Bonus,Social Culture Bonus,Social Trade Penalty,Social Build Penalty,Industry Trade Bonus,Industry Build Bonus,Industry Population Penalty,Industry Growth Penalty,Industry Culture Penalty," +
            "Smuggling %,Smuggling Corruption,Civilian Modules,Tactical Modules,Starbases,Space Mines,Health,Health Regen,Num Planet Bonuses,Home Tax Bonus,Home Metal Bonus, Home Crystal Bonus,Hyperspace Area,Move Area,Ruin Planet Type,Extra Ruin Scuttle Time," +
            "Ruin Credits,Ruin Metal,Ruin Crystal";

        public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<SinsSpreadsheetGeneratorMain>(args);

        private string _OutputFile;
        [Argument(0, Description = "Location to save the generated file. Defaults to current directory.")]
        private string OutputFile
        {
            get
            {
                return _OutputFile;
            }
            set
            {
                if (value.Substring(value.Length - 4, 4) != ".csv")
                {
                    _OutputFile = value + ".csv";
                } 
                else
                {
                    _OutputFile = value;
                }
            }
        }

        [Option("-d|--Directories", CommandOptionType.MultipleValue, Description = "File Path of a mod's core directory. Can be specified multiple times to combine results of multiple mods.")]
        private string[] ModDirectories { get; set; } = { System.Reflection.Assembly.GetExecutingAssembly().Location };

        [Option("-f|--Filter", Description = "A filter string that limits results to entity files that have the filter in their file name.")]
        private string Filter { get; set; }

        [Option("-s|--Sort", Description = "Property to sort initial order of rows. 0 = Type then StatCountType, 1 = Name, 2 = Type, 3 = StatCountType")]
        private SortOption Sort { get; set; } = SortOption.TypeAndStatCountType;

        [Option("-t|--Types", Description = "Property to limit the types of entities returned. Must use the same spreadsheet type. Defaults to all units. ")]
        private EntityType[] Types { get; set; } = new EntityType[] { EntityType.Frigate, EntityType.CapitalShip, EntityType.Titan};

        private ConcurrentDictionary<string, Entity> Entities = new ConcurrentDictionary<string, Entity>();

        private async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            try
            {
                if (Types.Contains(EntityType.Planet) && Types.Length > 1)
                {
                    throw new ArgumentException("Cannot output Planet Entity Types with other types.");
                }

                List<Task> modTasks = new List<Task>();
                Console.WriteLine($@"Processing {ModDirectories.Length} Mods.");
                short modOrder = 0; // A lower mod order gives priority to that mod's data.
                foreach (string mod in ModDirectories)
                {
                    modOrder++;
                    var localModOrder = modOrder; // Needs a local variable to ensure the tasks starts before modOrder is incremented again.
                    modTasks.Add(Task.Run(() => ProcessMod(mod, localModOrder)));
                }

                await Task.WhenAll(modTasks.ToArray());
                    
                Console.WriteLine($@"Found {Entities.Count} entities");
                List<Entity> sortedEntities = GetSortedEntities();

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(OutputFile))
                {
                    // First write CSV Header, then output each entity.
                    file.WriteLine(Types.Contains(EntityType.Planet) ? CSV_PLANET_HEADER : CSV_UNIT_HEADER);
                    sortedEntities.ForEach(e => file.WriteLine(e.ExportToCSV()));
                }
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"An error has occurred. {ex.Message}");
                return 0;
            }

            return 1;
        }

        private async Task ProcessMod(string mod, short modOrder)
        {
            var modStringDictionary = Helpers.BuildStringMap(mod + @"/String/English.str");
            string[] entityFiles = Directory.GetFiles(mod + "/GameInfo", "*" + Filter + "*.entity");
            foreach (var entity in entityFiles)
            {
                Entity entityObject = await new EntityReader(entity).BuildEntity(Types, modStringDictionary);
                if (entityObject != null)
                {
                    entityObject.ModOrder = modOrder;
                    // Add the new entity if it does not exist in the dictionary. If it does exist, update it if the new Entity has a smaller (higher priority) mod order.
                    Entities.AddOrUpdate(entity.Split('\\').Last(), entityObject, (key, oldEntity) => oldEntity.ModOrder > entityObject.ModOrder ? entityObject : oldEntity);
                }
            }
        }

        private List<Entity> GetSortedEntities()
        {
            switch(Sort)
            {
                case SortOption.TypeAndStatCountType:
                    return Entities.Values.OrderBy(e => e.Type).ThenBy(e => e.StatCountType).ToList();
                case SortOption.Name:
                    return Entities.Values.OrderBy(e => e.EntityName).ToList();
                case SortOption.Type:
                    return Entities.Values.OrderBy(e => e.Type).ToList();
                case SortOption.StatCountType:
                    return Entities.Values.OrderBy(e => e.StatCountType).ToList();
                default:
                    return Entities.Values.OrderBy(e => e.Type).ThenBy(e => e.StatCountType).ToList();
            }            
        }
    }
}
