# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Copy the .csproj file and restore
COPY ./ECommerceApi.csproj ./
RUN dotnet restore ./ECommerceApi.csproj

# Copy all source code
COPY . ./

WORKDIR /src
RUN dotnet publish -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ECommerceApi.dll"]
