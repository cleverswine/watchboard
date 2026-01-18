## Watchboard — quicknotes for AI code agents

Purpose: help an AI contributor get productive quickly in this repository by describing the architecture, workflows, conventions and concrete examples to reference when making edits.

- Repo root project: `watchboard/` — a .NET 9 minimal web app that serves Razor components and a small API surface. The UI pieces live in `watchboard/Pages` and `watchboard/Pages/Partials`.
- Frontend directories: `client-app-vue/` and `client-app-vuetify/` — separate Vite-based frontends (TypeScript). They are not required to edit server behavior but contain useful patterns for client-side UI.

Architecture & runtime
- Entry point: `watchboard/Program.cs` — minimal host. Important behaviours:
  - Loads a JSON config file from `$DATA_DIR/appsettings.json` (DATA_DIR default is set in `Program.cs` to "/data" if envvar missing).
  - Registers an `ItemWorker` hosted service only when NOT in Development: `if (!builder.Environment.IsDevelopment()) AddHostedService<ItemWorker>();`.
  - Registers EF Core SQLite database with file at `$DATA_DIR/watchboard.db` via `AppDbContext`.
  - Registers `ITmDb` HTTP client and `IRepository` implementation (`Services/Repository.cs`).

Core patterns and where to look
- Routes: Minimal-API extension methods in `watchboard/Routes/*.cs`. Example patterns:
  - `Pages.MapPages()` sets up page endpoints and returns `RazorComponentResult<T>` to render components server-side (see `Routes/Pages.cs`).
  - `RouteGroupBuilder` extension methods in `Routes/Items.cs`, `Routes/Lists.cs`, `Routes/Search.cs` implement POST/PUT/GET endpoints. Example: POST `/items/{tmDbId}?type=tv` calls `repo.AddItemToBoard(...)`.
- Data layer: `watchboard/Database/AppDbContext.cs` (EF models in `Database/Entities/`). The DB is SQLite and migrations are in `Database/Migrations/`.
- Business logic: `watchboard/Services/Repository.cs` (repository pattern used across the app). Mapping helpers are in `watchboard/Services/Mapping.cs` (contains TMDB -> domain mapping rules).
- TMDB integration: `watchboard/Services/TmDb/*` — this is the external API integration. The app expects a bearer token under the `TmdbToken` key in the JSON config loaded from DATA_DIR.
- Static assets & libs: `watchboard/wwwroot/lib/` — the repo uses a manual copy approach (see `lib/copy-dist-libs.sh`) rather than a JS bundler for those libraries.

Important commands & workflows (concrete)
- Run locally (dev):
  - Ensure your token file exists: `DATA_DIR="$HOME/.config/watchboard" && mkdir -p "$DATA_DIR" && echo '{ "TmdbToken": "YOUR_TOKEN" }' > "$DATA_DIR/appsettings.json"`
  - Run server: `dotnet run --project watchboard`
  - Note: Program.cs falls back to `/data` if `DATA_DIR` is not set — set `DATA_DIR` explicitly to avoid surprises.
- Docker: `docker compose up -d` (top-level `compose.yaml` and `watchboard/Dockerfile` are present). To rebuild: `docker compose down && docker rmi watchboard && docker compose up -d` (documented in README).
- EF migrations (when entities change):
  - Example: `DATA_DIR="." dotnet ef migrations add Foo --project watchboard/watchboard.csproj --configuration Debug --output-dir Database/Migrations`
  - `AppDbContext.ApplyMigrations()` is called on startup to apply migrations and seed initial data.
- Tests: `dotnet test` (project `watchboard.tests`). Some tests hit TMDB and expect a token; review `watchboard.tests/TmDb.Test.cs` before running.

Project-specific conventions & gotchas
- Config file location: prefer setting `DATA_DIR` — Program.cs uses that path to load `appsettings.json` and to place the SQLite file. README mentions a different default; use the source (`Program.cs`) as truth.
- Routes return `RazorComponentResult<T>` for server-side rendering of razor components — when adding endpoints, follow the existing pattern in `Routes/*` files.
- Repository methods often return EF models (detached via `.AsNoTracking()`), and many methods mutate entities and call `SaveChangesAsync()`; keep concurrency and tracking semantics in mind when changing repository code.
- Hosted worker runs only in non-development environments. If you need to debug worker logic locally, either run in a non-dev environment or instantiate the worker in dev while debugging.

Integration touchpoints
- TMDB: `watchboard/Services/TmDb` — token must be provided in `$DATA_DIR/appsettings.json` under `TmdbToken`.
- Caching: `MemoryCache` is used for TMDB results; check `Program.cs` registration and `TmDb` implementation for cache keys/TTL.
- Static files: if you update front-end libs, use `lib/copy-dist-libs.sh` to copy updated distributables into `watchboard/wwwroot/lib/`.

- Quick examples to copy when editing or adding features
- Add API endpoint rendering a component:
  - Follow `Routes/Items.cs` — create an extension `MapX(this RouteGroupBuilder app)` and return `RazorComponentResult<_YourComponent>(new { Model = model })`.
- Update TMDB-driven item data:
  - See `Services/Repository.UpdateItemFromTmDb` and `Services/Mapping.UpdateFromTmDb` for the canonical steps (fetch details, images, seasons, map providers/credits, store base64 poster/backdrop).

Razor components & props (concrete examples)
- `Pages/Home.razor` — page entry that the server returns via `RazorComponentResult<Home>`.
  - Parameters the server sends: `Lists` (List<List>) and `Boards` (List<Board>). The view derives `SelectedBoard` from `Lists[0].BoardId`.
  - Usage: iterates `Lists` and renders `<_List ListModel="l"></_List>` for each list.

- `Pages/Partials/_List.razor` — list column partial.
  - Parameter: `List ListModel` (contains `Items` collection).
  - Usage: renders each list item with `<_Item ItemModel="i"></_Item>` and posts list sort updates to `/app/lists/{listId}/items`.

- `Pages/Partials/_Item.razor` — compact item card used inside lists.
  - Parameter: `Item ItemModel` (model from `Database/Entities/Item`).
  - Notes: item card uses `ItemModel.PosterBase64` for the image, and opens details via `hx-get` to `/app/items/{itemId}` which returns `_ItemDetail` markup.

- `Pages/Partials/_ItemDetail.razor` — modal with item details and provider selection.
  - Parameters: `Item ItemModel`, `List<Board> Boards`.
  - Notes: form posts to `/app/items/{itemId}` for provider selection and exposes actions: refresh (`PUT /app/items/{itemId}/refresh`), move (`PUT /app/items/{itemId}/move/{boardId}`), delete (`DELETE /app/items/{itemId}`).

- `Pages/Partials/_SearchResults.razor` — search result list returned by `Routes/Search`.
  - Parameters: `List<Item> Items`, `List<List> Lists`.
  - Notes: each result posts to `/app/items/{tmdbId}?type={tv|movie}` to add an item; the server `Repository.SearchForItems` returns Item-shaped DTOs (Id = Guid.Empty) used by the UI.

These concrete examples are the most common patterns for adding new endpoints that render UI: the route should assemble the minimal model (usually EF entities or lightweight DTOs) and return `RazorComponentResult<YourComponent>(new { /* params */ })`.

If anything in this file is unclear or missing (examples, commands, or reference files), tell me which area you'd like expanded (routes, DB, TMDB, docker, tests) and I'll iterate. 
