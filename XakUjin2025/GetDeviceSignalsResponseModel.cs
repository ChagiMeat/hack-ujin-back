using Newtonsoft.Json;

namespace XakUjin2025
{
    public class GetDeviceSignalsResponseModel
    {
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public SignalData Data { get; set; }

        [JsonProperty("connection")]
        public ConnectionInfo Connection { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("fromdomain")]
        public string FromDomain { get; set; }

        [JsonProperty("worktime")]
        public string WorkTime { get; set; }

    }

    public class SignalData
    {
        [JsonProperty("apartment")]
        public ApartmentInfo Apartment { get; set; }

        [JsonProperty("signals")]
        public Dictionary<string, List<SignalItem>> Signals { get; set; }
    }

    public class ApartmentInfo
    {
        [JsonProperty("apartment_id")]
        public string ApartmentId { get; set; }

        [JsonProperty("apartment_title")]
        public string ApartmentTitle { get; set; }
    }

    public class SignalItem
    {
        [JsonProperty("intensity")]
        public double Intensity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("signal_label")]
        public string SignalLabel { get; set; }
    }

    public class ConnectionInfo
    {
        [JsonProperty("server_real_ip")]
        public string ServerRealIp { get; set; }

        [JsonProperty("user_ip")]
        public string UserIp { get; set; }
    }
}
