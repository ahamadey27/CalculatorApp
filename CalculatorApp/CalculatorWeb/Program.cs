using CalculatorWeb.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddTransient<CalculatorWeb.Services.ICalculatorService, CalculatorWeb.Services.CalculatorService>(); // Register the calculator service
//...

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
