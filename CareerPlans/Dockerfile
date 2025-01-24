FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

EXPOSE 80

# Copy everything
COPY *.sln ./
COPY CareerPlans/*.csproj ./CareerPlans/
# Restore as distinct layers
RUN dotnet restore

COPY . .
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "CareerPlans.dll"]