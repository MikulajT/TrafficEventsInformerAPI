FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

# Set environment variables:
#	1. Production
#	2. Docker
#	3. Debug
ENV ASPNETCORE_ENVIRONMENT Production
ENV DOTNET_ENVIRONMENT Production
ENV PORT 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TrafficEventsInformer.csproj", "."]
RUN dotnet restore "./TrafficEventsInformer.csproj"
COPY . .
WORKDIR "/src/."

# Switch between Release/Docker/Debug
RUN dotnet build "TrafficEventsInformer.csproj" -c Production -o /app/build

# Switch between Release/Debug
FROM build AS publish
RUN dotnet publish "TrafficEventsInformer.csproj" -c Production -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrafficEventsInformer.dll"]