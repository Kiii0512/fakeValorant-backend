FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file sln và csproj vào đúng cấu trúc để cache restore layer
COPY ["NexusProtocol.API/NexusProtocol.API.csproj", "NexusProtocol.API/"]
RUN dotnet restore "NexusProtocol.API/NexusProtocol.API.csproj"

# Copy toàn bộ code còn lại và build
COPY . .
WORKDIR "/src/NexusProtocol.API"
RUN dotnet publish "NexusProtocol.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NexusProtocol.API.dll"]