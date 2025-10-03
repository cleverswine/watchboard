<template>
  <v-app :theme="theme">
    <v-app-bar density="comfortable">
      <template v-slot:prepend>
        <v-app-bar-nav-icon variant="text" @click.stop="drawer = !drawer" />
      </template>

      <v-toolbar-title>My Stuff</v-toolbar-title>

      <div class="justify-center w-25">
        <v-text-field density="compact" placeholder="Search" prepend-inner-icon="mdi-magnify" variant="solo" flat hide-details single-line></v-text-field>
      </div>
      <v-spacer></v-spacer>
      <v-btn :prepend-icon="theme === 'light' ? 'mdi-weather-sunny' : 'mdi-weather-night'" @click="onClick" />
    </v-app-bar>

    <v-navigation-drawer v-model="drawer">
      <v-container>
        <v-list-item title="Saved Filters"></v-list-item>
        <div class="mb-2">
          <v-select density="compact" hide-details label="Type" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
        </div>

        <v-divider class="my-4"></v-divider>

        <v-list-item title="Filter" subtitle="set a name to save the filter"></v-list-item>
        <div class="mb-2">
          <v-text-field hide-details clearable density="compact" placeholder="filter name (optional)"></v-text-field>
        </div>
        <div class="mb-2">
          <v-select multiple clearable density="comfortable" hide-details label="Type" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
        </div>
        <div class="mb-2">
          <v-select multiple clearable density="comfortable" hide-details label="Country" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
        </div>
        <div class="mb-2">
          <v-select multiple clearable density="comfortable" hide-details label="Language" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
        </div>
        <div class="mb-2">
          <v-select multiple clearable density="comfortable" hide-details label="Source" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
        </div>
        <div class="mb-2">
          <v-select multiple clearable density="comfortable" hide-details label="Status" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
        </div>
        <div class="d-flex ga-2">
          <v-btn class="flex-grow-0" variant="tonal">Clear All</v-btn>
          <v-btn class="flex-grow-1" variant="tonal">Apply</v-btn>
        </div>
      </v-container>
    </v-navigation-drawer>

    <v-main>
      <v-container>
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import { ref } from 'vue'
import { shallowRef } from 'vue'

const items = shallowRef(['foo', 'bar', 'fizz', 'buzz', 'dfdf', 'sdfasdff', 'asdfsdffff'])
const value = shallowRef(['foo', 'bar', 'fizz', 'buzz'])

const theme = ref('dark')
const drawer = ref(true)

function onClick() {
  theme.value = theme.value === 'light' ? 'dark' : 'light'
}
</script>
