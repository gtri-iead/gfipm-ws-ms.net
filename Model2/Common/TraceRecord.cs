using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace Common
{
    public abstract class TraceRecord
    {
        // Fields
        internal const string _eventId = "http://schemas.microsoft.com/2009/06/IdentityModel/EmptyTraceRecord";
        protected const string ElementName = "TraceRecord";
        protected const string EventIdBase = "http://schemas.microsoft.com/2009/06/IdentityModel/";

        // Methods
        protected TraceRecord()
        {
        }

        public abstract void WriteTo(XmlWriter writer);

        // Properties
        public virtual string EventId
        {
            get
            {
                return "http://schemas.microsoft.com/2009/06/IdentityModel/EmptyTraceRecord";
            }
        }
    }
}
