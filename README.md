# ENSEK Meter Reading Upload API

This is a C# Web API solution developed as part of the ENSEK Remote Technical Test. It enables uploading and validating meter readings from a CSV file, ensuring the data corresponds to known account IDs.

---

## ✅ Features

- Upload and process CSV meter readings  
- Validate readings against seeded accounts  
- Reject duplicates and invalid formats  
- Return a summary of successful and failed readings  
- Include detailed failure reasons (**bonus**)  
- Frontend client (HTML/JS) to consume the API (**bonus**)  
- Unit tests for service and controller layers (**bonus**)

---

## 🔧 Tech Stack

- ASP.NET Core (.NET 9)
- Entity Framework Core (In-Memory Database)
- xUnit (Unit Testing)
- Plain HTML/CSS/JS client (No frameworks)
- Swagger (API documentation)

---

## 📁 Project Structure

```text
MeterReadings/
├── Ensek.MeterReadings.Api/         # Main Web API project
│   ├── Controllers/                 # API controllers
│   ├── Data/                        # EF Core DbContext & seeder
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Models/                      # Entity models
│   ├── Services/                    # Business logic
│   ├── Program.cs                   # App startup
│   └── appsettings.json
├── Ensek.MeterReadings.Tests/       # xUnit test project
│   ├── MeterReadingServiceTests.cs
│   └── MeterReadingControllerTests.cs
