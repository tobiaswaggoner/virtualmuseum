using UnityEngine;

namespace Server
{
    public class ConfigurationManager : MonoBehaviour
    {
        [SerializeField] private string apiUrl = "http://http://timeglide.xr-ai.de:5000/api/";
        [SerializeField] private string apiToken = "your-api-token";

        private void Awake()
        {
            ConfigurationClient = new ConfigurationClient(apiUrl, apiToken);
        }

        public ConfigurationClient ConfigurationClient { get; private set; }
    }
}