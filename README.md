# Watch Board

kanban board for tv/movie watching

## Start

things to set up (first time only)

```shell
mkdir $HOME/.config/watchboard
echo "{ \"TmdbToken\": \"YOUR TMDB TOKEN HERE\" }" > $HOME/.config/watchboard/appsettings.json
```

## Docker

Tear down old image, build new image, start container

```shell
docker compose down && docker rmi watchboard && docker compose up -d
```

## EF

create migrations when entity models change

```shell
DATA_DIR="." dotnet ef migrations add Init \
--project watchboard/watchboard.csproj \
--configuration Debug \
--output-dir Services/Database/Migrations
```