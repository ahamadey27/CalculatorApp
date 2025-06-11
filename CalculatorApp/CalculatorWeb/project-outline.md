Building a TDD-Driven ASP.NET Core Razor Pages Calculator with Currency Integration in VS CodeIntroduction: Project Overview and TDD PhilosophyThis report outlines the development of a web-based calculator application using ASP.NET Core Razor Pages, integrating a feature to display world currencies from an external API. A fundamental aspect of this development process is the strict adherence to Test-Driven Development (TDD) principles, utilizing the xUnit testing framework within Visual Studio Code. The project aims to demonstrate robust software engineering practices, ensuring that the core logic is thoroughly tested and the application is designed for maintainability and extensibility.Project Overview and ObjectivesThe primary goal is to create a simple web calculator that accepts two decimal numbers and an arithmetic operator (addition, subtraction, multiplication, or division). Upon calculation, the result will be displayed to the user. Complementing this, a dropdown menu will be populated with a comprehensive list of world currencies, retrieved dynamically from a public API. This integration showcases how to consume external RESTful services within an ASP.NET Core Razor Pages application. The entire development journey will be guided by TDD, emphasizing the creation of tests prior to writing production code.Understanding Test-Driven Development (TDD)Test-Driven Development is an iterative software development methodology that prioritizes writing automated tests before implementing the corresponding functional code. This approach is characterized by a continuous "Red-Green-Refactor" cycle.1The cycle begins with the Red phase, where a new unit test is written to define a desired piece of functionality or to replicate a bug. This test is expected to fail initially because the feature has not yet been implemented.1 Following this, the Green phase involves writing the minimum amount of production code necessary to make the failing test pass. The focus here is solely on achieving a passing test, often leading to simple or even naive implementations.1 Finally, the Refactor phase involves improving the design, readability, and efficiency of the code while ensuring that all existing tests continue to pass. This step ensures that the codebase remains clean and maintainable as new features are added.1 This iterative process, often performed in very short cycles, allows for continuous refinement of the code.The benefits of TDD are multifaceted. It leads to early bug detection, as issues are identified and resolved as soon as they are introduced, making fixes significantly cheaper and less time-consuming.1 TDD also results in living documentation, where the suite of unit tests clearly illustrates the intended behavior and usage of the code, serving as an up-to-date reference for developers.4 A significant advantage is improved design; by writing tests first, developers are compelled to consider the API of their code from a consumer's perspective, which naturally promotes more modular, decoupled, and testable designs.1 This proactive approach to design ensures that components are inherently easier to test in isolation, reducing the likelihood of tightly coupled code. Furthermore, a comprehensive suite of passing tests provides a safety net, instilling confidence when refactoring or extending existing code, as any unintended side effects will be immediately flagged by failing tests.4 This also ensures that no unnecessary code is written, as development is laser-focused on implementing only what is required to satisfy the tests.1Effective unit tests adhere to several key characteristics, often summarized by the FIRST principles 5:
Fast: Tests should execute rapidly (in milliseconds) to encourage frequent execution throughout the development process.5
Isolated: Each test must be self-contained and independent, with no reliance on external factors such as file systems, databases, or network calls. This ensures consistent results and simplifies debugging.5
Repeatable: Running a test multiple times should consistently produce the same outcome, regardless of the environment or execution order.5
Self-Checking: Tests should automatically determine their pass or fail status without requiring human inspection.5
Timely: The effort required to write a unit test should not be disproportionately large compared to the code being tested. If testing becomes overly complex, it often indicates a need for a more testable design.5
Unit tests are commonly structured using the Arrange-Act-Assert (AAA) pattern 3:
Arrange: This step involves setting up the necessary objects, data, and preconditions required for the test to run.3
Act: The core action or method under test is performed in this step.3
Assert: The final step verifies that the actual outcome of the action matches the expected result.3
Overview of ASP.NET Core Razor Pages and xUnitASP.NET Core Razor Pages offers a streamlined, page-focused approach to building web user interfaces within the ASP.NET Core framework.9 It simplifies web development by co-locating the view (.cshtml file), which defines the HTML structure, and its corresponding logic (.cshtml.cs PageModel) within a designated "Pages" directory.9 This structure contrasts with the traditional Model-View-Controller (MVC) pattern by simplifying routing and reducing boilerplate, making it an excellent choice for applications that are primarily page-centric.9xUnit is a widely adopted, free, and open-source unit testing framework for.NET applications. It is favored for its simplicity, expressiveness, and extensibility, and is officially supported by the.NET Foundation.3 xUnit provides intuitive attributes such as [Fact] to declare individual test methods and combined with to enable parameterized tests, allowing a single test method to be executed with multiple sets of input data.3Phase 1: Core Calculator Logic - TDD FirstThis phase focuses on developing the fundamental arithmetic logic of the calculator using a strict TDD methodology. Each operation will be implemented and verified through unit tests before any user interface components are built.1.1 Solution and Project Setup in VS CodeThe initial step involves structuring the development environment within Visual Studio Code to support a TDD workflow. This includes creating a solution file to organize the projects, followed by setting up the main ASP.NET Core Razor Pages application and a dedicated xUnit test project.First, open your terminal in VS Code and create a new solution file. This file acts as a container for all related projects, providing a hierarchical and organized structure for the codebase.1Bashdotnet new sln -o CalculatorApp
cd CalculatorApp
Next, generate the main web application project using the ASP.NET Core Razor Pages template. Razor Pages simplifies web development by adopting a page-focused model, where the UI (.cshtml) and its backing logic (.cshtml.cs) are co-located within the Pages directory. This design choice streamlines routing and reduces complexity for page-centric scenarios.9 The dotnet new webapp command creates a new Razor Pages project, and dotnet sln add integrates it into the solution.10Bashdotnet new webapp -o CalculatorWeb
dotnet sln add./CalculatorWeb/CalculatorWeb.csproj
Following the TDD approach, a separate xUnit test project is created. This isolation of tests from the main application code is crucial for maintaining a clean codebase and ensuring that unit tests remain focused and independent.5 xUnit is a widely adopted and robust framework for this purpose.3Bashdotnet new xunit -o CalculatorWeb.Tests
dotnet sln add./CalculatorWeb.Tests/CalculatorWeb.Tests.csproj
3For the test project to access and test the code within the main application, a project reference must be established. This allows the xUnit test runner to discover and execute tests against the application's components.11Bashdotnet add./CalculatorWeb.Tests/CalculatorWeb.Tests.csproj reference./CalculatorWeb/CalculatorWeb.csproj
The Program.cs file serves as the application's entry point in ASP.NET Core, where services are configured and the HTTP request pipeline is defined.10 Key operations in this file include initializing the web application builder, adding Razor Pages services to the dependency injection container, building the application, mapping Razor Pages endpoints, and running the application.10This initial project structure, with a separate test project referencing the main application, is a fundamental architectural decision that underpins the entire TDD process. It ensures that tests are truly unit tests, focusing on isolated components, rather than inadvertently becoming integration tests due to tight coupling. This promotes a "design for testability" mindset from the very beginning, leading to more modular and maintainable code.5 The ability to test core logic independently of the UI or infrastructure concerns is paramount for rapid feedback cycles and robust development.1.2 Designing the Calculator ServiceAdhering to TDD principles, the design of the calculator's core logic begins with defining its contract before implementing its functionality. This approach, known as "contract-first" design, is instrumental in achieving testability and promoting loose coupling.The first step is to define an interface, ICalculatorService, which outlines the arithmetic operations the calculator will perform. This interface acts as a contract, specifying the methods without detailing their implementation. The use of interfaces is a crucial step towards implementing Dependency Injection (DI), a design pattern that allows for easy substitution of implementations, particularly useful for mocking in unit tests.6Create a new folder named Services within your CalculatorWeb project. Inside this folder, create the ICalculatorService.cs file:C#// CalculatorWeb/Services/ICalculatorService.cs
namespace CalculatorWeb.Services
{
    public interface ICalculatorService
    {
        decimal Add(decimal num1, decimal num2);
        decimal Subtract(decimal num1, decimal num2);
        decimal Multiply(decimal num1, decimal num2);
        decimal Divide(decimal num1, decimal num2);
    }
}
Following the interface definition, a concrete implementation, CalculatorService, is created. Initially, the methods in this class will throw NotImplementedException or return default values. This ensures that any tests written for these methods will fail, signaling the "Red" phase of the TDD cycle.Create CalculatorService.cs inside the Services folder:C#// CalculatorWeb/Services/CalculatorService.cs
using System;

namespace CalculatorWeb.Services
{
    public class CalculatorService : ICalculatorService
    {
        public decimal Add(decimal num1, decimal num2)
        {
            throw new NotImplementedException("Add method not implemented."); // This will cause the initial test to fail
        }

        public decimal Subtract(decimal num1, decimal num2)
        {
            throw new NotImplementedException("Subtract method not implemented.");
        }

        public decimal Multiply(decimal num1, decimal num2)
        {
            throw new NotImplementedException("Multiply method not implemented.");
        }

        public decimal Divide(decimal num1, decimal num2)
        {
            throw new NotImplementedException("Divide method not implemented.");
        }
    }
}
This design choice, driven by the principles of TDD, not only facilitates unit testing but also promotes a more flexible and maintainable architecture. By defining the ICalculatorService interface before its concrete implementation, the system adheres to the Dependency Inversion Principle. This principle dictates that high-level modules (like the Razor Page that will consume the calculator) should not depend on low-level modules (the concrete CalculatorService), but both should depend on abstractions (the ICalculatorService). This architectural decision ensures that the core business logic can be thoroughly unit-tested in isolation without the overhead or complexity of the ASP.NET Core web stack, leading to more robust and easily maintainable code.5 If the implementation of CalculatorService changes, as long as the ICalculatorService contract remains consistent, other parts of the application dependent on it will not be affected, promoting the Open/Closed Principle.1.3 Implementing Arithmetic Operations with TDDThe implementation of each arithmetic operation will strictly follow the Red-Green-Refactor cycle. Unit tests, which focus on testing individual code units in isolation, are crucial for ensuring each component functions as intended.3 These tests must be fast, isolated, repeatable, self-checking, and timely.5 The Arrange-Act-Assert (AAA) pattern will be used to structure each test.3Create a new folder Services inside your CalculatorWeb.Tests project, and then create the CalculatorServiceTests.cs file within it.Addition

Red (Write Failing Test): A test method is written using the [Fact] attribute, which designates it as a single unit test.3 This test calls the Add method on the ICalculatorService and asserts that the returned sum matches the expected value. At this stage, the test will fail because the Add method in CalculatorService still throws a NotImplementedException.
C#// CalculatorWeb.Tests/Services/CalculatorServiceTests.cs
using Xunit;
using CalculatorWeb.Services;
using System;

namespace CalculatorWeb.Tests.Services
{
    public class CalculatorServiceTests
    {
        private readonly ICalculatorService _calculatorService;

        public CalculatorServiceTests()
        {
            _calculatorService = new CalculatorService(); // Instantiating the concrete service for unit tests
        }

        [Fact]
        public void Add_TwoNumbers_ReturnsCorrectSum()
        {
            // Arrange
            decimal num1 = 5.0m;
            decimal num2 = 3.0m;
            decimal expected = 8.0m;

            // Act
            decimal actual = _calculatorService.Add(num1, num2);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}



Green (Implement Minimal Code): The Add method in CalculatorService.cs is modified to return the sum of the two input numbers. This is the minimal code required to make the previously failing test pass.
C#// CalculatorWeb/Services/CalculatorService.cs (partial)
public decimal Add(decimal num1, decimal num2)
{
    return num1 + num2;
}



Refactor: For a simple addition operation, immediate refactoring might not be necessary. This step is more critical for complex logic, focusing on improving code readability, efficiency, or adherence to design patterns without altering behavior.

Subtraction and MultiplicationThe same Red-Green-Refactor cycle is applied to the Subtract and Multiply methods. For each, a failing [Fact] test is written, followed by the minimal implementation to pass the test, and then an optional refactoring step.2Division (Valid Cases)For the division operation, and attributes are utilized. This allows a single test method to be executed with multiple sets of input arguments, efficiently testing various valid division scenarios.3

Red (Write Failing Test with and):
C#// CalculatorWeb.Tests/Services/CalculatorServiceTests.cs (partial)




public void Divide_ValidNumbers_ReturnsQuotient(decimal num1, decimal num2, decimal expected)
{
    // Arrange, Act, Assert are handled by the InlineData and direct call
    decimal actual = _calculatorService.Divide(num1, num2);
    Assert.Equal(expected, actual);
}



Green (Implement Minimal Code):
C#// CalculatorWeb/Services/CalculatorService.cs (partial)
public decimal Divide(decimal num1, decimal num2)
{
    return num1 / num2;
}


Handling Division by ZeroDivision by zero is a critical edge case that requires specific handling and testing. In C#, attempting to divide an int or decimal type by zero explicitly throws a DivideByZeroException.18 This behavior differs from floating-point types (double, float), which would result in PositiveInfinity, NegativeInfinity, or NaN (Not a Number) without throwing an exception.19 Given the requirement for "decimal" inputs, the expectation is indeed an exception.

Red (Write Test Expecting Exception): An xUnit test is written to specifically assert that a DivideByZeroException is thrown when the denominator is zero. Assert.Throws<TException> is the appropriate method for this.19
C#// CalculatorWeb.Tests/Services/CalculatorServiceTests.cs (partial)
[Fact]
public void Divide_ByZero_ThrowsDivideByZeroException()
{
    // Arrange
    decimal num1 = 10.0m;
    decimal num2 = 0.0m;

    // Act & Assert
    Assert.Throws<DivideByZeroException>(() => _calculatorService.Divide(num1, num2));
}



Green (Implement Minimal Code): In this specific scenario, the default behavior of decimal division in C# is to throw a DivideByZeroException when the denominator is zero. Therefore, the minimal return num1 / num2; implementation already satisfies this test. However, for clarity and explicit control, the Divide method can be augmented to explicitly check for a zero denominator and throw the exception.
C#// CalculatorWeb/Services/CalculatorService.cs (partial)
public decimal Divide(decimal num1, decimal num2)
{
    if (num2 == 0)
    {
        throw new DivideByZeroException("Cannot divide by zero.");
    }
    return num1 / num2;
}


This detailed handling of division by zero, especially the distinction between decimal and floating-point types, highlights the importance of precise type selection and robust error management. The TDD approach compels developers to consider these edge cases early in the development cycle, transforming potential runtime failures into well-defined and tested behaviors. This proactive identification and resolution of such nuances significantly enhance the overall robustness of the application.Table 1: Calculator Service Test CasesThis table serves as living documentation for the calculator's core logic, clearly outlining the expected behavior under various conditions, including critical edge cases. This level of detail is invaluable for understanding the validated functionality and for future maintenance.OperationFirst NumberOperatorSecond NumberExpected Result/BehaviorAddition5.0+3.08.0Subtraction10.5-4.26.3Multiplication2.5*4.010.0Division10.0/2.05.0Division7.5/2.53.0Division-10.0/5.0-2.0Division0.0/5.00.0Division10.0/0.0DivideByZeroException1.4 Running and Debugging Tests in VS CodeThe ability to quickly run and debug tests is paramount to the effectiveness of the TDD cycle. The "Red-Green-Refactor" process relies on immediate feedback to validate each small step of development.To execute all xUnit tests, navigate to the CalculatorWeb.Tests directory in your VS Code terminal and run the dotnet test command.3 This command automatically builds both the test project and the main application project, then executes all discovered tests.For diagnosing test failures, VS Code's integrated debugger is an invaluable tool. Breakpoints can be set directly within the test methods or in the production code under test. To initiate a debug session, use the "Run and Debug" view in VS Code or press Ctrl + F5.13 When prompted, select the C# debugger. This allows developers to step through the code line by line, inspect variable states, and understand the exact point of failure, facilitating rapid problem resolution.22 The efficiency of these tooling features directly impacts the adoption and effectiveness of TDD, making it a truly agile practice by enabling a rapid feedback loop.Phase 2: Razor Pages UI DevelopmentThis phase focuses on constructing the user interface for the calculator using ASP.NET Core Razor Pages, establishing the connection between the UI elements and the previously developed ICalculatorService.2.1 Designing the User Interface (Index.cshtml)Razor Pages simplify web development by adopting a page-focused approach, where the UI (.cshtml file) and its backing logic (.cshtml.cs PageModel) are co-located within the Pages directory.9 The Index.cshtml file will define the HTML structure for the calculator's input, operator selection, and result display.The user interface will feature a standard HTML <form> element configured with method="post" to ensure that input data is sent to the OnPost handler in the PageModel.23 This form will contain two <input type="number"> fields for the decimal numbers, a <select> dropdown for the arithmetic operator, and a submit button. The type="number" attribute, combined with step="0.01", provides a hint to browsers for decimal input and precision, although the actual value transmitted to the server is always a string.25Razor Tag Helpers, such as asp-for and asp-items, are instrumental in simplifying the binding of HTML elements to PageModel properties and populating dropdowns.23 The asp-validation-for Tag Helper is used to automatically display validation error messages associated with specific model properties, enhancing user feedback.27HTML@page
@model CalculatorWeb.Pages.IndexModel
@{
    ViewData = "Calculator";
}

<h1>Simple Calculator</h1>

<form method="post">
    <div class="form-group">
        <label asp-for="FirstNumber"></label>
        <input asp-for="FirstNumber" type="number" step="0.01" class="form-control" />
        <span asp-validation-for="FirstNumber" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="Operator"></label>
        <select asp-for="Operator" class="form-control">
            <option value="+">Add (+)</option>
            <option value="-">Subtract (-)</option>
            <option value="*">Multiply (*)</option>
            <option value="/">Divide (/)</option>
        </select>
        <span asp-validation-for="Operator" class="text-danger"></span>
    </div>
    <div class="form-group">
        <label asp-for="SecondNumber"></label>
        <input asp-for="SecondNumber" type="number" step="0.01" class="form-control" />
        <span asp-validation-for="SecondNumber" class="text-danger"></span>
    </div>
    <button type="submit" class="btn btn-primary">Calculate</button>
</form>

@if (Model.Result.HasValue)
{
    <h2 class="mt-4">Result: @Model.Result.Value</h2>
}
@if (!string.IsNullOrEmpty(Model.ErrorMessage))
{
    <p class="text-danger mt-4">@Model.ErrorMessage</p>
}

<div class="form-group mt-4">
    <label asp-for="SelectedCurrency"></label>
    <select asp-for="SelectedCurrency" asp-items="Model.Currencies" class="form-control">
        <option value="">Select a currency</option>
    </select>
</div>
23The asp-for tag helper automatically generates id and name attributes for input fields, which are essential for correct form submission and model binding. This significantly reduces the amount of boilerplate HTML code and minimizes potential errors. Similarly, the asp-validation-for tag helper seamlessly integrates client-side and server-side validation, providing immediate feedback to the user.23 This simplification of data binding and validation directly enhances developer productivity and reduces the complexity associated with handling forms in web applications. Razor Pages' emphasis on a "page-centric" development model, combined with its robust tag helper system, makes it an excellent choice for applications requiring straightforward UI interactions without the overhead of a full MVC pattern.92.2 PageModel (Index.cshtml.cs) for Calculator LogicThe IndexModel class, located in Pages/Index.cshtml.cs, serves as the code-behind for the Index.cshtml page. It inherits from PageModel and contains the C# logic that handles HTTP requests for the page.10Properties for the calculator's input values (FirstNumber, Operator, SecondNumber), the calculation Result, and any ErrorMessage are defined within the IndexModel. The attribute is applied to these properties, enabling automatic model binding from the HTML form. This mechanism simplifies data retrieval from form submissions, as ASP.NET Core automatically populates these properties with the corresponding values from the HTTP request.23 Data annotation attributes like and `` are used to define server-side validation rules for the input properties, ensuring data integrity.C#// CalculatorWeb/Pages/Index.cshtml.cs
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

            if (!FirstNumber.HasValue ||!SecondNumber.HasValue)
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
The ICalculatorService is injected into the IndexModel constructor. This is enabled by registering the CalculatorService (implementing ICalculatorService) with the Dependency Injection container in Program.cs.6 This practice promotes loose coupling, as the PageModel depends on an abstraction (ICalculatorService) rather than a concrete implementation, enhancing testability and flexibility.C#// CalculatorWeb/Program.cs (partial)
//...
builder.Services.AddRazorPages();
builder.Services.AddTransient<CalculatorWeb.Services.ICalculatorService, CalculatorWeb.Services.CalculatorService>(); // Register the calculator service
//...
The OnPost handler method is automatically invoked when the HTML form is submitted via a POST request.23 Inside this method, ModelState.IsValid is checked first. This property indicates whether the data bound to the model properties is valid according to the DataAnnotations attributes (e.g., ,).27 If validation fails, an error message is set, and the page is returned to display these errors to the user.The core calculation logic is then executed within a try-catch block. A switch statement directs the call to the appropriate arithmetic method on the injected _calculatorService. Crucially, the try-catch block specifically handles the DivideByZeroException that the CalculatorService might throw, providing a user-friendly error message to prevent a generic application crash.18 A general Exception catch is also included for broader robustness. Finally, the Result or ErrorMessage property is set, and the page is returned to update the UI.This design, where ICalculatorService is injected into the PageModel, exemplifies a clear separation of concerns. The UI logic within the PageModel is decoupled from the core business logic of the calculator. This means the PageModel does not need to understand the internal workings of the Add or Divide methods; it only needs to know that it can delegate these operations to the service. This separation simplifies the testing of both components independently. Furthermore, handling the DivideByZeroException explicitly in the OnPost handler, rather than letting it propagate, provides a more refined user experience by presenting a specific, understandable error message.19 This modularity makes the application easier to understand, debug, and maintain, as each component has a distinct responsibility.Phase 3: World Currency API IntegrationThis phase details the process of fetching a list of world currencies from an external API and populating a dropdown menu on the Razor Page, enhancing the application's functionality.3.1 Choosing a Free Currency APITo fulfill the requirement of populating a dropdown with world currencies, a suitable free public API is necessary. Based on the available information, two primary candidates were evaluated:
Currencylayer: This API offers access to a broad range of 168 world currencies and precious metals. Its free plan provides 100 API calls per month and daily data updates.13 While comprehensive in its currency coverage, the free tier's call limit is relatively restrictive for frequent requests.
Freecurrencyapi.com: This alternative provides data for 32 major world currencies, with a more generous free tier allowing 5,000 monthly requests and daily updates.13
For the purpose of this demonstration, Freecurrencyapi.com is selected due to its higher free monthly request limit, which is more forgiving for development and testing cycles, even though it offers fewer currencies. For a production application requiring a wider range of currencies or higher request volumes, a paid plan or a more robust solution would be necessary, as free tiers often impose significant limitations that can quickly be exhausted during development or in multi-user environments.13 This highlights a crucial consideration in real-world application development: balancing immediate development needs with long-term scalability and reliability.Both Currencylayer and Freecurrencyapi.com require users to sign up on their respective websites to obtain a free API key.13 This key is essential for authenticating requests to the API and will be used in the service implementation.3.2 Creating a Currency API ServiceTo consume the external currency API, a dedicated service will be created. This service will encapsulate the logic for making HTTP requests, deserializing the JSON responses, and handling potential API-related errors.First, define the C# classes that mirror the expected JSON response structure from Freecurrencyapi.com's /currencies endpoint.32 This allows for strongly-typed deserialization of the API's data.C#// CalculatorWeb/Models/Currency.cs
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace CalculatorWeb.Models
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
Next, define an interface, ICurrencyApiService, to maintain testability and enable mocking of the API service in unit tests. This adheres to the principles of Dependency Injection, allowing the service to be easily substituted for a mock during testing.6C#// CalculatorWeb/Services/ICurrencyApiService.cs
using CalculatorWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculatorWeb.Services
{
    public interface ICurrencyApiService
    {
        Task<IEnumerable<CurrencyData>> GetCurrenciesAsync();
    }
}
The CurrencyApiService implements the ICurrencyApiService interface and handles the actual HTTP calls. HttpClient is the standard class in.NET for sending HTTP requests and receiving responses.34 It is crucial to use HttpClient for asynchronous operations with async/await keywords, as this prevents blocking the main thread during I/O-bound network calls, thereby improving application responsiveness and scalability.15The API key, obtained from Freecurrencyapi.com, should be stored securely and accessed via the application's configuration system (e.g., appsettings.json for development, or User Secrets/environment variables for production).C#// CalculatorWeb/Services/CurrencyApiService.cs
using CalculatorWeb.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // For GetFromJsonAsync
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // To access API key
using System;

namespace CalculatorWeb.Services
{
    public class CurrencyApiService : ICurrencyApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public CurrencyApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["FreeCurrencyApi:ApiKey"]; // Retrieve API key from configuration
            _httpClient.BaseAddress = new Uri("https://api.freecurrencyapi.com/v1/"); // Base URL for Freecurrencyapi.com
        }

        public async Task<IEnumerable<CurrencyData>> GetCurrenciesAsync()
        {
            try
            {
                // Request to Freecurrencyapi.com's /currencies endpoint
                var response = await _httpClient.GetFromJsonAsync<CurrencyApiResponse>($"currencies?apikey={_apiKey}");

                if (response?.Data!= null)
                {
                    return response.Data.Values; // Return the collection of CurrencyData objects
                }
                return new List<CurrencyData>(); // Return empty list on no data
            }
            catch (HttpRequestException ex)
            {
                // Log the exception (e.g., using ILogger) for debugging purposes
                Console.WriteLine($"API request failed: {ex.Message}");
                // For a robust application, consider a fallback mechanism or a cached response
                return new List<CurrencyData>();
            }
        }
    }
}
To integrate this service into the application, HttpClient and CurrencyApiService are registered with the Dependency Injection container in Program.cs. Using AddHttpClient is the recommended approach, as it correctly manages the HttpClient's lifecycle, preventing common issues like socket exhaustion.15C#// CalculatorWeb/Program.cs (partial)
//...
builder.Services.AddRazorPages();
builder.Services.AddTransient<CalculatorWeb.Services.ICalculatorService, CalculatorWeb.Services.CalculatorService>();
builder.Services.AddHttpClient<CalculatorWeb.Services.ICurrencyApiService, CalculatorWeb.Services.CurrencyApiService>(); // Register HttpClient for the currency service
//...
For development purposes, the API key can be stored in appsettings.json. For production environments, more secure methods like User Secrets or environment variables should be used.JSON// appsettings.json (for development)
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FreeCurrencyApi": {
    "ApiKey": "YOUR_FREECURRENCYAPI_KEY" // Replace with your actual key
  }
}
The approach to consuming external APIs, particularly the use of HttpClient with async/await, is not merely a best practice for responsiveness; it is a fundamental requirement for building scalable web applications. Synchronous network calls would block server threads, severely limiting the application's ability to handle concurrent user requests. Furthermore, implementing robust error handling, such as try-catch blocks for HttpRequestException and checking HttpResponseMessage.EnsureSuccessStatusCode(), is crucial for preventing API failures from cascading and crashing the entire application, thereby ensuring a more resilient user experience.15 The proper management of HttpClient instances through AddHttpClient also prevents common pitfalls like socket exhaustion, which can lead to application instability. This phase demonstrates how to address critical non-functional aspects of web development, emphasizing that a well-designed API consumption strategy is paramount for building performant and reliable web applications that interact with external services.Table 2: Currency API Endpoints and Response StructureThis table provides a concise, actionable summary of the chosen currency API's relevant endpoint, its purpose, required parameters, an example request, and a simplified representation of its JSON response structure. It also highlights the free tier limitations, which are critical for understanding the API's practical usability.
API NameEndpoint URL (List Currencies)Request MethodRequired Parameter(s)Example JSON Response (Simplified)Free Tier LimitsFreecurrencyapi.comhttps://api.freecurrencyapi.com/v1/currenciesGETapikey{"data": {"USD": {"code": "USD", "name": "United States Dollar"}, "EUR": {"code": "EUR", "name": "Euro"},...}}5,000 API Calls/Month, Daily Updates, 32 Currencies 13
This table serves as a quick reference for developers, distilling complex API documentation into an easily digestible format. By explicitly listing free tier limitations, it empowers developers to make informed decisions about API selection for current and future projects, considering both functional requirements and non-functional constraints like cost and scalability.13 The clear example of the JSON response structure is invaluable for debugging and developing the C# models for deserialization, ensuring correct data parsing and integration.3.3 Populating the Currency DropdownWith the CurrencyApiService in place, the next step is to integrate it with the Razor Page to populate the currency dropdown menu.The OnGetAsync handler method in Index.cshtml.cs is the appropriate place to fetch data required for initial page rendering, such as the list of currencies.26 By making OnGet asynchronous (OnGetAsync), the application can efficiently retrieve the currency data without blocking the UI thread, ensuring a smooth user experience.C#// CalculatorWeb/Pages/Index.cshtml.cs (Updated constructor and OnGetAsync)
//...
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
//...
The raw currency data received from the API (e.g., a collection of CurrencyData objects) needs to be transformed into a SelectList or List<SelectListItem> to be compatible with Razor Pages dropdowns.26 The SelectListItem class typically has Text and Value properties, which map to the display text and the actual value of each dropdown option, respectively.26 This transformation step is a common pattern when integrating external data with UI components, ensuring that the data is in the correct format for presentation and binding.Finally, the asp-items tag helper is used in Index.cshtml to bind the Currencies SelectList property to the <select> HTML element.26 This automatically generates the <option> elements for the dropdown.HTML<div class="form-group mt-4">
    <label asp-for="SelectedCurrency">Currency:</label>
    <select asp-for="SelectedCurrency" asp-items="Model.Currencies" class="form-control">
        <option value="">Select a currency</option> </select>
</div>
This process of data transformation and UI binding highlights the importance of adaptation layers between different parts of an application, such as API responses and UI models. This isolation ensures that changes in the API's response format do not directly impact the UI, making the application more maintainable and resilient to external changes. Populating the dropdown on OnGetAsync ensures that the currency list is available when the page initially loads, providing a seamless user experience.Phase 4: Refinement, Testing, and Deployment ConsiderationsThis final phase addresses the broader aspects of application quality, including a review of testing strategies, adherence to best practices, and considerations for running and deploying the application.4.1 Comprehensive Unit Testing ReviewA thorough review of the unit tests is essential to ensure adequate test coverage for all core calculator operations and their edge cases, such as division by zero.4 The primary objective of these tests is to provide confidence that any new changes or refactorings will not inadvertently break existing functionality.4While the primary focus of this report has been on unit testing, it is important to acknowledge the broader spectrum of testing. Unit tests confirm the behavior of individual components in isolation, but a complete testing strategy also includes integration tests and potentially end-to-end tests.3 Integration tests would verify how different components, such as the PageModel and the ICalculatorService, interact with each other. Similarly, for API consumption, while unit tests of the CurrencyApiService would mock the HttpClient to ensure isolation, integration tests would involve making actual calls to the external currency API to verify real-world connectivity and data parsing.3 This distinction is vital for understanding what each test type validates and where the testing effort should be focused to provide comprehensive quality assurance. This broader perspective on testing ensures that the application is robust at all levels, from individual units to integrated systems.4.2 Best Practices and Code QualityAdhering to established best practices is crucial for developing high-quality, maintainable, and scalable applications. These practices complement TDD by addressing aspects beyond just functional correctness.Razor Pages Best Practices:
Separation of Concerns: Maintain a clear separation between UI logic and business logic. The PageModel should primarily handle UI interactions and data flow, delegating core business operations to dedicated services like ICalculatorService and ICurrencyApiService.
Model Binding and Tag Helpers: Leverage ASP.NET Core's powerful model binding capabilities using `` and Razor Tag Helpers (e.g., asp-for, asp-items) for clean, efficient, and less error-prone form handling.23
Validation: Implement robust server-side validation using Data Annotations (e.g., ,), which can be complemented by client-side validation for immediate user feedback.28
API Consumption Best Practices:
Asynchronous Programming: Always use async/await for any I/O-bound operations, particularly network calls to external APIs. This prevents blocking the main thread, ensuring the application remains responsive and scalable, especially under load.15
HttpClient Lifecycle Management: Utilize AddHttpClient with Dependency Injection to register HttpClient instances. This approach correctly manages the HttpClient's lifetime, preventing common issues such as socket exhaustion and ensuring efficient resource utilization.15
Robust Error Handling: Implement comprehensive try-catch blocks and use HttpResponseMessage.EnsureSuccessStatusCode() to gracefully handle network errors, API-specific error responses, and deserialization failures. Providing informative error messages to the user enhances the application's resilience.15
Secure Configuration Management: Store API keys and other sensitive configuration data outside of the source code. For development, appsettings.json or User Secrets are suitable. For production, environment variables or dedicated secret management services are recommended.
These best practices, when combined with TDD, contribute to a holistic approach to software quality. For instance, correctly managing HttpClient prevents resource leaks, and proper error handling makes the application more resilient to external failures. These practices address aspects beyond just functional correctness, impacting performance, security, and long-term viability. Building a robust application therefore requires a combination of effective development methodologies and adherence to established best practices across all layers of the application, from the UI to business logic and external service integrations.4.3 Running the Application in VS CodeTo run the developed ASP.NET Core Razor Pages application, navigate to the CalculatorWeb directory in your terminal and execute dotnet run. Alternatively, within Visual Studio Code, press Ctrl + F5 to run the app without debugging.13 Upon launching, verify that the calculator functionality operates as expected, performing accurate arithmetic calculations, and that the currency dropdown is successfully populated with data retrieved from the external API.4.4 Next Steps and Further EnhancementsThe current application provides a solid foundation, and several enhancements can be considered to extend its functionality and robustness:
Advanced Calculator Features: Expand the calculator's capabilities to include memory functions, scientific operations (e.g., square root, powers), or more sophisticated input parsing (e.g., handling expressions).
Currency Conversion: Extend the currency integration to perform actual currency conversions. This would involve utilizing additional API endpoints (e.g., /latest or /convert from Freecurrencyapi.com or Currencylayer) to fetch exchange rates and apply them to the calculated amount.13
UI/UX Improvements: Enhance the user interface with better styling, responsive design, or real-time updates for calculations and currency selection (e.g., using JavaScript/AJAX or HTMX for partial page updates).27 Incorporate accessibility features to ensure the application is usable by a wider audience.
Logging and Monitoring: Implement a comprehensive logging system (e.g., using Serilog) to capture application events, errors, and API call details. This is invaluable for debugging, performance monitoring, and understanding user behavior.15
Configuration Management: Further explore and implement more secure and scalable methods for managing API keys and other sensitive configuration data, especially for deployment in production environments.
Containerization: Consider packaging the application within a Docker container. This simplifies deployment, ensures consistency across environments, and isolates the application from the host system's dependencies.
Continuous Integration/Continuous Deployment (CI/CD): Integrate the unit tests into a CI/CD pipeline. This automates the build, test, and deployment processes, ensuring that code changes are continuously validated and deployed efficiently.
This exploration of next steps demonstrates that software development is an ongoing process of refinement and adaptation. It encourages the application of the learned TDD and architectural principles to increasingly complex scenarios, and to consider the entire software development lifecycle, from initial coding to deployment and ongoing maintenance.