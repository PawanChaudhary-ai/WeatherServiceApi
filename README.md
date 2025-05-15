# Weather Service API

## Overview
Weather Service API is a .NET Core web application that provides weather information for cities based on their city IDs. The application uses an external weather data source and offers a simple REST API endpoint to retrieve weather data.

## Features
- Retrieve weather information for a specific city by its ID
- Swagger UI for API documentation and testing
- Use Postman to test the API
- Use fidler to test the API
- Robust error handling
- Configurable weather data processing

## Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Internet connection (for external weather data retrieval)

## Project Structure
- `Controllers/`: Contains the main API controller
- `Models/`: Defines data models for weather responses
- `Services/`: Implements weather data retrieval and processing logic
- `Interfaces/`: Defines contracts for services
- `Helpers/`: Utility classes for file and data processing
- `WeatherOutput/`: Directory for storing weather data output files
- `CityIDs/`: Contains city ID mappings

## API Endpoints
### Get Weather
- **URL**: `/api/weatherservice/{cityId}`
- **Method**: GET
- **Parameters**: 
  - `cityId` (integer): Unique identifier for the city
- **Success Response**: 
  - Code: 200
  - Content: Weather information for the specified city
- **Error Responses**:
  - 404: City not found
  - 500: Internal server error

## Configuration
Modify `appsettings.json` to adjust application settings.

## Running the Application
1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore`
4. Run `dotnet run`
5. Open Swagger UI at `http://localhost:5000/swagger` to test the API
6. For testing the API, use the link - http://localhost:58974/api/WeatherService/2643741

## Testing
Run unit tests using `dotnet test`

## Dependencies
- ASP.NET Core
- Swagger
- Dependency Injection
- HttpClient

## Contributing
1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
MIT License

## Contact
Pawan Kumar Chaudhary

pawan.ai.infoweb@gmail.com