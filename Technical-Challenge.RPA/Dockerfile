FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Technical-Challenge.RPA/Technical-Challenge.RPA.csproj", "Technical-Challenge.RPA/"]
COPY ["Technical-Challenge.Application/Technical-Challenge.Application.csproj", "Technical-Challenge.Application/"]
COPY ["Technical-Challenge.Infrastructure/Technical-Challenge.Infrastructure.csproj", "Technical-Challenge.Infrastructure/"]
COPY ["Technical-Challenge.Domain/Technical-Challenge.Domain.csproj", "Technical-Challenge.Domain/"]

RUN dotnet restore "Technical-Challenge.RPA/Technical-Challenge.RPA.csproj"

COPY . .
WORKDIR "/src/Technical-Challenge.RPA"
RUN dotnet publish "Technical-Challenge.RPA.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Technical-Challenge.RPA.dll"]
