# Use the official ASP.NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WeddingUI.Server/WeddingUI.Server.csproj", "./"]
RUN dotnet restore "WeddingUI.Server.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image for the final output
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WeddingUI.Server.dll"]
