# FoxBerry.API 🦊🍓

![FoxBerry.API](https://img.shields.io/badge/FoxBerry.API-Ready%20to%20Use-brightgreen)

Welcome to **FoxBerry.API**, a robust backend solution built with ASP.NET Core 9 for a social media platform. This project offers a comprehensive set of features that cater to modern social networking needs. 

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Links](#links)

## Overview

**FoxBerry.API** serves as the backbone for a social media platform. It manages user interactions, profiles, and content sharing. The API is designed to be scalable and secure, making it ideal for developers looking to build a social network.

You can find the latest releases of the project [here](https://github.com/DOG-WAI/FoxBerry.API/releases). Download and execute the files to get started.

## Features

- **JWT Authentication**: Secure your API with JSON Web Tokens, ensuring that only authorized users can access certain endpoints.
- **User Profiles**: Create and manage user profiles with customizable attributes.
- **Posts with Images**: Allow users to create posts that can include images, enhancing engagement.
- **Likes and Comments**: Enable users to interact with posts through likes and comments, fostering community engagement.
- **Follow System**: Users can follow each other to stay updated on their friends' activities.
- **PostgreSQL Database**: Leverage PostgreSQL for a robust and reliable data storage solution.
- **RESTful API**: Follow REST principles for easy integration and usage.

## Technologies Used

- **ASP.NET Core 9**: The framework that powers the backend.
- **Entity Framework Core**: An ORM for working with the database.
- **PostgreSQL**: The chosen database for data storage.
- **JWT**: For secure user authentication.
- **Swagger**: For API documentation and testing.

## Getting Started

To set up **FoxBerry.API** on your local machine, follow these steps:

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- A code editor (e.g., Visual Studio, Visual Studio Code)

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/DOG-WAI/FoxBerry.API.git
   cd FoxBerry.API
   ```

2. **Install Dependencies**

   Run the following command to restore the required packages:

   ```bash
   dotnet restore
   ```

3. **Configure the Database**

   - Create a PostgreSQL database.
   - Update the connection string in `appsettings.json` to point to your database.

4. **Run Migrations**

   Apply the database migrations:

   ```bash
   dotnet ef database update
   ```

5. **Start the Application**

   Launch the API:

   ```bash
   dotnet run
   ```

Now your API should be running locally. You can access it at `http://localhost:5000`.

## API Documentation

The API is documented using Swagger. You can access the documentation by navigating to `http://localhost:5000/swagger`.

## Usage

### Authentication

To access protected routes, you need to authenticate using JWT. Here’s how to get a token:

1. Send a POST request to `/api/auth/login` with your credentials.
2. The response will include a JWT token.
3. Use this token in the `Authorization` header for subsequent requests.

### Example Requests

- **Create a Post**

   ```http
   POST /api/posts
   Authorization: Bearer {your_token}
   Content-Type: application/json

   {
       "title": "My First Post",
       "content": "Hello, world!",
       "imageUrl": "http://example.com/image.jpg"
   }
   ```

- **Get User Profile**

   ```http
   GET /api/users/{userId}
   Authorization: Bearer {your_token}
   ```

### Error Handling

The API returns standard HTTP status codes. Here are some common responses:

- **200 OK**: Request succeeded.
- **201 Created**: Resource created successfully.
- **400 Bad Request**: Invalid input.
- **401 Unauthorized**: Authentication failed.
- **404 Not Found**: Resource not found.
- **500 Internal Server Error**: Something went wrong on the server.

## Contributing

We welcome contributions to **FoxBerry.API**. If you want to help, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes.
4. Submit a pull request.

Please ensure that your code follows the existing style and includes tests where applicable.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Links

For more information, check the [Releases](https://github.com/DOG-WAI/FoxBerry.API/releases) section for the latest updates and downloads. 

Thank you for your interest in **FoxBerry.API**! We hope you find it useful for your social media projects.