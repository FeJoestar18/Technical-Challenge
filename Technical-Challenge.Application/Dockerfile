FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Technical-Challenge.WebAPI/Technical-Challenge.WebAPI.csproj", "Technical-Challenge.WebAPI/"]
COPY ["Technical-Challenge.Infrastructure/Technical-Challenge.Infrastructure.csproj", "Technical-Challenge.Infrastructure/"]
COPY ["Technical-Challenge.Application/Technical-Challenge.Application.csproj", "Technical-Challenge.Application/"]
COPY ["Technical-Challenge.Domain/Technical-Challenge.Domain.csproj", "Technical-Challenge.Domain/"]

RUN dotnet restore "Technical-Challenge.WebAPI/Technical-Challenge.WebAPI.csproj"

COPY . .
WORKDIR "/src/Technical-Challenge.WebAPI"
RUN dotnet publish "Technical-Challenge.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "Technical-Challenge.WebAPI.dll"]
