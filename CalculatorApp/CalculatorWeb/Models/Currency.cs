using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalculatorWeb.Models
{
    public class Currency
    {
        public class CurrencyApiResponse
        {
            [JsonPropertyName("data")]
            public Dictionary<string, CurrencyData> Data { get; set; }
        }

        public class CurrencyData
        {
            [JsonPropertyName("symbol")]
            public string Symbol { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("symbol_native")]
            public string SymbolNative { get; set; }
            [JsonPropertyName("decimal_digits")]
            public int DecimalDigits { get; set; }
            [JsonPropertyName("rounding")]
            public int Rounding { get; set; }
            [JsonPropertyName("code")]
            public string Code { get; set; }
            [JsonPropertyName("name_plural")]
            public string NamePlural { get; set; }

        }
    }
}