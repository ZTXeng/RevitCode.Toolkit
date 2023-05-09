using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    public  class SchemaModelBase : ISerializable
    {
        public SchemaModelBase() { }

        public SchemaModelBase(SerializationInfo info, StreamingContext context) {
            Serializable.DeserializeModel(this, info);
        }

        public virtual void GetObjectData(SchemaModelBase model,Attribute attribute, SerializationInfo info, StreamingContext context)
        {
            Serializable.SerializeModel(this, info, attribute);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(this, null,info,context);
        }
    }
}
