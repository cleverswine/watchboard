<script setup lang="ts">
import NavigationDrawer from '@/components/NavigationDrawer.vue'
import { computed, ref, shallowRef } from 'vue'
import allItems from '@/assets/ItemAllDetails.json'
import { MediaItem, SpokenLanguage, WatchProvider } from '@/views/Item.ts'

/*
Singleton: items, current filter
 */

const drawer = ref(true)

const items = computed(() => {
  return allItems.map(
    (item) =>
      new MediaItem({
        name: item.name,
        posterPath: item.poster_path,
        spokenLanguages: item.spoken_languages.map((i) => new SpokenLanguage(i)),
        watchProviders: item['watch/providers']?.results?.US?.flatrate.map((i) => new WatchProvider(i)),
      }),
  )
})

// const groupedByLanguageItems: Record<string, any[]> = items.reduce((acc: Record<string, any[]>, currentItem) => {
//   const keys = currentItem.spoken_languages.map((i) => i.english_name)
//   for (let key of keys) {
//     if (!acc[key]) {
//       acc[key] = []
//     }
//     acc[key].push(currentItem)
//   }
//   return acc
// }, {})
// console.log(groupedByLanguageItems)
//
// const groupedByProviderItems: Record<string, any[]> = items.reduce((acc: Record<string, any[]>, currentItem) => {
//   const keys = (currentItem['watch/providers']?.results?.US?.flatrate ?? []).map((i) => i.provider_name)
//   for (let key of keys) {
//     if (!acc[key]) {
//       acc[key] = []
//     }
//     acc[key].push(currentItem)
//   }
//   return acc
// }, {})
// console.log(groupedByProviderItems)

const languageChoices = [...new Set(allItems.map((i) => i.spoken_languages.map((l) => l.english_name)).flat())]
const providerChoices = [
  ...new Set(
    allItems
      .map((i) => i['watch/providers']?.results?.US?.flatrate)
      .flat()
      .sort((a, b) => (a && b ? a.display_priority - b.display_priority : 0))
      .map((l) => (l ? l.provider_name : '?')),
  ),
]

// shallowRef means only setting the top level value triggers change, not nested properties
const languagesSelected = shallowRef([1, 4])
const providersSelected = shallowRef([1, 4])

const filterDisplayString = computed(() => {
  const result = [languagesSelected.value.map((i) => languageChoices[i]).join(', '), providersSelected.value.map((i) => providerChoices[i]).join(', ')]
  return result.join(', ')
})

// const items = shallowRef([1, 4])

// function ismatch(item: any) {
//   const l = languagesSelected.value.map((i) => languageChoices[i])
//   const p = providersSelected.value.map((i) => providerChoices[i])
//
//   const itemLanguages = [...item.spoken_languages.map((i: any) => i.english_name)]
//   const itemProviders = [...(item['watch/providers']?.results?.US?.flatrate.map((i: any) => i.provider_name) ?? '?')]
//
//   if (itemLanguages.some((lang: string) => l.includes(lang))) return true
//   if (itemProviders.some((prov: string) => p.includes(prov))) return true
//   return false
// }
</script>

<template>
  <NavigationDrawer v-model="drawer"></NavigationDrawer>

  <v-container>
    <v-expansion-panels>
      <v-expansion-panel>
        <v-expansion-panel-title>{{ filterDisplayString }} <v-btn variant="plain" density="compact" class="ms-2" icon="mdi-close-circle-outline"></v-btn> </v-expansion-panel-title>
        <v-expansion-panel-text>
          <v-row>
            <v-col cols="4">
              <div class="text-caption">Languages</div>
              <v-chip-group v-model="languagesSelected" column multiple>
                <v-chip v-for="s in languageChoices" density="compact" :text="s" variant="outlined" filter></v-chip>
              </v-chip-group>
            </v-col>
            <v-col cols="8">
              <div class="text-caption">Services</div>
              <v-chip-group v-model="providersSelected" column multiple>
                <v-chip v-for="s in providerChoices" density="compact" :text="s" variant="outlined" filter></v-chip>
              </v-chip-group>
            </v-col>
          </v-row>
          <v-row>
            <v-col cols="12" class="text-end">
              <v-btn variant="plain">Save as...</v-btn>
            </v-col>
          </v-row>
        </v-expansion-panel-text>
      </v-expansion-panel>
    </v-expansion-panels>

    <v-divider class="mt-2 mb-4"></v-divider>

    <!--    <v-row v-for="[k, items] in groupedByLanguageItems">-->
    <!--      <v-col cols="12">-->
    <!--        <p>-->
    <!--          {{ k }}-->
    <!--        </p>-->
    <!--      </v-col>-->
    <!--      <v-col v-for="item in items">-->
    <!--        <div class="text-caption">{{ item }}</div>-->
    <!--      </v-col>-->
    <!--    </v-row>-->
    <v-row>
      <v-col v-for="n in items" lg="2" md="4" sm="6" xl="2" xs="12">
        <v-card link to="/ItemDetail">
          <v-img class="align-end text-white" cover src="@/assets/logo.png">
            <v-card-text>{{ n.name }}</v-card-text>
          </v-img>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped></style>
