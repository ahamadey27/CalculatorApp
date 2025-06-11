using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculatorWeb.Models;

namespace CalculatorWeb.Models
{
    public interface ICurrencyApiService
    {
        Task<IEnumerable<CurrencyData>> GetCurrencyAsync();
    }
}