# Create build image using SDK
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build

# Sets the working directory
WORKDIR /src

# Copies the current directory to the image
COPY . ./

# Restore the app and dependancies
RUN dotnet restore CryptoTechReminderSystem.Main/CryptoTechReminderSystem.Main.csproj

# Publish app
RUN dotnet publish -o out CryptoTechReminderSystem.Main/CryptoTechReminderSystem.Main.csproj

# Create runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS release

# Copy compiled app into new folder
RUN mkdir /app
COPY --from=build /src/CryptoTechReminderSystem.Main/out /app
WORKDIR /app

# Run
ENTRYPOINT ["dotnet", "CryptoTechReminderSystem.Main.dll"]
