using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Hosting
{
    public interface IHostEnvironment
    {
        string ModuleBase { get; }
    }
}
