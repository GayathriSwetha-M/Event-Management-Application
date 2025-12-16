-- PostgreSQL Database Schema for Event Management System

-- Create Users table
CREATE TABLE "Users" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" character varying(100) NOT NULL,
    "EmailOrPhone" character varying(100) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL
);

-- Create Events table
CREATE TABLE "Events" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Title" character varying(150) NOT NULL,
    "Description" text NOT NULL,
    "Venue" character varying(150) NOT NULL,
    "EventDate" date NOT NULL,
    "EventTime" time without time zone NOT NULL,
    "Capacity" integer NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL
);

-- Create Bookings table
CREATE TABLE "Bookings" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" uuid NOT NULL,
    "EventId" uuid NOT NULL,
    "NumberOfSeats" integer NOT NULL,
    "Status" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("EventId") REFERENCES "Events"("Id") ON DELETE CASCADE
);

-- Create RefreshTokens table
CREATE TABLE "RefreshTokens" (
    "Id" uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    "Token" text NOT NULL,
    "UserId" uuid NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "IsRevoked" boolean NOT NULL,
    "RevokedAt" timestamp with time zone,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

-- Create Indexes
CREATE UNIQUE INDEX "IX_Users_EmailOrPhone" ON "Users" ("EmailOrPhone");
CREATE INDEX "IX_Bookings_EventId" ON "Bookings" ("EventId");
CREATE UNIQUE INDEX "IX_Bookings_UserId_EventId" ON "Bookings" ("UserId", "EventId");
CREATE INDEX "IX_RefreshTokens_UserId" ON "RefreshTokens" ("UserId");
CREATE UNIQUE INDEX "IX_RefreshTokens_Token" ON "RefreshTokens" ("Token");
