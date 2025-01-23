# Watch Board

kanban board for tv/movie watching

## Docker

Start

```shell
mkdir $HOME/.config/blazboard # first time only
docker compose up -d
```

Build New Image and Start

```shell
docker compose down && docker rmi blazboard && docker compose up -d
```
s
## EF

```shell

DATA_DIR="." dotnet ef migrations add Init \
--project watchboard/watchboard.csproj \
--configuration Debug \
--output-dir Services/Database/Migrations
```

## TMDB

Set Bearer token

`dotnet user-secrets set "Tmdb:Token" "..."`
