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
        /// <summary>
        /// 写入Schema数据
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="element"></param>
        /// <param name="schema"></param>
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
        /// <summary>
        /// 写入多个Schema数据
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="element"></param>
        /// <param name="schemas"></param>
        public static void WriteSchemas<TSchema>(this Element element, params TSchema[] schemas) where TSchema : ISerializable
        {
            var entity = CreateSchemaEntity(schemas.GetType(), "BINARYT_DATA");

            var formatter = new BinaryFormatter();

            using (var steam = new MemoryStream())
            {
                formatter.Serialize(steam, schemas);
                entity.Set<IList<byte>>("BINARYT_DATA", steam.ToArray());
            }

            element.SetEntity(entity);
        }
        /// <summary>
        /// 读取Schema数据
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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
        /// <summary>
        /// 读取多个Schema数据
        /// </summary>
        /// <typeparam name="TSchema"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TSchema[] ReadSchemas<TSchema>(this Element element) where TSchema : ISerializable
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
                return formatter.Deserialize(steam) is TSchema[] result ? result : default;
            }
        }

        public static void CreateSchema<TSchema>(this Element element ,TSchema schema) where TSchema : SchemaModelBase
        {
            var schemas = element.ReadSchemas<SchemaModelBase>();

            if (schemas == null)
            {
                schemas = new SchemaModelBase[] { schema };
            }
            else
            {
                var index = FindIndex(schemas, schema.GetType());

                if (index >= 0)
                {
                    schemas[index] = schema;
                }
                else
                {
                    schemas =  schemas.Append(schema).ToArray(); 
                }

            }

            element.WriteSchemas(schemas);  
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

        private static int FindIndex(ISerializable[] schemas,Type type)
        {
            for (int i = 0; i < schemas.Length; i++)
            {
                if (schemas[i].GetType() == type)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
