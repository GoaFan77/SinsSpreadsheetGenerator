using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SinsSpreadsheetGenerator.Interfaces
{
    public interface ILinePropertyMapped
    {
        Dictionary<string, PropertyInfo> LinePropertyMap { get; }
    }
}
