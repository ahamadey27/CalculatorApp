using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculatorWeb.Models;
using System.Net.Http;
using System.Net.Http.Json; // For GetFromJsonAsync
using Microsoft.Extensions.Configuration; // To access API key

namespace CalculatorWeb.Services 
{
    public class CurrencyApiService : CalculatorWeb.Models.ICurrencyApiService 
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public CurrencyApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["FreeCurrencyApi:ApiKey"]; // Retrieve API key from configuration
            _httpClient.BaseAddress = new Uri("https://api.freecurrencyapi.com/v1/"); // Base URL for Freecurrencyapi.com
        }

        public async Task<IEnumerable<CalculatorWeb.Models.Currency.CurrencyData>> GetCurrenciesAsync()
        {
            try
            {
                var rawResponse = await _httpClient.GetStringAsync($"currencies?apikey={_apiKey}");
                var response = System.Text.Json.JsonSerializer.Deserialize<CalculatorWeb.Models.Currency.CurrencyApiResponse>(rawResponse);

                if (response?.Data != null)
                {
                    return response.Data.Values; // Return the collection of CurrencyData objects
                }
                return new List<CalculatorWeb.Models.Currency.CurrencyData>(); // Return empty list on no data
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"API request failed: {ex.Message}");
                return new List<CalculatorWeb.Models.Currency.CurrencyData>();
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                return new List<CalculatorWeb.Models.Currency.CurrencyData>();
            }
        }
    }
}