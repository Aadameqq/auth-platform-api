FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /home/app

COPY  src/Api/Api.csproj ./Api/
COPY  src/Core/Core.csproj ./Core/
COPY  src/Infrastructure/Infrastructure.csproj ./Infrastructure/

RUN dotnet restore ./Api
RUN dotnet restore ./Core
RUN dotnet restore ./Infrastructure

COPY  src .

RUN dotnet publish ./Api --configuration Development -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /home/app

COPY --from=build /home/app/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]
