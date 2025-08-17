FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../examples/DevKit.Example.Api/DevKit.Example.Api.csproj", "../examples/DevKit.Example.Api/"]
COPY ["DevKit.Api.Logging/DevKit.Api.Logging.csproj", "DevKit.Api.Logging/"]
RUN dotnet restore "../examples/DevKit.Example.Api/DevKit.Example.Api.csproj"
COPY . .
WORKDIR "/src/../examples/DevKit.Example.Api"
RUN dotnet build "./DevKit.Example.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DevKit.Example.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DevKit.Example.Api.dll"]
