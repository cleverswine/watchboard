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
      <v-menu>
        <template v-slot:activator="{ props }">
          <v-btn icon="mdi-account-circle" v-bind="props"></v-btn>
        </template>
        <v-list>
          <v-list-item v-for="i in 6" :key="i" :value="i">
            <v-list-item-title>Stuff for {{ i }}</v-list-item-title>
          </v-list-item>
        </v-list>
      </v-menu>

      <v-btn :prepend-icon="theme === 'light' ? 'mdi-weather-sunny' : 'mdi-weather-night'" @click="onClick" />
    </v-app-bar>

    <v-navigation-drawer v-model="drawer">
      <v-container>
        <div>
          Filter
        </div>

        <v-select density="default" v-model="value" :items="items" label="country" multiple></v-select>
        <v-select density="comfortable" v-model="value" :items="items" label="language" multiple></v-select>
        <v-select density="compact" v-model="value" :items="items" label="service" multiple></v-select>

        <div class="v-row">
          <div class="v-col-6">
            <v-btn block variant="elevated" color="blue-darken-4">Apply</v-btn>
          </div>
          <div class="v-col-6">
            <v-btn block variant="tonal">Clear</v-btn>
          </div>
        </div>
      </v-container>

      <v-list>
        <v-list-item title="My Application" subtitle="Vuetify"></v-list-item>
        <v-divider></v-divider>
        <v-list-item>
          <v-select density="default" v-model="value" :items="items" label="country" multiple></v-select>
        </v-list-item>
        <v-list-item>
          <v-select density="comfortable" v-model="value" :items="items" label="language" multiple></v-select>
        </v-list-item>
        <v-list-item>
          <v-select density="compact" v-model="value" :items="items" label="service" multiple></v-select>
        </v-list-item>
      </v-list>
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
