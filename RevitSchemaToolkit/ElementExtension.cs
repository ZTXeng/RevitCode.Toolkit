using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    public static class ElementExtension
    {
        public static void WriteSchema<TSchema>(this Element element, TSchema schema) where TSchema : ISerializable
        {
            var entity = CreateSchemaEntity(schema.GetType(), "BINARYT_DATA");

            var formatter = new BinaryFormatter();

            using (var steam = new MemoryStream())
            {
                formatter.Serialize(steam, schema);
                entity.Set<IList<byte>>("BINARYT_DATA", steam.ToArray());
            }

            element.SetEntity(entity);
        }

        public static TSchema ReadSchema<TSchema>(this Element element) where TSchema : ISerializable
        {
            var attribute = typeof(TSchema).GetCustomAttribute<SchemaFormatAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException("Please Write foramt Attriubute");
            }

            var schema = Schema.Lookup(attribute.SchemaId);

            if (schema == null)
            {
                return default;
            }

            var entity = element.GetEntity(schema);

            if (entity.Schema == null)
            {
                return default;
            }

            var array = entity.Get<IList<byte>>("BINARYT_DATA").ToArray();

            var formatter = new BinaryFormatter()
            {
                Binder = new TypeBinder(typeof(TSchema))
            };

            using (var steam = new MemoryStream(array))
            {
                return formatter.Deserialize(steam) is TSchema result ? result : default;
            }

        }

        private static Entity CreateSchemaEntity(Type type, string name)
        {
            var attribute = type.GetCustomAttribute<SchemaFormatAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException("Please Write foramt Attriubute");
            }

            var schema = Schema.Lookup(attribute.SchemaId);
            if (schema == null)
            {
                using (var builder = new SchemaBuilder(attribute.SchemaId))
                {
                    builder.SetReadAccessLevel(AccessLevel.Public);
                    builder.SetWriteAccessLevel(AccessLevel.Public);
                    builder.SetSchemaName(type.FullName.Replace(".", "_"));

                    builder.AddArrayField(name, typeof(byte));

                    schema = builder.Finish();
                }
            }

            return new Entity(schema);
        }
    }
}
