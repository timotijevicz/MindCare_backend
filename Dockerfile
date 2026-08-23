# STAGE 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Railway dodeljuje pravi port preko PORT environment varijable u runtime-u (Program.cs to čita).
# Ovo je samo podrazumevana vrednost za lokalno Docker Compose pokretanje.
ENV ASPNETCORE_URLS=http://+:8085
EXPOSE 8085

ENTRYPOINT ["dotnet", "MentalHealthApi.dll"]
