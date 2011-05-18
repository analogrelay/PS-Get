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
            PSObject obj = new PSObject();

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
            Type valueType = member.Value.GetType();
            if (valueType == typeof(String) || valueType.IsValueType || valueType == typeof(DBNull) || valueType.IsEnum) {
                return new JValue((string)member.Value);
            }
            else if (valueType == typeof(PSObject)) {
                return ConvertToJObject((PSObject)member.Value);
            }
            return new JObject(member.Value);
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
