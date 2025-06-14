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
        private CurrencyApiService? _currencyApiServiceImpl;

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
            _currencyApiServiceImpl = currencyApiService as CurrencyApiService;
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

            var action = Request.Form["action"].ToString();

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

            // Always calculate first
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
                ErrorMessage = string.Empty; // Clear any previous errors on success
            }
            catch (DivideByZeroException ex)
            {
                ErrorMessage = ex.Message; // Display specific error for division by zero
                Result = null; // Clear result on error
                return;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred: {ex.Message}"; // Catch other unexpected errors
                Result = null; // Clear result on error
                return;
            }

            // If action is convert, perform conversion
            if (action == "convert")
            {
                if (Result.HasValue && !string.IsNullOrEmpty(SelectedCurrency) && _currencyApiServiceImpl != null)
                {
                    var rate = await _currencyApiServiceImpl.GetRateForCurrencyAsync(SelectedCurrency);
                    if (rate.HasValue)
                    {
                        Result = Math.Round(Result.Value * rate.Value, 2); // Round to 2 decimal places
                        ErrorMessage = string.Empty;
                    }
                    else
                    {
                        ErrorMessage = "Could not fetch conversion rate.";
                    }
                }
                else
                {
                    ErrorMessage = "No result to convert or no currency selected.";
                }
            }
            else
            {
                // Always round the result for normal calculation as well
                if (Result.HasValue)
                    Result = Math.Round(Result.Value, 2);
            }
        }
    }
}