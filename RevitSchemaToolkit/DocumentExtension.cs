using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    public static class DocumentExtension
    {
        public static  void  WriteSchema(this Document doc,ISerializable schema)
        {
            using (var storage = DataStorage.Create(doc) )
            {
                storage.WriteSchema(schema);
            }
        }

        public static TSchema ReadSchema<TSchema>(this Document doc)where TSchema : ISerializable
        {
            TSchema schema = default;

            var elements = doc.CollectorOfSchema<TSchema>();

            foreach ( var element in elements )
            {
                if(element is DataStorage)
                {
                    schema = element.ReadSchema<TSchema>();
                }
            }

            return schema;
        }
    }
}
