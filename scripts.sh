#!/bin/bash

case "$1" in
  quick-start)
    docker-compose -f docker-compose.dev.yml up app
    ;;
  services)
    docker-compose -f docker-compose.dev.yml up database smtp redis
    ;;
  run)
    dotnet run --project ./src/Api/Api.csproj
    ;;
  watch)
    dotnet watch --project ./src/Api/Api.csproj
    ;;
  create-migration)
    dotnet ef migrations add $2 --project ./src/Infrastructure --startup-project ./src/Api
    ;;
  push-migration)
    dotnet ef database update --project ./src/Infrastructure --startup-project ./src/Api
    ;;
  style)
    dotnet format style --verify-no-changes && dotnet format analyzers --verify-no-changes
    ;;
esac
