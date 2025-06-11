// CalculatorWeb/Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectList
using CalculatorWeb.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculatorWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICalculatorService _calculatorService;
        private readonly ICurrencyApiService _currencyApiService; // To be injected in Phase 3




        public decimal? FirstNumber { get; set; }



        public string Operator { get; set; } = "+"; // Default operator




        public decimal? SecondNumber { get; set; }

        public decimal? Result { get; set; }
        public string ErrorMessage { get; set; }

        // Properties for currency dropdown (Phase 3)
        public SelectList Currencies { get; set; }

        public string SelectedCurrency { get; set; }

        public IndexModel(ICalculatorService calculatorService, ICurrencyApiService currencyApiService)
        {
            _calculatorService = calculatorService;
            _currencyApiService = currencyApiService;
        }

        public async Task OnGetAsync() // Made async for currency loading
        {
            // Load currencies on page load (Phase 3)
            var currencyData = await _currencyApiService.GetCurrenciesAsync();
            Currencies = new SelectList(currencyData.OrderBy(c => c.Name), nameof(Models.CurrencyData.Code), nameof(Models.CurrencyData.Name));
        }

        public void OnPost()
        {
            // Re-populate currencies for the POST request to keep dropdown populated
            // This is important because OnGetAsync is not called on POST
            OnGetAsync().Wait(); // Synchronously wait for currencies to load for simplicity in this example

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