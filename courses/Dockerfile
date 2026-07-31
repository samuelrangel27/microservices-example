FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

EXPOSE 80

# Copy everything
COPY *.sln ./
COPY src/Courses.Api/*.csproj ./src/Courses.Api/
# Restore as distinct layers
RUN dotnet restore

COPY . .
# Build and publish a release
RUN dotnet publish -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "Courses.dll"]