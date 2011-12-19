using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Abstractions;

namespace PsGet.Facts.TestDoubles
{
    public class TestSessionStore : ISessionStore
    {
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public object Get(string variableName)
        {
            object val;
            if (!_values.TryGetValue(variableName, out val))
            {
                return null;
            }
            return val;
        }

        public void Set(string variableName, object value)
        {
            _values[variableName] = value;
        }
    }
}
