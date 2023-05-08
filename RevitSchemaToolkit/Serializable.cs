using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RevitSchemaToolkit
{
    public static class Serializable
    {
        public static void DeserializeModel<TModel>(TModel model, SerializationInfo info) where TModel : ISerializable
        {
            var properties = model.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance
                ).ToList();

            foreach (var property in properties)
            {
                var value = info.GetValue(property.Name,property.PropertyType);
                property.SetValue(model, value);
            }

        }

        public static void SerializeModel<TModel>(TModel model,SerializationInfo info,Attribute attribute)where TModel : ISerializable
        {
            var properties = model.GetType().GetProperties(
                BindingFlags.Public|BindingFlags.Instance
                ).ToList();

            if (attribute != null)
            {
                properties = properties.Where(x =>
                {
                    return x.IsDefined(attribute.GetType()) == false;

                }).ToList();
            }


            foreach ( var property in properties)
            {
                info.AddValue(property.Name, property.GetValue(model));
            }
        }


    }
}
