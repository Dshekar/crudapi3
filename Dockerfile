FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

# Set the default logging driver to json-file with specified options
ENV DOCKER_LOGGING_DRIVER=json-file
ENV DOCKER_LOGGING_OPTIONS='{"max-size": "10m", "max-file": "5"}'

WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["crudapi2.csproj", "./"]
RUN dotnet restore "crudapi2.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "crudapi2.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "crudapi2.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "crudapi2.dll"]
