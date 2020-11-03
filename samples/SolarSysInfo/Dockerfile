FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# publish app
COPY . .
WORKDIR /app/samples/SolarSysInfo
RUN dotnet restore
RUN dotnet publish -c release -o out

## run app
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/samples/SolarSysInfo/out ./

ENTRYPOINT ["dotnet", "SolarSysInfo.dll"]