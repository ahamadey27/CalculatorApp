using CalculatorWeb.Services;
using CalculatorWeb.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddTransient<CalculatorWeb.Services.ICalculatorService, CalculatorWeb.Services.CalculatorService>(); // Register the calculator service
builder.Services.AddHttpClient<CalculatorWeb.Services.ICurrencyApiService, CalculatorWeb.Services.CurrencyApiService>(); // Register HttpClient for the currency service

public IndexModel(ICalculatorService calculatorService, ICurrencyApiService currencyApiService)
{
    _calculatorService = calculatorService;
    _currencyApiService = currencyApiService;
}

public async Task OnGetAsync() // Made OnGet asynchronous to await API call
{
    var currencyData = await _currencyApiService.GetCurrenciesAsync();
    // Transform the fetched data into a SelectList suitable for the dropdown
    Currencies = new SelectList(currencyData.OrderBy(c => c.Name), nameof(Models.CurrencyData.Code), nameof(Models.CurrencyData.Name));
}
//provides new CalculatorService instance whenever ICalculatorService is requested
builder.Services.AddScoped<ICalculatorService, CalculatorService>(); 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
