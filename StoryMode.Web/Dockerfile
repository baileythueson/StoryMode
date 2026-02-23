# Stage 1: Base Runtime (Optimized for fast startup)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build & Restore (The "Heavy Lifting")
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# SRE Best Practice: Copy csproj files SEPARATELY to cache NuGet restore
COPY ["StoryMode.Web/StoryMode.Web.csproj", "StoryMode.Web/"]
COPY ["StoryMode.Core/StoryMode.Core.csproj", "StoryMode.Core/"]
RUN dotnet restore "StoryMode.Web/StoryMode.Web.csproj"

# Copy the rest of the code only after packages are cached
COPY . .
WORKDIR "/src/StoryMode.Web"
RUN dotnet build "StoryMode.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish (Minify and Trim)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StoryMode.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final Image (The one that goes to k3s)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoryMode.Web.dll"]