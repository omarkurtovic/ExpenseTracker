FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore ExpenseTrackerWebApi/ExpenseTrackerWebApi.csproj

# Build and publish a release
RUN dotnet publish ExpenseTrackerWebApi/ExpenseTrackerWebApi.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "ExpenseTrackerWebApi.dll"]