using System;

namespace SinsSpreadsheetGenerator.EntityClasses
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    internal class EntityLineAttribute : Attribute
    {
        public string EntityLine { get; set; }

        public EntityLineAttribute(string entityLine)
        {
            EntityLine = entityLine;
        }
    }
}