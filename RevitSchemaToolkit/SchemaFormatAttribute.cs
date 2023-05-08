using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SchemaFormatAttribute:Attribute
    {
        public Guid SchemaId { get; set; }

        public SchemaFormatAttribute(Guid id)
        {
            SchemaId = id;
        }

        public SchemaFormatAttribute(string id) : this(Guid.Parse(id)) { }
    }
}
