// CalculatorWeb/Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using CalculatorWeb.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CalculatorWeb.Models;

namespace CalculatorWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICalculatorService _calculatorService;
        private readonly ICurrencyApiService _currencyApiService; // To be injected in Phase 3

        [BindProperty]
        public decimal? FirstNumber { get; set; }

        [BindProperty]
        public string Operator { get; set; } = "+"; // Default operator

        [BindProperty]
        public decimal? SecondNumber { get; set; }

        [BindProperty]
        public string? SelectedCurrency { get; set; }

        public SelectList? Currencies { get; set; }

        public decimal? Result { get; set; }
        public string? ErrorMessage { get; set; }

        // Properties for currency dropdown (Phase 3)
        

        public IndexModel(ICalculatorService calculatorService, ICurrencyApiService currencyApiService)
        {
            _calculatorService = calculatorService;
            _currencyApiService = currencyApiService;
        }

        public async Task OnGetAsync() // Made async for currency loading
        {
            // Load currencies on page load (Phase 3)
            var currencyData = await _currencyApiService.GetCurrenciesAsync();
            Currencies = new SelectList(currencyData.OrderBy(c => c.Name), nameof(CalculatorWeb.Models.Currency.CurrencyData.Code), nameof(CalculatorWeb.Models.Currency.CurrencyData.Name));
        }

        public async Task OnPostAsync()
        {
            // Re-populate currencies for the POST request to keep dropdown populated
            var currencyData = await _currencyApiService.GetCurrenciesAsync();
            Currencies = new SelectList(currencyData.OrderBy(c => c.Name), nameof(CalculatorWeb.Models.Currency.CurrencyData.Code), nameof(CalculatorWeb.Models.Currency.CurrencyData.Name));

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please correct the input errors.";
                return; // Return the page with validation errors
            }

            if (!FirstNumber.HasValue || !SecondNumber.HasValue)
            {
                ErrorMessage = "Both numbers are required.";
                return;
            }

            try
            {
                Result = Operator switch
                {
                    "+" => _calculatorService.Add(FirstNumber.Value, SecondNumber.Value),
                    "-" => _calculatorService.Subtract(FirstNumber.Value, SecondNumber.Value),
                    "*" => _calculatorService.Multiply(FirstNumber.Value, SecondNumber.Value),
                    "/" => _calculatorService.Divide(FirstNumber.Value, SecondNumber.Value),
                    _ => throw new InvalidOperationException("Invalid operator selected.")
                };

                // Currency conversion logic
                if (!string.IsNullOrEmpty(SelectedCurrency) && Result.HasValue)
                {
                    var selected = currencyData.FirstOrDefault(c => c.Code == SelectedCurrency);
                    if (selected != null && selected.Code != "USD") // Assuming base is USD
                    {
                        // Convert from USD to selected currency
                        // If you want to show the value in the selected currency, you need the rate
                        // For this API, 1 USD = selected currency's value in USD
                        // So, to convert USD to selected currency: Result / (1 USD in selected currency)
                        Result = Result / 1m; // Default, in case rate is not found
                        if (selected != null)
                        {
                            // If you want to convert from USD to selected currency, you need the rate
                            // But CurrencyData does not have a rate, so you need to fetch rates if needed
                            // For now, just show the result as is, or extend the model to include rates
                        }
                    }
                }

                ErrorMessage = string.Empty; // Clear any previous errors on success
            }
            catch (DivideByZeroException ex)
            {
                ErrorMessage = ex.Message; // Display specific error for division by zero
                Result = null; // Clear result on error
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}"; // Catch other unexpected errors
                Result = null; // Clear result on error
            }
        }
    }
}