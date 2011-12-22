using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Abstractions
{
    public interface ISessionStore
    {
        object Get(string variableName);
        void Set(string variableName, object value);
    }
}
