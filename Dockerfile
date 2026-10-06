FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY RebooTech.Fruitlogix.Platform/*.csproj RebooTech.Fruitlogix.Platform/
RUN dotnet restore ./RebooTech.Fruitlogix.Platform
COPY . .
RUN dotnet publish ./RebooTech.Fruitlogix.Platform -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "RebooTech.Fruitlogix.Platform.dll"]