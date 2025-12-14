# 🎉 Event Management System

A full-stack Event Management System built with **ASP.NET Core Web API (.NET 8)** and **React (Vite)**.  
Users can browse events and book multiple seats, while admins manage events, users, and bookings using a secure dashboard.

---

## ✨ Features

- User & Admin authentication (JWT + Refresh Tokens)
- Browse events and view event details
- Multi-seat booking support
- Admin dashboard for managing events, users, and bookings
- Role-based access control
- Secure password hashing using BCrypt

---

## 🛠️ Tech Stack

### Backend
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL
- JWT Authentication
- FluentValidation

### Frontend
- React (Vite)
- React Router
- Axios
- Context API

---

## ⚙️ Backend Setup (API)

### Prerequisites
- .NET 8 SDK
- PostgreSQL
- Entity Framework Core CLI

### Install EF Core CLI (if not installed)
```bash
dotnet tool install --global dotnet-ef
```

### Configure Database
Update the PostgreSQL connection string in:

```
appsettings.json
```

### Run EF Core Migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Build and Run Backend
```bash
dotnet build
dotnet run
```

Backend API will run at:

```
https://localhost:7205
```

Swagger UI:

```
https://localhost:7205/swagger
```

---

## 💻 Frontend Setup (UI)

### Navigate to UI Folder
```bash
cd event-management-ui
```

### Install Dependencies
```bash
npm install
```

### Run Frontend
```bash
npm run dev
```

Frontend will run at:

```
http://localhost:5173
```

---

## 🔐 Roles

- **User**: Browse events, book seats, view bookings
- **Admin**: Manage events, users, and bookings

### Admin Creation Endpoint
```
POST /api/adminsetup/create-admin
```

This project is licensed under the MIT License.

