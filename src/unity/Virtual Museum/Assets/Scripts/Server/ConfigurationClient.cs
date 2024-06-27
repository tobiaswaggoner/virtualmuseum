using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Server
{
    public class ConfigurationClient
    {
        private readonly string apiUrl;
        private readonly string apiToken;

        public ConfigurationClient(string apiUrl, string apiToken)
        {
            this.apiUrl = apiUrl;
            this.apiToken = apiToken;
        }

        
        public async Task<TopographicalTableConfiguration> GetTableConfiguration(Guid id)
        {
            return await MakeRequest<TopographicalTableConfiguration>($"{apiUrl}topographical-table/{id}");
        }
        
        public async Task<Room> GetRoom(Guid id)
        {
            return await MakeRequest<Room>($"{apiUrl}rooms/{id}");
        }

        public async Task<List<Room>> GetRooms()
        {
            return await MakeRequest<List<Room>>($"{apiUrl}rooms");
        }

        private async Task<T> MakeRequest<T>(string url) where T : class
        {
            using var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer {apiToken}");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                return null;
            }

            var json = request.downloadHandler.text;
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}