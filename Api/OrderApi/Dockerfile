FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base 

WORKDIR /app
EXPOSE 1444

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OrderApi/OrderApi.csproj OrderApi/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Shared/Shared.csproj Shared/

RUN dotnet restore "OrderApi/OrderApi.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "OrderApi/OrderApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderApi/OrderApi.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderApi.dll"]
 