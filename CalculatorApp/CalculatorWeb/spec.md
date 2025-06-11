# Project: TDD Razor Pages Calculator

**Goal:** To develop a web-based calculator using ASP.NET Core Razor Pages, strictly adhering to Test-Driven Development (TDD). The application will perform basic arithmetic and integrate with an external API to display a list of world currencies, demonstrating robust testing, dependency injection, and secure API consumption.

---

## Components

### Environment/Hosting
* **Local Development Machine:** Windows/macOS/Linux
* **IDE:** Visual Studio Code
* **Version Control:** Git

### Software Components
* **Web Application Backend**
    * **Framework:** ASP.NET Core Razor Pages
    * **Language:** C#
    * **API Interaction:** Exposes form endpoints (`POST`), consumes external RESTful APIs (`GET`).
    * **JSON Processing:** `System.Text.Json`
* **Core Logic Services**
    * `CalculatorService.cs` (Handles all arithmetic operations).
    * `CurrencyApiService.cs` (Fetches currency data from the external API).
* **Testing Framework**
    * **Framework:** xUnit
    * **Purpose:** Unit testing the core logic services.

### External APIs
* **Freecurrencyapi.com:** Used to fetch a list of world currencies to populate a dropdown menu.

---

## Core Services and Data Structures

### CalculatorService.cs (Service)
* **Interface:** `ICalculatorService`
* **Responsibilities:**
    * Provides methods for basic arithmetic: Addition, Subtraction, Multiplication, and Division.
    * Implements business logic for handling edge cases, specifically throwing a `DivideByZeroException`.
* **Key Methods (Conceptual):**
    * `Add(decimal num1, decimal num2)`: Returns the sum.
    * `Subtract(decimal num1, decimal num2)`: Returns the difference.
    * `Multiply(decimal num1, decimal num2)`: Returns the product.
    * `Divide(decimal num1, decimal num2)`: Returns the quotient or throws an exception.

### CurrencyApiService.cs (Service)
* **Interface:** `ICurrencyApiService`
* **Responsibilities:**
    * Encapsulates all logic for interacting with the external `Freecurrencyapi.com` API.
    * Constructs and sends HTTP GET requests asynchronously.
    * Deserializes the JSON response into strongly-typed C# objects.
    * Handles potential network or API errors gracefully.
* **Key Methods (Conceptual):**
    * `GetCurrenciesAsync()`: Returns a collection of `CurrencyData` objects.

### Index.cshtml.cs (PageModel)
* **Responsibilities:**
    * Acts as the controller/handler for the main calculator page.
    * Uses Dependency Injection to receive `ICalculatorService` and `ICurrencyApiService`.
    * Handles `OnGetAsync` to populate the currency dropdown on initial page load.
    * Handles `OnPost` to process form submissions, validate input, and orchestrate calls to the `ICalculatorService`.
    * Manages UI state, including input values, the final `Result`, and any `ErrorMessage`.
* **Implied Data Models:**
    * `CurrencyApiResponse` & `CurrencyData`: Records for deserializing the JSON response from the currency API.
    * `SelectList`: Used to bind the collection of currencies to the HTML dropdown.

### Configuration (`appsettings.json` / User Secrets)
* **`FreeCurrencyApi:ApiKey`**: Stores the secret API key required to authenticate with `Freecurrencyapi.com`.

---

## Development Plan

### Phase 1: Core Calculator Logic Implementation via TDD
- [x] **Step 1.1: Project Initialization & Environment Setup**
    - [x] Create new solution `CalculatorApp.sln`.
    - [x] Create ASP.NET Core Razor Pages project `CalculatorWeb`.
    - [x] Create xUnit test project `CalculatorWeb.Tests`.
    - [x] Add project reference from `Tests` to `Web`.
- [x] **Step 1.2: Define Core Domain Contracts**
    - [x] Define `ICalculatorService` interface.
    - [x] Create initial `CalculatorService` class with methods throwing `NotImplementedException`.
- [x] **Step 1.3: TDD Implementation of Arithmetic Operations**
    - [x] Write failing unit test for `Add` method (Red).
    - [x] Implement `Add` method to make the test pass (Green).
    - [x] Repeat Red-Green-Refactor cycle for `Subtract`, `Multiply`, and `Divide`.
- [x] **Step 1.4: TDD for Edge Cases**
    - [x] Write a failing unit test to assert that `Divide` throws a `DivideByZeroException` when the denominator is zero.
    - [x] Ensure the implementation passes the exception test.
- [x] **Step 1.5: Dependency Injection Setup**
    - [x] Register `ICalculatorService` and its implementation in `Program.cs`.

### Phase 2: Razor Pages UI and PageModel Logic
- [ ] **Step 2.1: Implement Razor Page UI (`Index.cshtml`)**
    - [ ] Create an HTML form with `method="post"`.
    - [ ] Add input fields for two numbers, a select dropdown for the operator, and a submit button.
    - [ ] Use Razor Tag Helpers (`asp-for`, `asp-validation-for`) for data binding and validation messages.
- [ ] **Step 2.2: Implement PageModel (`Index.cshtml.cs`)**
    - [ ] Define properties for inputs (`FirstNumber`, `SecondNumber`, `Operator`) and outputs (`Result`, `ErrorMessage`) using `[BindProperty]`.
    - [ ] Use constructor injection to get an instance of `ICalculatorService`.
- [ ] **Step 2.3: Implement `OnPost` Handler Logic**
    - [ ] Check `ModelState.IsValid` to ensure inputs are valid.
    - [ ] Call the appropriate `_calculatorService` method based on the selected operator.
    - [ ] Use a `try-catch` block to gracefully handle the `DivideByZeroException` and set a user-friendly error message.
- [ ] **Step 2.4: Implement Input Validation**
    - [ ] Add Data Annotation attributes (e.g., `[Required]`) to the PageModel properties to enforce server-side validation.

### Phase 3: External Currency API Integration
- [ ] **Step 3.1: Define Currency Data Models**
    - [ ] Create `CurrencyApiResponse` and `CurrencyData` C# classes to match the structure of the API's JSON response.
- [ ] **Step 3.2: Define API Service Contract & Implementation**
    - [ ] Define the `ICurrencyApiService` interface.
    - [ ] Implement `CurrencyApiService` using `HttpClient` to fetch data from the external API asynchronously.
- [ ] **Step 3.3: Configure Secure API Key**
    - [ ] Add the `FreeCurrencyApi:ApiKey` to `appsettings.json` for local development and add to `.gitignore`.
    - [ ] Use `IConfiguration` to retrieve the key in the `CurrencyApiService`.
- [ ] **Step 3.4: Dependency Injection Setup**
    - [ ] Register `ICurrencyApiService` and configure `AddHttpClient` in `Program.cs`.
- [ ] **Step 3.5: Integrate into PageModel**
    - [ ] Inject `ICurrencyApiService` into the `IndexModel` constructor.
    - [ ] Implement `OnGetAsync` to call the service and retrieve currency data.
    - [ ] Create a `SelectList` from the returned data and bind it to the currency dropdown in the UI.

### Phase 4: Finalization and Documentation
- [ ] **Step 4.1: Craft Excellent README.md**
    - [ ] Add project description, goals, and technology stack.
    - [ ] Provide clear instructions for local setup, including API key configuration.
    - [ ] Document how to run the project and execute tests.
- [ ] **Step 4.2: Comprehensive Testing Review**
    - [ ] Review all unit tests for clarity and coverage.
    - [ ] Document the distinction between unit tests (mocking `HttpClient`) and the need for integration tests (actual API calls).
- [ ] **Step 4.3: Document Best Practices**
    - [ ] Summarize key principles followed: TDD, Separation of Concerns, secure configuration, and proper `HttpClient` usage.
- [ ] **Step 4.4: Prepare Deployment Talking Points**
    - [ ] Outline next steps for deployment, such as containerizing with Docker and setting up a CI/CD pipeline.