
# 🎮 TC.CloudGames

**TC.CloudGames** is a backend application developed using .NET 9, designed to simulate a digital game sales and management platform. This project was created as part of the FIAP Tech Challenge – Phase 1.

---

## 📌 Project Overview

The primary goal of this project is to provide a robust backend system that supports:

- **User Authentication**: Secure login and registration processes.
- **Game Management**: CRUD operations for digital games.
- **Sales Processing**: Handling purchases and transaction records. (Future version)
- **Administrative Controls**: Managing users and game listings.

---

## 📚 Main links and Documentation

   - Event Storming: [Miro Event Storming Diagram](https://miro.com/app/board/uXjVI4H0GgA=/)
   - Notion: [Group information & more docs](https://checkered-cod-bac.notion.site/Tech-Challenge-Net-1dfc8c4bf938806da273ca6a401f147d)
   - SonarQube: [Group Name & SonarQube info](doc/fase_01/FIAP_TC_Fase_01.pdf)
   - Youtube: [Tech Challenge video Phase 01](https://www.youtube.com/watch?v=9zyK9rb1lTs)
   - Postman Collection: [Import Collection file](doc/fase_01/TC.CloudGaming.postman_collection.json)
---

## 🛠️ Technologies Used

- **.NET 9**: Core framework for building the application.
- **ASP.NET Core** (Web API, FastEndpoints)
- **Entity Framework Core**: ORM for database interactions.
- **Serilog & Seq**: Structured logging, with sensitive data masking.
- **PostgreSQL**: Relational database management system.
- **Docker**: Containerization for consistent development and deployment environments.
- **Docker Compose**: Tool for defining and running multi-container Docker applications.

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/rdpresser/tc-cloud-games.git
   cd tc-cloud-games
   ```
2. **Setup HTTPS Development Certificate**
   #### Linux/macOS
   ```bash
   # Make the script executable
   chmod +x ./scripts/setup-devcert.sh

   # Run the setup script
   ./scripts/setup-devcert.sh
   ```

   #### Windows

   ```powershell
   # Run the setup script
   .\scripts\setup-devcert.bat
   ```

3. **Build and run the application using Docker Compose**:

   ```bash
   docker-compose up --build
   ```

   This command will set up the application along with its dependencies.

   3.1 **Or simply make docker-compose project in Visual Studio 2022 the default startup project and hit F5**

   ![Docker Compose](images/001_Docker_Compose_Startup_Project.png)

4. **Access the application**:

   Once the containers are up and running, the application can be accessed at:
   - Swagger UI: [http://localhost:55556/swagger](http://localhost:55556/swagger)
   - Health checks: [http://localhost:55556/health](http://localhost:55556/health)
   - Seq | Structured Logs: [http://localhost:8082/](http://localhost:8082/)
   - SonarQube: [http://localhost:9000/](http://localhost:9000/)

   ### Web API Login
   ```bash
   # Admin User
   Email: admin@admin.com
   Password: Admin@123

   # Regular User
   Email: user@user.com
   Password: User@123
   ```

   To access the database, you can use the pgAdmin4 PostgreSQL client with the provided link below:
   - pgAdmin: [http://localhost:15432/](http://localhost:15432/)
   ```bash
   # Credentials for pgAdmin4:
   Username: admin@admin.com
   Password: admin
   ```
   ![pgAdmin](images/003_pgAdmin_login_screen.png)

### Database Login on localhost server
- After logging in to pgAdmin, you can access the database using the following credentials:

![database login](images/004_database_server_login.png)

```bash
# Credentials for PostgreSQL:
Username: postgres
Password: postgres
```

### Database Structure

The application will create a database named `tc_cloud_games` with the following tables:
- `Users`
- `Games`

![Tables](images/002_ER_Diagram.png)

---

## 📂 Project Structure

```
tc-cloud-games/
├── doc/                   # Project documentation files
├── src/                   # Application source code
├── test/                  # Unit and integration tests
├── scripts/               # Utility scripts for setup and maintenance
├── docker-compose.yml     # Docker Compose configuration
├── Dockerfile             # Dockerfile for building the application image
├── README.md              # Project documentation
└── ...
```

---

## 🧪 Running Tests

To execute the test suite:

```bash
# Run tests for solution in the root folder of the application
dotnet test TC.CloudGames.sln
```

Ensure that the application is not running when executing tests to avoid port conflicts.

---

## 📄 Features

- **User Management** 
   - Add Users: Create new users with roles such as Admin or User.
   - Login: Authenticate users and manage sessions.
   - List Users: Retrieve a paginated list of users with filtering and sorting options.
- **Game Management**
   - Create Games: Add new games to the platform with detailed metadata, including genre, platform, price, and system requirements.
   - List Games: Retrieve a paginated list of games with filtering and sorting options.
   - Get Game Details: Fetch detailed information about a specific game.
