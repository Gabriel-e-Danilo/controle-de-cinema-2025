# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG RESOURCE_REAPER_SESSION_ID="00000000-0000-0000-0000-000000000000"
LABEL "org.testcontainers.resource-reaper-session"=$RESOURCE_REAPER_SESSION_ID
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ControleDeCinema.WebApp/ControleDeCinema.WebApp.csproj", "ControleDeCinema.WebApp/"]
COPY ["ControleDeCinema.Aplicacao/ControleDeCinema.Aplicacao.csproj", "ControleDeCinema.Aplicacao/"]
COPY ["ControleDeCinema.Dominio/ControleDeCinema.Dominio.csproj", "ControleDeCinema.Dominio/"]
COPY ["ControleDeCinema.Infraestrutura.Orm/ControleDeCinema.Infraestrutura.Orm.csproj", "ControleDeCinema.Infraestrutura.Orm/"]
RUN dotnet restore "./ControleDeCinema.WebApp/ControleDeCinema.WebApp.csproj"
COPY . .
WORKDIR "/src/ControleDeCinema.WebApp"
RUN dotnet build "./ControleDeCinema.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ControleDeCinema.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ControleDeCinema.WebApp.dll"]