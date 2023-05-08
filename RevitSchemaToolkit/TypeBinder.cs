using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    public class TypeBinder : SerializationBinder
    {
        private readonly Type _type;
        public TypeBinder(Type type) => _type = type;
        public override Type BindToType(string assemblyName, string typeName)
        {
            return _type.Assembly.GetType(typeName);
        }
    }
}
