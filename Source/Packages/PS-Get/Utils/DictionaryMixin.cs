using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Utils {
    public static class DictionaryMixin {
        public static T TryGet<T>(this IDictionary<string, object> self, string key) {
            return self.TryGet(key, default(T));
        }

        public static T TryGet<T>(this IDictionary<string, object> self, string key, T def) {
            return self.TryGet(key, () => def);
        }

        public static T TryGet<T>(this IDictionary<string, object> self, string key, Func<T> def) {
            object val;
            if (!self.TryGetValue(key, out val) || !(val is T)) {
                return def();
            }
            return (T)val;
        }
    }
}
