using SinsSpreadsheetGenerator.EntityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SinsSpreadsheetGenerator
{
    /// <summary>
    /// Builds a new appropriate object for an Entity Type.
    /// </summary>
    public static class EntityFactory
    {
        public static Entity GetEntity(string entityType, string fileName)
        {
            fileName = fileName.Split('\\').Last();
            if (Enum.TryParse(typeof(EntityType), entityType, out object type))
            {
                switch ((EntityType)type)
                {
                    case EntityType.Frigate:
                        return new Frigate(fileName);
                    case EntityType.CapitalShip:
                        return new Capitalship(fileName);
                    case EntityType.Titan:
                        return new Titan(fileName);
                    case EntityType.Planet:
                        return new Planet(fileName);
                }
            }

            return null;
        }
    }
}
