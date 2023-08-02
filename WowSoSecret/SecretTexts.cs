using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowSoSecret
{
    public class SecretTexts
    {
        public List<string> Editor { get; set; }
        public List<string> Playing { get; set; }
        public List<string> Results { get; set; }

        public static SecretTexts Default() =>
            new SecretTexts
            {
                Editor = new List<string>
                {
                    "Secret Mode Enabled!",
                },
                Playing = new List<string>
                {
                    "Secret Mode Enabled!",
                },
                Results = new List<string>
                {
                    "Secret Mode Enabled!",
                }
            };

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static SecretTexts FromJson(string json) => JsonConvert.DeserializeObject<SecretTexts>(json) ?? Default();
    }
}
