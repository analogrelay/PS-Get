using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PsGet.Abstractions
{
    public class PSVariableSessionStorage : ISessionStorage
    {
        private PSVariableIntrinsics _vars;

        public PSVariableSessionStorage(PSVariableIntrinsics vars)
        {
            _vars = vars;
        }

        public object GetValue(string name)
        {
            return _vars.GetValue(name);
        }

        public void SetValue(string name, object value)
        {
            _vars.Set(name, value);
        }
    }
}
