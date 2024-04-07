FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/KuCloud.Web/KuCloud.Web.csproj", "src/KuCloud.Web/"]
COPY ["src/KuCloud.Infrastructure/KuCloud.Infrastructure.csproj", "src/KuCloud.Infrastructure/"]
COPY ["src/KuCloud.Core/KuCloud.Core.csproj", "src/KuCloud.Core/"]
COPY ["src/KuCloud.UseCases/KuCloud.UseCases.csproj", "src/KuCloud.UseCases/"]
RUN dotnet restore "src/KuCloud.Web/KuCloud.Web.csproj"
COPY . .
WORKDIR "/src/src/KuCloud.Web"
RUN dotnet build "KuCloud.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "KuCloud.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KuCloud.Web.dll"]
