FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.*.props", "."]
COPY ["src/KuCloud.SharedKernel/KuCloud.SharedKernel.csproj", "src/KuCloud.SharedKernel/"]
COPY ["src/KuCloud.Infrastructure/KuCloud.Infrastructure.csproj", "src/KuCloud.Infrastructure/"]
COPY ["src/KuCloud.Core/KuCloud.Core.csproj", "src/KuCloud.Core/"]
COPY ["src/KuCloud.UseCases/KuCloud.UseCases.csproj", "src/KuCloud.UseCases/"]

COPY ["src/KuCloud.Api/KuCloud.Api.csproj", "src/KuCloud.Api/"]
RUN dotnet restore "src/KuCloud.Api/KuCloud.Api.csproj"
COPY . .

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/KuCloud.Api"
RUN dotnet publish "KuCloud.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KuCloud.Api.dll"]
