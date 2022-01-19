#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DataBase_Dev/DataBase_Dev.csproj", "DataBase_Dev/"]
RUN dotnet restore "DataBase_Dev/DataBase_Dev.csproj"
COPY . .
WORKDIR "/src/DataBase_Dev"
RUN dotnet build "DataBase_Dev.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataBase_Dev.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataBase_Dev.dll"]