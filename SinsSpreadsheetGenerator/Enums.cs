﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SinsSpreadsheetGenerator
{
    public enum EntityType
    {
        [Description("Frigate")]
        Frigate,
        [Description("CapitalShip")]
        CapitalShip,
        [Description("Titan")]
        Titan,
        [Description("Structure")]
        Structure,
        [Description("Fighter")]
        Fighter,
        [Description("Squad")]
        Squad
    }

    public enum SortOption
    {
        TypeAndStatCountType,
        Name,
        Type,
        StatCountType
    }
}
