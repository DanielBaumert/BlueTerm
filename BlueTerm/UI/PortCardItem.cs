using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueTerm.UI
{
        
    public struct PortItem
    {
        public Type Type;
        public string Name;

        public static bool operator ==(PortItem left, PortItem right) => left.Type == right.Type && left.Name == right.Name; 
        public static bool operator !=(PortItem left, PortItem right) => left.Type != right.Type || left.Name != right.Name;
    }
}
