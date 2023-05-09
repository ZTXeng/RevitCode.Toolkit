using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    public static class DocumentCollectionExtension
    {
        public static IEnumerable<Element> CollectorOfSchema<TSchema>(this Document doc)where TSchema : ISerializable
        {
            var attrubute = typeof(TSchema).GetCustomAttribute<SchemaFormatAttribute>();

            if (attrubute != null) { throw new InvalidOperationException("Please Write schema format Attribute"); }

            return doc.CollectorOfSchema(attrubute.SchemaId);
        }


        public static IEnumerable<Element> CollectorOfSchema(this Document doc,Guid guid)
        {
            return new FilteredElementCollector(doc).WherePasses(new ExtensibleStorageFilter(guid));
        }
    }
}
