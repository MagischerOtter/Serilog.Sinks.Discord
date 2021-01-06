using Newtonsoft.Json;

using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Specialized;
using System.Drawing;

namespace Serilog.Sinks.Discord
{
    public class Discord : ILogEventSink
    {
        private readonly LogEventLevel _logEventLevel;
        private readonly string _discordWebhookUrl;
        private readonly IFormatProvider _formatProvider;
        private readonly string _userName;
        public Discord(string discordWebhookUrl, string userName, LogEventLevel logEventLevel, IFormatProvider formatProvider)
        {
            _discordWebhookUrl = discordWebhookUrl;
            _formatProvider = formatProvider;
            _userName = userName;
            _logEventLevel = logEventLevel;
            if (logEventLevel < LogEventLevel.Warning) _logEventLevel = LogEventLevel.Warning;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level < _logEventLevel) return;

            var message = logEvent.RenderMessage(_formatProvider);
            var exceptionString = logEvent.Exception?.ToString() ?? "";
            var payload = new Payload(_userName, logEvent.Level, message, "Exception", exceptionString);
            var payloadString = JsonConvert.SerializeObject(payload);
            var content = new NameValueCollection()
            {
                {
                    "payload_json",
                    payloadString
                }
            };

            DiscordHttp.Post(_discordWebhookUrl, content);
        }
    }

    public class Payload
    {
        [JsonProperty("username")]
        public string _userName;
        [JsonProperty("embeds")]
        public Embed[] Embeds;

        public Payload(string userName, LogEventLevel title, string description, string fieldName, string fieldValue)
        {
            _userName = userName;
            
            Color? color = title switch
            {
                LogEventLevel.Warning => Color.FromArgb(255, 101, 101),
                LogEventLevel.Error => Color.FromArgb(216, 46, 46),
                LogEventLevel.Fatal => Color.FromArgb(158, 10, 10),
                _ => null
            };

            var colorValue = color.Value.ToUint();

            if (string.IsNullOrWhiteSpace(fieldValue)) Embeds = new Embed[] { new Embed { Titel = title.ToString(), Description = description, Color = colorValue } };
            else Embeds = new Embed[] { new Embed { Titel = title.ToString(), Description = description, EmbedFields = new EmbedField[] { new EmbedField(fieldName, fieldValue) } } };
        }
    }

    public class Embed
    {
        [JsonProperty("title")]
        public string Titel;
        [JsonProperty("description")]
        public string Description;
        [JsonProperty("fields")]
        public EmbedField[] EmbedFields;
        [JsonProperty("color")]
        public uint Color;
    }

    public class EmbedField
    {
        [JsonProperty("Name")]
        public string _name;
        [JsonProperty("value")]
        public string _value;
        
        public EmbedField(string name, string value)
        {
            _name = name;
            _value = value;
        }
    }
}
