using System;

namespace BattleRoyalleWindowsService
{
    internal class Maquina
    {
        public string IP { get; set; }
        public string Nome { get; set; }
        public string AntiVirus { get; internal set; }
        public bool Firewall { get; internal set; }
        public string VersaoWindows { get; internal set; }
        public string VersaoDotNeto { get; internal set; }
    }
}