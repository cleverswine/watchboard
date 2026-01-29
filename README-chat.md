# TV Series Kanban (Nuxt 3)

A Kanban-style web application for tracking TV series you are watching, planned to watch, completed, or dropped.

**Tech stack**

* **Nuxt 3 / Vue 3** – application framework
* **better-sqlite3** – fast, synchronous SQLite access (server-side)
* **tmdb-ts** – typed TMDB API client
* **Tailwind CSS + DaisyUI** – styling and UI components

---

## Prerequisites

* **Node.js 18+** (required by Nuxt 3)
* npm (or pnpm/yarn if you prefer)

Verify:

```bash
node --version
```

---

## 1. Scaffold the Nuxt 3 project

Create the project:

```bash
npx nuxi@latest init tv-kanban
cd tv-kanban
npm install
```

Start the dev server to confirm everything works:

```bash
npm run dev
```

---

## 2. Install Tailwind CSS and DaisyUI

Install Tailwind module for Nuxt and DaisyUI:

```bash
npm install -D @nuxtjs/tailwindcss
npm install daisyui
```

Enable Tailwind in `nuxt.config.ts`:

```ts
export default defineNuxtConfig({
  modules: ['@nuxtjs/tailwindcss']
})
```

Create or update `tailwind.config.ts`:

```ts
import type { Config } from 'tailwindcss'

export default <Config>{
  content: [
    './components/**/*.{vue,js,ts}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
    './app.vue'
  ],
  plugins: [require('daisyui')],
  daisyui: {
    themes: ['light', 'dark']
  }
}
```

---

## 3. Install server-side dependencies

Install SQLite and TMDB client libraries:

```bash
npm install better-sqlite3 tmdb-ts
```

> ⚠️ **Important**
>
> * `better-sqlite3` must only be imported in **server-side code** (`/server/*`).
> * Never import it into Vue components or client composables.

---

## 4. Create server structure

Set up directories for database access, APIs, and utilities:

```bash
mkdir -p server/api server/db server/utils
```

Recommended usage:

* `server/db/` – SQLite connection and schema
* `server/api/` – CRUD API routes for series and Kanban columns
* `server/utils/` – TMDB client helpers

---

## 5. Enable strict TypeScript (recommended)

Nuxt generates `tsconfig.json` automatically. To tighten type safety, enable strict mode:

```json
{
  "compilerOptions": {
    "strict": true
  }
}
```

This pairs well with `tmdb-ts` for strong API typing.

---

## 6. Verify installed dependencies

Check your top-level dependencies:

```bash
npm ls --depth=0
```

You should see entries for:

* `nuxt`
* `vue`
* `better-sqlite3`
* `tmdb-ts`
* `tailwindcss`
* `daisyui`

---

## 7. Suggested architecture (next steps)

Once scaffolding is complete, a clean Nuxt-centric approach is:

* **Database**

  * `server/db/sqlite.ts` – open SQLite connection
* **TMDB integration**

  * `server/utils/tmdb.ts` – TMDB client wrapper
* **Kanban states**

  * `planned | watching | completed | dropped`
* **APIs**

  * `server/api/series/*` – list, add, move, delete series
* **UI**

  * DaisyUI cards + columns
  * Optional drag-and-drop via `@vueuse/core` or `sortablejs`

---

## Development

Run the development server:

```bash
npm run dev
```

Build for production:

```bash
npm run build
```

Preview production build:

```bash
npm run preview
```

---

## Notes

* SQLite is ideal for this app due to its simplicity and zero-config setup.
* Nuxt server routes give you a built-in API without running a separate backend.
* DaisyUI accelerates layout and keeps Tailwind class noise manageable.

---

This README covers **project scaffolding only**. Database schema, API routes, and UI composition are intended to be layered on top incrementally.
