using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otoface
{
    public struct KeyEvent
    {
        public int Frame { get; set; }
        public string Key { get; set; }
        public string EventType { get; set; } // "ON" or "OFF"

        public KeyEvent(int frame, string key, string eventType)
        {
            Frame = frame;
            Key = key;
            EventType = eventType;
        }
    }
}