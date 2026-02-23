# Use dotnet 10 sdk
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["StoryMode.sln", "./"]
COPY ["StoryMode.API/StoryMode.API.csproj", "StoryMode.API/"]
COPY ["StoryMode/StoryMode.csproj", "StoryMode/"]
RUN dotnet restore "StoryMode.API/StoryMode.API.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/StoryMode.API"
RUN dotnet publish -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# SRE Vital: Expose the port we monitor
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "StoryMode.API.dll"]