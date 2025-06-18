# Ensek Meter Readings API

This project is a technical assessment for Ensek. It is a C# Web API for uploading and validating customer meter readings from a CSV file.

## ✅ Features

- Upload CSV file via `POST /meter-reading-uploads`
- Validates each row:
  - Must match an existing Account ID
  - Must have a valid 5-digit meter reading (e.g., `12345`)
  - Must not be a duplicate reading (based on Account ID + DateTime)
- Returns the count of successful and failed uploads

## 🛠️ Tech Stack

- .NET 9 Web API
- EF Core (in-memory DB)
- Swagger (OpenAPI)

## 📁 CSV Format

