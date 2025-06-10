# FoxBerry.API 🍓

## A social network backend built with ASP.NET Core.

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://github.com/abdullokhonz/FoxBerry.API/actions) [![Contributors](https://img.shields.io/github/contributors/abdullokhonz/FoxBerry.API)](https://github.com/abdullokhonz/FoxBerry.API/graphs/contributors)

---

## 🚀 Overview

**FoxBerry.API: Robust ASP.NET Core 9 backend for a social media platform. Features include JWT Auth, user profiles, posts with images, likes, comments, and a follow system, all powered by PostgreSQL.**

FoxBerry is a social media platform that allows users to share posts with images, like and comment on posts, and follow other users. This repository contains the **backend API** for the FoxBerry application, built using **ASP.NET Core 9** and **Entity Framework Core**. It provides a robust and scalable foundation for managing user data, posts, interactions, and more.

### FoxBerry Solution Structure:

This repository is the root of the `FoxBerry` solution, which currently contains the backend API project (`FoxBerry.API`).
The frontend web application (`FoxBerry.Web` - Angular) will reside in a [separate repository](https://github.com/abdullokhonz/FoxBerry.Web).

## ✨ Features

* **User Authentication & Authorization:** Secure JWT-based authentication (registration, login) and authorization for protected endpoints.
* **User Profiles:** Create, view, and update user profiles, including biography and profile pictures.
* **Post Management:** Create, view (individual and feed), update, and delete posts with image uploads.
* **Likes:** Like and unlike posts.
* **Comments:** Add, view, and delete comments on posts.
* **Follow System:** Follow and unfollow other users, view followers and following lists.
* **Image Storage:** Local file system storage for post and profile images (scalable to cloud storage for production).
* **Database:** PostgreSQL integration via Entity Framework Core.
* **API Documentation:** Self-documenting API with Swagger UI.

## 🛠️ Technologies Used

* **Backend Framework:** ASP.NET Core 9.0
* **Language:** C# 12
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core 9.0 (via `Microsoft.EntityFrameworkCore` and `Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Authentication:** JWT (JSON Web Tokens) with `Microsoft.AspNetCore.Authentication.JwtBearer` (v9.0.5)
* **Password Hashing:** BCrypt.Net-Next (v4.0.3)
* **API Documentation:** Swashbuckle (Swagger/OpenAPI) (v8.1.4)

## 📦 Getting Started

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or latest compatible)
* [PostgreSQL](https://www.postgresql.org/download/) (and optionally, [pgAdmin](https://www.pgadmin.org/download/))
* A code editor (e.g., [Visual Studio Code](https://code.visualstudio.com/), [Visual Studio](https://visualstudio.microsoft.com/downloads/))

### Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/abdullokhonz/FoxBerry.API.git
    cd FoxBerry.API # Navigate into the solution root directory
    ```

2.  **Configure Sensitive Data (Connection Strings & JWT Secret) using User Secrets:**
    * Sensitive information like database connection strings and JWT secret keys should not be committed to source control. ASP.NET Core provides a secure way to manage these for development using User Secrets.
    * Navigate into the `FoxBerry.API` **project directory** (where `FoxBerry.API.csproj` is located):
        ```bash
        cd FoxBerry.API/
        ```
    * Initialize User Secrets for the project (if not already done):
        ```bash
        dotnet user-secrets init
        ```
    * Add your database connection string:
        ```bash
        dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=FoxBerryDb;Username=your_username;Password=your_password"
        ```
        **Important:** Replace `your_username` and `your_password` with your actual PostgreSQL credentials.
    * Add your JWT secret key:
        ```bash
        dotnet user-secrets set "JwtSettings:Secret" "YourStrongAndUniqueJwtSecretKeyThatIsAtLeast32CharactersLong"
        ```
        **Important:** Replace `YourStrongAndUniqueJwtSecretKeyThatIsAtLeast32CharactersLong` with a secure, random string (minimum 32 characters). This should be the same key used during development of the Auth system.
    * You can verify your secrets are set by running `dotnet user-secrets list`.

3.  **Configure Database:**
    * Ensure your PostgreSQL server is running.
    * Create a new database for this project (e.g., `FoxBerryDb`).
    * From the `FoxBerry.API` **project directory** (where `FoxBerry.API.csproj` is), apply database migrations:
        ```bash
        dotnet ef database update
        ```
        This will create the necessary tables in your `FoxBerryDb` database.

4.  **Create Image Folders:**
    * Manually create the following folders within the `wwwroot` directory of the `FoxBerry.API` project:
        * `FoxBerry.API/wwwroot/images`
        * `FoxBerry.API/wwwroot/images/posts`
        * `FoxBerry.API/wwwroot/images/profiles`
    * This is where uploaded images will be stored locally.

### Running the Application

1.  **Navigate back to the solution root directory:**
    ```bash
    cd .. # If you are in FoxBerry.API project directory, go up one level
    ```
2.  **Run the application (from solution root, for FoxBerry.API project):**
    ```bash
    dotnet run --project FoxBerry.API/FoxBerry.API.csproj
    ```
    The API will typically run on `https://localhost:7001` (or `http://localhost:5000`).

3.  **Access Swagger UI:**
    * Open your browser and navigate to `https://localhost:7001/swagger` (replace port if different).
    * Here you can explore all API endpoints and test them.

## 🔑 API Usage (via Swagger UI)

1.  **Register a User:**
    * Navigate to `Auth` -> `POST /api/Auth/register`.
    * Provide `username`, `email`, and `password`.
    * Execute the request.

2.  **Login and Get JWT Token:**
    * Navigate to `Auth` -> `POST /api/Auth/login`.
    * Provide `email`, and `password`.
    * Execute the request. Copy the `token` from the response.

3.  **Authorize in Swagger:**
    * Click the **"Authorize"** button at the top right of the Swagger UI.
    * In the dialog, type `Bearer YourJWTToken` (replace `YourJWTToken` with the actual token you copied).
    * Click "Authorize".

4.  **Explore and Test Endpoints:**
    * Now you can use the various endpoints (e.g., `Posts`, `Likes`, `Comments`, `Follows`, `Users`) to create, read, update, and delete data.

## 🤝 Contributing

Contributions are welcome! If you have suggestions or want to contribute, please feel free to open issues or pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---