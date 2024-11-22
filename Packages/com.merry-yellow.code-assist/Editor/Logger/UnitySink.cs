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
    public class UnityOutputWindowSink : ILogEventSink
    {
        private readonly IFormatProvider? _formatProvider;

        public UnityOutputWindowSink(IFormatProvider? formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);

            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                case LogEventLevel.Information:
                    UnityEngine.Debug.Log(message);
                    break;
                case LogEventLevel.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    UnityEngine.Debug.LogError(message);
                    break;
                default:
                    break;
            }
        }
    }

}