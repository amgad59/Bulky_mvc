#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["EmpireApp/EmpireApp.csproj", "EmpireApp/"]
COPY ["Empire.DataAccess/Empire.DataAccess.csproj", "Empire.DataAccess/"]
COPY ["Empire.Models/Empire.Models.csproj", "Empire.Models/"]
COPY ["Empire.Utilities/Empire.Utilities.csproj", "Empire.Utilities/"]
COPY ["Empire.Services/Empire.Services.csproj", "Empire.Services/"]
RUN dotnet restore "EmpireApp/EmpireApp.csproj"
COPY . .
WORKDIR "/src/EmpireApp"
RUN dotnet build "EmpireApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmpireApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmpireApp.dll"]