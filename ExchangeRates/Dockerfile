FROM microsoft/aspnetcore:2.2-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.2-nanoserver-sac2016 AS build
WORKDIR /src
COPY *.sln ./
COPY ExchangeRates/ExchangeRates.csproj ExchangeRates/
RUN dotnet restore
COPY . .
WORKDIR /src/ExchangeRates
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ExchangeRates.dll"]
