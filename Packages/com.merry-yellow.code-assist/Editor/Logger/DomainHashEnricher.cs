using System;
using System.Linq;
using Meryel.UnityCodeAssist.Serilog;
using Meryel.UnityCodeAssist.Serilog.Core;
using Meryel.UnityCodeAssist.Serilog.Events;
using Meryel.UnityCodeAssist.Serilog.Configuration;


#pragma warning disable IDE0005
using Serilog = Meryel.UnityCodeAssist.Serilog;
#pragma warning restore IDE0005


#nullable enable


namespace Meryel.UnityCodeAssist.Editor.Logger
{
    public class DomainHashEnricher : ILogEventEnricher
    {
        static readonly int domainHash;

        static DomainHashEnricher()
        {
            var guid = UnityEditor.GUID.Generate();
            domainHash = guid.GetHashCode();
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "DomainHash", domainHash));
        }
    }

}