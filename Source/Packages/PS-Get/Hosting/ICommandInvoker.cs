using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Hosting
{
    public interface ICommandInvoker
    {
        void InvokeScript(string script);
        IEnumerable<T> InvokeScript<T>(string script);
        IEnumerable<T> InvokeScript<T>(string script, Func<object, T> converter);
    }
}
