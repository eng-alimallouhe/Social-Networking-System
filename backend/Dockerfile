FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SNS.slnx", "./"]
COPY ["SNS.API/SNS.API.csproj", "SNS.API/"]
COPY ["SNS.Infrastructure/SNS.Infrastructure.csproj", "SNS.Infrastructure/"]
COPY ["SNS.Application/SNS.Application.csproj", "SNS.Application/"]
COPY ["SNS.Domain/SNS.Domain.csproj", "SNS.Domain/"]
COPY ["SNS.Shared/SNS.Shared.csproj", "SNS.Shared/"]

RUN dotnet restore "SNS.API/SNS.API.csproj"

COPY . .

WORKDIR "/src/SNS.API"
RUN dotnet publish "SNS.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SNS.API.dll"]