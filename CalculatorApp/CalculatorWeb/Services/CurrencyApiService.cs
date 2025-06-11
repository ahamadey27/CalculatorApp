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

        public async Task<IEnumerable<Currency.CurrencyData>> GetCurrenciesAsync()
        {
            try
            {
                // Request to Freecurrencyapi.com's /currencies endpoint
                var response = await _httpClient.GetFromJsonAsync<Currency.CurrencyApiResponse>($"currencies?apikey={_apiKey}");

                if (response?.Data != null)
                {
                    return response.Data.Values; // Return the collection of CurrencyData objects
                }
                return new List<Currency.CurrencyData>(); // Return empty list on no data
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (e.g., using ILogger) for debugging purposes
                Console.WriteLine($"API request failed: {ex.Message}");
                // For a robust application, consider a fallback mechanism or a cached response
                return new List<Currency.CurrencyData>();
            }

        }

        // Implementation for interface method
        public async Task<IEnumerable<Currency.CurrencyData>> GetCurrencyAsync()
        {
            return await GetCurrenciesAsync();
        }
    }
}