# ✈️ Flight Schedule Change Detector

A .NET Console Application that detects changes in flight schedules (new or discontinued flights) based on agency subscriptions and weekly time intervals.

---

## 📌 Project Goal

To help airline agencies identify:

- **New flights** introduced by competitors.
- **Discontinued flights** that create business opportunities.

---

## 📁 Features

- Imports flight data from CSV into SQL Server.
- Supports filtering by **agency** and **date range**.
- Detects:
  - 🆕 **New flights**: no similar flight 7 days before (±30 minutes).
  - 🗑️ **Discontinued flights**: no similar flight 7 days after (±30 minutes).
- Outputs results to a `results.csv` file.
- Tracks and displays execution time.

---

## 🏗️ Architecture

- **Data Access**: Entity Framework Core
- **Imports**: `CsvHelper`
- **Database**: SQL Server
- **Layers**:
  - `Entities`: Flight, Route, Subscription
  - `Data`: DbContext
  - `Import`: CSV importing logic
  - `Services`: Change Detection logic
  - `Models`: DTO for result output

---

## 🗃️ Database Tables

- `Routes`: RouteId, OriginCityId, DestinationCityId, DepartureDate
- `Flights`: FlightId, RouteId, DepartureTime, ArrivalTime, AirlineId
- `Subscriptions`: AgencyId, OriginCityId, DestinationCityId

> ⚠️ Foreign keys are implicit; all CSV data is pre-matched.

---

## 🚀 How to Run

### 🧾 Usage

```bash
FlightScheduleChangeDetector.exe <startDate:yyyy-MM-dd> <endDate:yyyy-MM-dd> <agencyId:int>

Example : FlightScheduleChangeDetector.exe 2024-04-20 2024-04-23 1

Output
After running, a file results.csv will be generated with columns:
FlightId | OriginCityId | DestinationCityId | DepartureTime | ArrivalTime | AirlineId | Status
10 | 100 | 200 | 2024-04-20 08:00 | 2024-04-20 10:00 | 1 | New
11 | 100 | 300 | 2024-04-21 09:00 | 2024-04-21 11:00 | 1 | Discontinued