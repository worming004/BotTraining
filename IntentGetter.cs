using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;
using System.Collections.Generic;

namespace BotTraining
{
    public interface IIntentGetter
    {
        Task<Intent> GetIntent(string message);
    }

    public struct Intent
    {
        public string Name { get; set; }
        public float Score { get; set; }
        public bool IsHello => this.Name.Equals("Hello", StringComparison.OrdinalIgnoreCase);
        public bool IsHelp => this.Name.Equals("Help", StringComparison.OrdinalIgnoreCase);
    }

    internal class LuisIntentGetter : IIntentGetter
    {
        private JsonSerializerOptions luisSerializationOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public class IntentResponse
        {
            public string Query { get; set; }
            public Prediction Prediction { get; set; }

            public Intent ToIntent()
            {
                return new Intent { Name = this.Prediction.TopIntent, Score = GetTopIntentScore };
            }

            private float GetTopIntentScore => Prediction.Intents[Prediction.TopIntent].Score;
        }
        public class Prediction
        {
            public string TopIntent { get; set; }
            public Dictionary<string, IntentScore> Intents { get; set; } = new Dictionary<string, IntentScore>();
        }
        public class IntentScore
        {
            public float Score { get; set; }
        }
        private string AppId;
        private string PredictionKey;
        private string PredictionEndpoint;

        public LuisIntentGetter(string appId, string predictionKey, string predictionEndpoint)
        {
            AppId = appId;
            PredictionKey = predictionKey;
            PredictionEndpoint = predictionEndpoint;
        }

        public async Task<Intent> GetIntent(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", PredictionKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = message;

            // These optional request parameters are set to their default values
            // queryString["verbose"] = "true";
            // queryString["show-all-intents"] = "true";
            // queryString["staging"] = "false";
            // queryString["timezoneOffset"] = "0";

            var predictionEndpointUri = String.Format("{0}luis/prediction/v3.0/apps/{1}/slots/production/predict?{2}", PredictionEndpoint, AppId, queryString);


            var response = await client.GetAsync(predictionEndpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS.
            var jsonIntent = strResponseContent.ToString();
            Console.WriteLine(jsonIntent);

            var IntentResponse = JsonSerializer.Deserialize<IntentResponse>(jsonIntent, luisSerializationOptions);
            return IntentResponse.ToIntent();
        }
    }
}