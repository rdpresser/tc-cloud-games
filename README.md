# TC.CloudGames

TC.CloudGames is a cloud-based gaming platform built using modern software development principles, including **Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)**. The project is structured into distinct layers (API, Application, Domain, and Infrastructure) and leverages **FastEndpoints** for high-performance API development. It also uses **Dapper** for efficient database access, ensuring optimal performance.

## Features

### User Management
- **Add Users**: Create new users with roles such as `Admin` or `User`.
- **Login**: Authenticate users and manage sessions.
- **List Users**: Retrieve a paginated list of users with filtering and sorting options.

### Game Management
- **Create Games**: Add new games to the platform with detailed metadata, including genre, platform, price, and system requirements.
- **List Games**: Retrieve a paginated list of games with filtering and sorting options.
- **Get Game Details**: Fetch detailed information about a specific game.

## Tech Stack

### Architecture
- **Clean Architecture**: Separation of concerns into API, Application, Domain, and Infrastructure layers.
- **CQRS**: Command Query Responsibility Segregation for clear separation of read and write operations.
- **DDD**: Domain-Driven Design principles for modeling the core business logic.

### Frameworks and Libraries
- **FastEndpoints**: High-performance API framework for building RESTful endpoints.
- **Dapper**: Lightweight ORM for efficient database access.
- **Ardalis.Result**: Standardized result handling for commands and queries.

### Infrastructure
- **Docker**: Containerized deployment with a `docker-compose` setup.
- **PostgreSQL**: Relational database for storing application data.

## Project Structure

- **API Layer**: Contains the FastEndpoints-based REST API.
  - Example: `GetGameEndpoint`, `GetUserListEndpoint`.
- **Application Layer**: Implements business logic and CQRS handlers.
  - Example: `GetGameListQueryHandler`, `CreateGameCommandHandler`.
- **Domain Layer**: Contains core domain models and business rules.
  - Example: `Game`, `User`, `Role`.
- **Infrastructure Layer**: Handles database access and external integrations.
  - Example: `ISqlConnectionFactory` for managing database connections.

## API Endpoints

### User Endpoints
1. **Add User**: `POST /users`
   - Request: 
     {
   "firstName": "John",
   "lastName": "Doe",
   "email": "john.doe@example.com",
   "role": "User"
 }

    - Response: `201 Created`
2. **Login**: `POST /users/login`
   - Request: 
     {
   "email": "john.doe@example.com",
   "password": "password123"
 }

    - Response: 
     
     {
   "token": "jwt-token"
 }

 3. **List Users**: `GET /users`
   - Query Parameters: `?filter=John&pageNumber=1&pageSize=10&sortBy=FirstName&sortDirection=ASC`
   - Response: 
      [
   {
     "id": "guid",
     "firstName": "John",
     "lastName": "Doe",
     "email": "john.doe@example.com",
     "role": "User"
   }
 ]


### Game Endpoints
1. **Create Game**: `POST /games`
   - Request: 
 {
   "name": "Game Name",
   "releaseDate": "2025-01-01",
   "price": 59.99,
   "genre": "Action",
   "platform": ["Windows", "Xbox"]
 }

        - Response: `201 Created`
2. **List Games**: `GET /games`
   - Query Parameters: `?filter=Action&pageNumber=1&pageSize=10&sortBy=Name&sortDirection=ASC`
   - Response: 
     

   [
   {
     "id": "guid",
     "name": "Game Name",
     "releaseDate": "2025-01-01",
     "price": 59.99,
     "rating": 4.5
   }
 ]

  3. **Get Game Details**: `GET /games/{id}`
   - Response: 
       {
   "id": "guid",
   "name": "Game Name",
   "releaseDate": "2025-01-01",
   "price": 59.99,
   "genre": "Action",
   "platform": ["Windows", "Xbox"]
 }
 

## Running the Application

### Prerequisites
- **Docker**: Ensure Docker is installed on your machine.
- **.NET 9 SDK**: Required for local development.

### Steps to Run
1. Clone the repository:
   
   git clone https://github.com/your-repo/tc-cloud-games.git cd tc-cloud-games


2. Build and run the application using Docker Compose:
   
docker-compose up --build

3. The application will be available at:
   - API: `http://localhost:5000`
   - PostgreSQL: `localhost:5432`

### Docker Configuration
- **API**: Exposed on port `5000`.
- **Database**: PostgreSQL is exposed on port `5432`.

### Environment Variables
- Configure the following environment variables in the `.env` file:
  - `DB_CONNECTION_STRING`: Connection string for the PostgreSQL database.
  - `JWT_SECRET`: Secret key for JWT authentication.

## Highlights

### Dapper for Performance
- **Why Dapper?**
  - Lightweight and fast compared to traditional ORMs.
  - Direct SQL execution for better control and performance.
- **Usage in the Project**:
  - Used in query handlers like `GetGameListQueryHandler` and `GetUserListQueryHandler` for efficient data retrieval.

### Clean Architecture Benefits
- Clear separation of concerns.
- Easy to test and maintain.
- Scalable for future feature additions.
