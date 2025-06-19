using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebelAlliance.TelnetDemo.Interfaces
{
    internal interface ITelnetFilter
    {
        byte[] FilterCommands(byte[] data, int length);
    }
}
