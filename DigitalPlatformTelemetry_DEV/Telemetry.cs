

namespace DigitalPlatformTelemetry_DEV
{
    public class Telemetry
    {
        public Telemetry(string id, string version, string logtype, string appSource, string instant, string payload)
        {
            this.Id = id;
            this.Version = version;
            this.LogType = logtype;
            this.AppSource = appSource;
            this.Instant = instant;
            this.Payload = payload;
        }

        public string Id { get; set; }
        public string Version { get; set; }
        public string LogType { get; set; }
        public string AppSource { get; set; }
        public string Instant { get; set; }
        public string Payload { get; set; }
    }
}
