FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["XakUjin2025.csproj", "XakUjin2025.csproj"]

RUN dotnet restore "./XakUjin2025.csproj"
COPY . .


WORKDIR "/src"

RUN dotnet build "./XakUjin2025.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./XakUjin2025.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && \
    apt-get install -y netcat-openbsd && \
    rm -rf /var/lib/apt/lists/* && \
    dotnet tool install --global dotnet-ef --version 8.0.*

ENV PATH="$PATH:/root/.dotnet/tools"

COPY XakUjin2025.csproj .

COPY . .

COPY --chmod=755 entrypoint.sh .

ENTRYPOINT ["./entrypoint.sh"]