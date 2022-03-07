using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public interface IAbstractGameEvent
    {

    }
    public class AbstractGameEvent
    {
        private DateTime timestamp = DateTime.Now;
        private DateTime timestampUtc = DateTime.UtcNow;
        private String eventName;
        public string EventName { get => eventName; set => eventName = value; }
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        public DateTime TimestampUts { get => timestampUtc; set => timestampUtc = value; }

        public AbstractGameEvent()
        {

        }
    }



}
