FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["bank.api/bank.api.csproj", "bank.api/"]
RUN dotnet restore "bank.api/bank.api.csproj"
COPY . .
WORKDIR "/src/bank.api"
RUN dotnet build "bank.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bank.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "bank.api.dll"]