FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/Demo.Dapr.EventProcessor.Api/Demo.Dapr.EventProcessor.Api.csproj", "Demo.Dapr.EventProcessor.Api/"]
COPY ["/Demo.Shared/Demo.Shared.csproj", "Demo.Shared/"]
RUN dotnet restore "Demo.Dapr.EventProcessor.Api/Demo.Dapr.EventProcessor.Api.csproj"
COPY . .
WORKDIR "/src/Demo.Dapr.EventProcessor.Api"
RUN dotnet build "Demo.Dapr.EventProcessor.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Dapr.EventProcessor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Dapr.EventProcessor.Api.dll"]