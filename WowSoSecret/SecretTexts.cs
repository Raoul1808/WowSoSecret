using System.Collections.Generic;
using Newtonsoft.Json;

namespace WowSoSecret
{
    public class SecretTexts
    {
        public List<string> Editor { get; set; }
        public List<string> Playing { get; set; }
        public List<string> Results { get; set; }

        public const string DefaultText = "Secret Mode Enabled!";

        public static SecretTexts Default() =>
            new SecretTexts
            {
                Editor = new List<string>
                {
                    DefaultText,
                },
                Playing = new List<string>
                {
                    DefaultText,
                },
                Results = new List<string>
                {
                    DefaultText,
                }
            };

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static SecretTexts FromJson(string json) => JsonConvert.DeserializeObject<SecretTexts>(json) ?? Default();
    }
}
