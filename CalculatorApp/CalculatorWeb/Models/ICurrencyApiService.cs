using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculatorWeb.Models;
using CalculatorWeb.Services;

namespace CalculatorWeb.Models
{
    public interface ICurrencyApiService
    {
        Task GetCurrenciesAsync();
        Task<IEnumerable<Currency.CurrencyData>> GetCurrencyAsync();
    }
}