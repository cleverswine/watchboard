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
DATA_DIR="." dotnet ef migrations add SeriesStatus \
--project watchboard/watchboard.csproj \
--configuration Debug \
--output-dir Services/Database/Migrations
```

## TODO

- Admin - crud boards
- Admin - crud board / lists
- Admin - set, order interesting providers
  - on Add item, select provider based on ^^
- Admin - config provider icons
- Item - add notes
- Search - include tv, movies, or both
- Board - dynamic col-x based on list count?
- Board - if > 3 lists, too wide... ??
- Item - confirm on delete