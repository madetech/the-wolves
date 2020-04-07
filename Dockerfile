# Gets the official Docker image for .NET from MS
FROM microsoft/dotnet:2.2-sdk

# Sets the working directory
WORKDIR /app

# Copies the current directory to the image
COPY . ./

RUN dotnet restore

RUN dotnet publish -o out

CMD ["dotnet", "CryptoTechReminderSystem.Main/out/CryptoTechReminderSystem.Main.dll"]
