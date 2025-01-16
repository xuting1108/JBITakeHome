# JBITakeHome

This project performs matrix multiplication using a distributed service to fetch and validate matrices. It demonstrates the integration of asynchronous HTTP operations, matrix computation, and hash validation.

## Features

- Fetch matrix rows from an external API.
- Perform matrix multiplication.
- Validate the MD5 hash of the resulting matrix.

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later)
- Internet connection (to access the API)
- Install Newtonsoft.Json (dotnet add package Newtonsoft.Json)
## How to Run

1. Clone the repository:
   ```bash
   git clone https://github.com/xuting1108/JBITakeHome.git
   cd TakeHome

2. Build the project:
    dotnet build

3. Run the application:
    dotnet run

## Project Structure

Program.cs:
    Main entry point of the application.

MatrixService.cs:
    Contains logic for matrix initialization, fetching rows, multiplication, and hash validation.