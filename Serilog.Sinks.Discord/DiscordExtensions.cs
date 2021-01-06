using Serilog.Configuration;
using Serilog.Events;

using System;
using System.Drawing;

namespace Serilog.Sinks.Discord
{
    public static class DiscordExtensions
    {
        public static LoggerConfiguration Discord(this LoggerSinkConfiguration loggerConfiguration, string discordWebhookUrl, string userName = null, LogEventLevel logEventLevel = LogEventLevel.Warning, IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new Discord(discordWebhookUrl, userName, logEventLevel, formatProvider));
        }

        public static uint ToUint(this Color c)
        {
            return ((uint)c.R << 16) | ((uint)c.G << 8) | (uint)c.B;
        }
    }
}
