# Stage 1: Build với .NET SDK 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj và restore package
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code và build Release
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Chạy runtime với ASP.NET 10.0
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render gán cổng qua biến PORT
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "NexusProtocol.API.dll"]