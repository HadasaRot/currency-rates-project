# Currency Rates Project

A full-stack application for managing and displaying foreign currency exchange rates against the Israeli Shekel (ILS), with automatic daily import from Bank of Israel.

## Project Overview

A system for managing and graphically displaying foreign currency exchange rates (USD, GBP, CHF, SEK) against the Israeli Shekel. The system includes:
- Automatic daily import of data from Bank of Israel
- RESTful API for retrieving data across different time periods
- Interactive user interface with views for week, month, half-year, and year
- Interactive charts using Chart.js
- Summary cards showing changes and percentage variations

## Project Requirements

### 1.1 General Requirements
Display a graph of foreign currency values in Shekels for multiple time periods:
- Display data for 4 different currency types
- Allow users to select a time period: Week, Month, Half-Year, and Year
  - **Week**: Display 7 values from the last week (current day backwards)
  - **Month**: Display daily currency values from current day to the same day in the previous month
  - **Half-Year**: Display the value on the current day of the month for the last 6 months
  - **Year**: Display the value on the current day of the month for the last 12 months

### 1.2 Currency Graph
Interactive line chart displaying multiple currencies simultaneously with color-coded lines.

### 1.3 Data Source
- Data is read from Bank of Israel
- Data is based on representative exchange rates
- Supported currencies from Bank of Israel file:
  - 01 - USD (United States Dollar) - דולר
  - 02 - GBP (British Pound) - לירה
  - 03 - SEK (Swedish Krona) - כתר
  - 05 - CHF (Swiss Franc) - פרנק

### 1.4 Implementation Guidelines
1. ✅ Create database table and prepare function to populate table data
2. ✅ Run the function and insert values for 4 currencies
3. ✅ Implement the graph visualization
4. ✅ Enable period selection to display values for all 4 currencies on the graph/table
5. ✅ Handle edge cases: missing months (February), non-trading days (weekends and holidays)

**Additional Notes:**
- Currency flags are not required - currency name is sufficient
- Full visual design is not mandatory - using a library or ready-made component for graph drawing is acceptable
- Special attention to missing months (like February) and non-trading days (weekends and holidays)

### 1.5 Reading Data from Bank of Israel
Data is fetched from Bank of Israel API. For documentation, visit: https://www.boi.org.il/qawebsite/

**Reference**: "What happened to the XML exchange rates at boi.org.il/currency.xml"

The old XML endpoint has been replaced with a modern SDMX API that provides CSV data. This project uses the new API endpoint.

## Architecture

The project consists of three main components:

### Backend (ASP.NET Core 8.0)
- **Framework**: .NET 8.0
- **Database**: SQL Server עם Entity Framework Core
- **API**: RESTful API עם Swagger documentation
- **Scheduled Jobs**: Background service לייבוא יומי אוטומטי

### Frontend (Angular 18)
- **Framework**: Angular 18 (Standalone Components)
- **Charts**: Chart.js
- **Styling**: CSS מותאם אישית
- **HTTP Client**: Angular HttpClient

### Database (SQL Server)
- טבלת `CurrencyRates` עם אינדקסים מותאמים
- Unique constraint על צמד מטבע-תאריך

## System Requirements

### Backend
- .NET 8.0 SDK
- SQL Server (LocalDB or Full)
- Visual Studio 2022 or VS Code

### Frontend
- Node.js 18+ and npm
- Angular CLI 18

## Installation and Setup

### 1. Database Setup

```sql
-- Create database
CREATE DATABASE CurrencyDB;
GO

-- Run the script
USE CurrencyDB;
-- Execute database/create_tables.sql
```

### 2. Backend Setup

```bash
cd backend/CurrencyRates/CurrencyRates

# Update connection string in appsettings.json
# "Server=.;Database=CurrencyDB;Trusted_Connection=True;TrustServerCertificate=True;"

# Run the application
dotnet run
```

The API will be available at: `https://localhost:7019`

### 3. Frontend Setup

```bash
cd frontend/currency-graph-app

# Install dependencies
npm install

# Run the application
npm start
```

The application will be available at: `http://localhost:4200`

## API Endpoints

### Currency Controller

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/currency/week` | Exchange rates for the last 7 days |
| GET | `/api/currency/month` | Exchange rates for the last month |
| GET | `/api/currency/half-year` | Exchange rates for the last 6 months (monthly sampling) |
| GET | `/api/currency/year` | Exchange rates for the last 12 months (monthly sampling) |
| POST | `/api/currency/import-boi` | Manual import of data from Bank of Israel |

### Sample Response

```json
[
  {
    "id": 1,
    "currencyCode": "USD",
    "currencyName": "דולר",
    "rate": 3.72,
    "rateDate": "2026-04-20T00:00:00"
  }
]
```

## Project Structure

### Backend Structure

```
backend/CurrencyRates/CurrencyRates/
├── Controllers/
│   └── CurrencyController.cs      # API endpoints
├── Data/
│   └── AppDbContext.cs            # EF Core context
├── Jobs/
│   └── DailyCurrencyJob.cs        # Scheduled job (runs daily at 4:00 PM)
├── Models/
│   ├── CurrencyRate.cs            # Entity model
│   └── BoIResponse.cs             # Bank of Israel API response model
├── Services/
│   ├── CurrencyDataService.cs     # Business logic for data retrieval
│   └── CurrencyImportService.cs   # Import service from Bank of Israel
└── Program.cs                     # Configuration & Dependency Injection
```

### Frontend Structure

```
frontend/currency-graph-app/src/app/
├── features/currency/
│   ├── components/
│   │   └── currency-chart/        # Main chart component
│   ├── models/
│   │   └── cur.model.ts           # TypeScript interfaces
│   └── services/
│       └── currency.service.ts    # HTTP service
└── app.component.ts               # Root component
```

### Database Structure

```
database/
├── create_tables.sql              # Table and index creation
├── queries.sql                    # Helper queries
└── seed_data.sql                  # Demo data
```

## Key Features

### 1. Automatic Import
- Background service runs daily at 4:00 PM
- Imports data from the last year from Bank of Israel
- Prevents duplicates with unique constraint
- Detailed import logging

### 2. Smart Data Retrieval
- **Week**: Last 7 days with gap filling (forward-fill missing days)
- **Month**: Last month with gap filling for all days
- **Half-Year**: Monthly sampling (6 data points)
- **Year**: Monthly sampling (12 data points)

### 3. User Interface
- Tabs for switching between time periods
- Interactive line chart
- Summary cards for each currency with:
  - Current exchange rate
  - Absolute change
  - Percentage change
  - Up/down indicator

### 4. Data Handling
- Handles non-trading days (weekends and holidays) by forward-filling the last known rate
- Handles missing months (e.g., February with fewer days)
- Ensures continuous data visualization even with gaps in source data

## Supported Currencies

| Code | Name (Hebrew) | Name (English) | Chart Color |
|------|---------------|----------------|-------------|
| USD | דולר | US Dollar | Blue |
| GBP | לירה | British Pound | Green |
| CHF | פרנק | Swiss Franc | Red |
| SEK | כתר | Swedish Krona | Orange |

## Configuration

### Backend - appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CurrencyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Frontend - currency.service.ts

```typescript
private baseUrl = 'https://localhost:7019/api/currency';
```

## Implementation Details

### Gap Filling Strategy
The application implements intelligent gap filling for non-trading days:

1. **Week View**: Forward-fills missing days with the last known rate
2. **Month View**: Ensures continuous daily data by carrying forward the last available rate
3. **Half-Year/Year Views**: Uses monthly sampling, taking the most recent rate available for each month

### Data Import Process
1. Scheduled job runs daily at 4:00 PM
2. Fetches CSV data from Bank of Israel API for the last year
3. Parses CSV and extracts currency code, date, and rate
4. Checks for existing records to prevent duplicates
5. Inserts new records into the database
6. Returns summary: saved, skipped, errors, and total lines processed

### Time Period Calculations
- **Week**: `DateTime.Today.AddDays(-6)` to `DateTime.Today` (7 days)
- **Month**: `DateTime.Today.AddMonths(-1).AddDays(1)` to `DateTime.Today`
- **Half-Year**: 6 monthly samples from current month backwards
- **Year**: 12 monthly samples from current month backwards

## CORS Policy

The Backend is configured to allow requests from:
- `http://localhost:4200` (Angular dev server)

## Technologies

### Backend
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server
- Swagger/OpenAPI
- Background Services (Hosted Service)
- HttpClient for Bank of Israel API integration

### Frontend
- Angular 18 (Standalone Components)
- TypeScript 5.5
- Chart.js 4.5
- RxJS 7.8
- Angular HttpClient

### Database
- SQL Server
- Unique constraints and indexes for performance
- Date-based queries optimization

## Development and Customization

### Adding a New Currency
1. Update the URL in `CurrencyImportService.cs` to include the new currency code
2. Add mapping in the `MapName()` method
3. Update colors in `currency-chart.component.ts`
4. Add the currency to the datasets array in the chart rendering

### Changing Import Schedule
Edit `DailyCurrencyJob.cs`:
```csharp
var nextRun = DateTime.Today.AddHours(16); // Change to desired hour
```

### Modifying Time Periods
Edit the methods in `CurrencyDataService.cs`:
- `GetWeek()` - Adjust the number of days
- `GetMonth()` - Modify the date range calculation
- `GetHalfYear()` / `GetYear()` - Change the number of months sampled

## Troubleshooting

### Backend Cannot Connect to Database
- Verify SQL Server is running
- Ensure connection string is correct in `appsettings.json`
- Run `create_tables.sql` manually if tables don't exist
- Check SQL Server authentication mode (Windows Authentication vs SQL Server Authentication)

### Frontend Not Receiving Data
- Verify Backend is running on `https://localhost:7019`
- Check CORS policy configuration
- Open Developer Tools and check Console for errors
- Verify the API URL in `currency.service.ts` matches the backend URL

### Import Fails
- Check internet connection
- Verify Bank of Israel API URL is valid
- Check Console logs for detailed error messages
- Ensure database has write permissions
- Check if unique constraint is causing conflicts (duplicate data)

### Chart Not Displaying
- Verify Chart.js is properly installed (`npm install`)
- Check browser console for JavaScript errors
- Ensure data is being received from the API (check Network tab)
- Verify canvas element exists in the DOM

### Missing Data for Certain Days
- This is expected for weekends and holidays (non-trading days)
- The system uses forward-fill logic to display the last known rate
- Check if data exists in the database for those dates

## Bank of Israel API Integration

The application fetches data from Bank of Israel's SDMX API:
- **Endpoint**: `https://edge.boi.gov.il/FusionEdgeServer/sdmx/v2/data/dataflow/BOI.STATISTICS/EXR/1.0/`
- **Format**: CSV
- **Data Type**: Representative Exchange Rates (OF00)
- **Currencies**: RER_USD_ILS, RER_GBP_ILS, RER_SEK_ILS, RER_CHF_ILS
- **Time Range**: Last year from current date

For more information, visit: https://www.boi.org.il/qawebsite/

## License

Private project for learning and development purposes.

## Author

Created as part of a currency exchange rates management project.
