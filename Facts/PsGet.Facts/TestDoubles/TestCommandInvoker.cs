using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Facts.TestDoubles
{
    // Test Doubles are tested via their use in unit tests
    [ExcludeFromCodeCoverage]
    public class TestCommandInvoker : ICommandInvoker
    {
        private static object VoidObject = new object();
        private static IEnumerable<object> Void = new object[] { VoidObject };
        private Dictionary<string, Handler> _scripts = new Dictionary<string, Handler>();
        private bool _autoHandleVoids = false;

        public TestCommandInvoker(bool autoHandleVoids)
        {
            _autoHandleVoids = autoHandleVoids;
        }

        public void RegisterScript(string script)
        {
            RegisterScript(script, () => Void);
        }

        public void RegisterScript(string script, Action handler)
        {
            _scripts[script] = new Handler(() =>
            {
                handler();
                return Void;
            });
        }

        public void RegisterScript(string script, Func<IEnumerable<object>> handler)
        {
            _scripts[script] = new Handler(handler);
        }

        public void InvokeScript(string script)
        {
            IEnumerable<object> ret = InvokeScript<object>(script);
            if (!ReferenceEquals(ret.Single(), TestCommandInvoker.VoidObject))
            {
                throw new InvalidOperationException("Registered handler returned a value but was called from InvokeScript overload which does not return a value");
            }
        }

        public IEnumerable<T> InvokeScript<T>(string script)
        {
            return InvokeScriptCore(script, o => (T)o);
        }

        public IEnumerable<T> InvokeScript<T>(string script, Func<object, T> converter)
        {
            return InvokeScriptCore(script, converter);
        }

        public int GetInvokeCount(string script)
        {
            return _scripts[script].InvokeCount;
        }

        private IEnumerable<T> InvokeScriptCore<T>(string script, Func<object, T> converter)
        {
            Handler handler;
            if (!_scripts.TryGetValue(script, out handler))
            {
                if (_autoHandleVoids)
                {
                    handler = CreateAutoHandler();
                    _scripts[script] = handler;
                }
                else
                {
                    throw new InvalidOperationException("No handler registered for script: " + script);
                }
            }
            return handler.Invoke().Select(converter);
        }

        private Handler CreateAutoHandler()
        {
            return new Handler(() => Void);
        }

        private class Handler
        {
            public int InvokeCount { get; set; }
            public Func<IEnumerable<object>> Implementation { get; set; }

            public Handler(Func<IEnumerable<object>> impl)
            {
                Implementation = impl;
                InvokeCount = 0;
            }

            public IEnumerable<object> Invoke()
            {
                InvokeCount++;
                return Implementation();
            }
        }
    }
}
