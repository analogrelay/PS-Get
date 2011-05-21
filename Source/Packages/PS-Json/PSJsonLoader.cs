using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PsJson {
    public static class PSJsonLoader {
        public static PSObject ConvertToPSObject(JObject jobj) {
            // I want to be able to make the JObject a base class
            // But that causes an error about duplicate "Value" members :(
            PSObject obj = new PSObject();

            // Copy JSON properties to PSObject members
            foreach (JProperty prop in jobj.Properties()) {
                object value = ConvertValue(prop.Value);
                if (value != null) {
                    obj.Members.Add(new PSNoteProperty(prop.Name, value));
                }
            }

            return obj;
        }

        public static JObject ConvertToJObject(PSObject pshobj) {
            JObject obj = new JObject();
            foreach (PSPropertyInfo member in pshobj.Properties) {
                obj.Add(member.Name, ConvertValue(member));
            }
            return obj;
        }

        private static JToken ConvertValue(PSPropertyInfo member) {
            object value = member.Value;
            Type valueType = value.GetType();
            if (valueType == typeof(String) || valueType.IsValueType || valueType == typeof(DBNull) || valueType.IsEnum) {
                return new JValue((string)value);
            }
            else if (valueType == typeof(PSObject)) {
                return ConvertToJObject((PSObject)value);
            }
            return new JObject(value);
        }

        private static object ConvertValue(JToken token) {
            switch (token.Type) {
                case JTokenType.Object:
                    return ConvertToPSObject((JObject)token);
                case JTokenType.Array:
                    return ConvertJArray((JArray)token);
                default:
                    JValue val = token as JValue;
                    if (val != null) {
                        return val.Value;
                    }
                    return null;
            }
        }

        private static object ConvertJProperty(JProperty prop) {
            return prop.Value;
        }

        private static object ConvertJArray(JArray array) {
            return array.Select(ConvertValue).ToArray();
        }
    }
}
