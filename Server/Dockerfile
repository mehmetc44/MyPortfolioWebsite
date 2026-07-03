# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Server.csproj", "./"]
RUN dotnet restore "Server.csproj"

# Copy the rest of the source code
COPY . .
RUN dotnet build "Server.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Default environment variables
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Copy published application
COPY --from=publish /app/publish .

# Create directories for persistent volumes
RUN mkdir -p /app/Db /app/wwwroot/uploads

ENTRYPOINT ["dotnet", "Server.dll"]
