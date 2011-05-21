using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Helper {
    public static class EnumerableMixin {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> self, T item) {
            foreach (T existing in self) {
                yield return existing;
            }
            yield return item;
        }
    }
}
