
using Newtonsoft.Json;

namespace DigitalPlatformTelemetry_DEV
{
    public class TelemetryOut
    {
        public TelemetryOut(Telemetry telemetry)
        {
            this.Id = telemetry.Id;
            this.Version = telemetry.Version;
            this.LogType = telemetry.LogType;
            this.AppSource = telemetry.AppSource;
            this.Instant = telemetry.Instant;
            this.Payload = DeserializePayload(telemetry.Payload);
        }

        public string Id { get; set; }
        public string Version { get; set; }
        public string LogType { get; set; }
        public string AppSource { get; set; }
        public string Instant { get; set; }
        public object Payload { get; set; }

        public object DeserializePayload(string payload) {

            try { return JsonConvert.DeserializeObject<object>(payload); }
            catch { return payload; }
        }
    }


}
