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

## TODO

- Admin - crud boards
- Admin - crud board / lists
- Admin - set, order interesting providers
  - on Add item, select provider based on ^^
- Admin - config provider icons
- Item - links to imdb, etc
- Item - add notes
- Item - move to other board
- Search - include tv, movies, or both
- Search - fall back to poster when no backdrop
- Board - dynamic col-x based on list count?
- Board - if > 3 lists, too wide... ??
- Item - image select list - number them 1,2,3,etc
- Item - confirm on delete
- Item - show status (in progress, ended, etc)