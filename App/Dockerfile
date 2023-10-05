#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["App/EmpireApp/EmpireApp.csproj", "App/EmpireApp/"]
COPY ["App/Libraries/Empire.DataAccess/Empire.DataAccess.csproj", "App/Libraries/Empire.DataAccess/"]
COPY ["App/Libraries/Empire.Models/Empire.Models.csproj", "App/Libraries/Empire.Models/"]
COPY ["App/Libraries/Empire.Services/Empire.Services.csproj", "App/Libraries/Empire.Services/"]
COPY ["App/Libraries/Empire.Utilities/Empire.Utilities.csproj", "App/Libraries/Empire.Utilities/"]
RUN dotnet restore "App/EmpireApp/EmpireApp.csproj"
COPY . .
WORKDIR "/src/App/EmpireApp"
RUN dotnet build "EmpireApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmpireApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmpireApp.dll"]