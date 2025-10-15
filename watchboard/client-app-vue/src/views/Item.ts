export class MediaItem {
  name: string | null
  posterPath: string | null
  spokenLanguages: SpokenLanguage[] = []
  watchProviders: WatchProvider[] = []

  constructor(init?: Partial<MediaItem>) {
    this.name = init?.name ?? null
    this.posterPath = init?.posterPath ?? null
    this.spokenLanguages = init?.spokenLanguages ?? []
    this.watchProviders = init?.watchProviders ?? []
  }
}

export class SpokenLanguage {
  english_name: string | null
  iso_639_1: string | null
  name: string | null

  constructor(init?: Partial<SpokenLanguage>) {
    this.english_name = init?.english_name ?? null
    this.iso_639_1 = init?.iso_639_1 ?? null
    this.name = init?.name ?? null
  }
}

export class WatchProvider {
  logo_path: string | null
  provider_id: number | null
  provider_name: string | null
  display_priority: number | null

  constructor(init?: Partial<WatchProvider>) {
    this.logo_path = init?.logo_path ?? null
    this.provider_id = typeof init?.provider_id === 'number' ? init.provider_id : null
    this.provider_name = init?.provider_name ?? null
    this.display_priority = typeof init?.display_priority === 'number' ? init.display_priority : null
  }
}
