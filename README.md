# dotNet_5785_5864_8145

## Overview

This repository is a C# project designed to demonstrate layered architecture principles and the implementation of core business logic for managing volunteers, calls, assignments, and administrative operations. The project is organized into clear layers:
- **Data Access Layer (DAL)**: Handles data storage and retrieval using lists and XML files.
- **Business Logic Layer (BL)**: Encapsulates application rules and operations.
- **Helpers and Managers**: Provide utilities for geocoding, observer management, and simulation of business scenarios.

## Main Features

- **Volunteer Management**: Add, update, and manage volunteers with persistent storage.
- **Call Management**: Track and manage calls, including assignment of volunteers.
- **Assignments**: Link volunteers to calls and maintain assignment records.
- **Admin Operations**: Configure global settings, run simulations, and manage application clock.
- **Geocoding Service**: Convert addresses to latitude and longitude using OpenStreetMap and OpenRouteService APIs.
- **Observer Infrastructure**: Event-driven updates for entities and configuration changes.
- **Simulator**: Simulate business scenarios such as clock advancement and course registration.

## Project Structure

- `BL/`: Business Logic implementations and interfaces.
- `DalList/`: In-memory data source layer.
- `DalFacade/`, `DalXml/`: Data Access Layer implementations (XML-based, configurable).
- `Helpers/`: Utility classes for geocoding, admin, observer, and volunteer management.
- `BO/`, `DO/`: Business objects and data objects models.

## Setup Instructions

1. **Clone the repository**
   ```sh
   git clone https://github.com/NitayMagal0/dotNet_5785_5864_8145.git
   ```

2. **Open in Visual Studio**
   - Open the solution file in Visual Studio 2022 or newer.

3. **Restore NuGet packages**
   - Right-click on the solution and select "Restore NuGet Packages".

4. **Configure DAL**
   - Ensure the `xml/dal-config.xml` file exists and is configured with the correct DAL implementation.
   - The DAL can swap between list-based and XML-based storage by updating the config.

5. **Build and Run**
   - Build the solution.
   - Run the main application project.

## Usage

- **Volunteer Operations**: Use the BL interfaces to add, update, or retrieve volunteers.
- **Call Operations**: Manage calls and assignments via BL layer methods.
- **Admin Controls**: Use the admin API for clock management, simulation, and configuration observation.
- **Geocoding**: Utilize helper functions to convert addresses to coordinates for mapping and routing.
- **Simulation**: Run provided simulation scenarios to test business logic.

## Example: Geocoding Usage

```csharp
var coordinates = Helpers.Tools.GetCoordinates("1600 Amphitheatre Parkway, Mountain View, CA");
// coordinates => (Latitude, Longitude)
```

## Technologies Used

- C#
- .NET
- XML for configuration and data persistence
- OpenStreetMap & OpenRouteService APIs for geocoding
---
