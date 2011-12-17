using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Xunit;
using System.Linq.Expressions;

namespace PsGet.Facts
{
    public static class CmdletAssert
    {
        public static void IsCmdlet(Type t, string verb, string noun)
        {
            Type current = t;
            bool match = false;
            while (!match && current != typeof(object))
            {
                match = current == typeof(Cmdlet);
                current = current.BaseType;
            }
            Assert.True(match);
            AttributeAssert.Has(
                t,
                new CmdletAttribute(verb, noun));
        }

        public static void IsParameter(Expression<Func<object>> member)
        {
            IsParameter(member, new ParameterAttribute());
        }

        public static void IsParameter(Expression<Func<object>> member, ValidateArgumentsAttribute validator)
        {
            IsParameter(member, new ParameterAttribute(), validator);
        }

        public static void IsParameter(Expression<Func<object>> member, ParameterAttribute param)
        {
            IsParameter(member, param, validator: null);
        }

        public static void IsParameter(Expression<Func<object>> member, ParameterAttribute param, ValidateArgumentsAttribute validator)
        {
            AttributeAssert.Has(member, param);
            if (validator != null)
            {
                AttributeAssert.Has(member, validator);
            }
        }
    }
}
