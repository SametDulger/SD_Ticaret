# Use the official .NET 9 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files
COPY ["SDTicaret.API/SDTicaret.API.csproj", "SDTicaret.API/"]
COPY ["SDTicaret.Application/SDTicaret.Application.csproj", "SDTicaret.Application/"]
COPY ["SDTicaret.Core/SDTicaret.Core.csproj", "SDTicaret.Core/"]
COPY ["SDTicaret.Infrastructure/SDTicaret.Infrastructure.csproj", "SDTicaret.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "SDTicaret.API/SDTicaret.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/SDTicaret.API"
RUN dotnet build "SDTicaret.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SDTicaret.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM base AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Set the entry point
ENTRYPOINT ["dotnet", "SDTicaret.API.dll"] 