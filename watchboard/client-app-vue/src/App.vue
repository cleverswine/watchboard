<template>
  <v-app :theme="theme">
    <v-app-bar density="comfortable">
      <template v-slot:prepend>
        <v-app-bar-nav-icon variant="text" @click.stop="drawer = !drawer" />
      </template>

      <v-toolbar-title> My Stuff</v-toolbar-title>

      <div class="justify-center w-25">
        <v-text-field density="compact" placeholder="Search" prepend-inner-icon="mdi-magnify" variant="solo" flat hide-details single-line></v-text-field>
      </div>
      <v-spacer></v-spacer>
      <!--      <IconTooling></IconTooling>-->
      <!--      <img width="24" height="24" src="@/assets/logo.png" alt="logo" />-->
      <v-btn :prepend-icon="theme === 'light' ? 'mdi-weather-sunny' : 'mdi-weather-night'" @click="onClick" />
    </v-app-bar>

    <v-main>
      <v-container fluid class="pa-xs-2 pa-sm-4 pa-md-6 pa-lg-8 pa-xl-10">
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import { ref } from 'vue'
import { shallowRef } from 'vue'
import NavigationDrawer from '@/components/NavigationDrawer.vue'
import IconTooling from '@/components/icons/IconTooling.vue'

const items = shallowRef(['foo', 'bar', 'fizz', 'buzz', 'dfdf', 'sdfasdff', 'asdfsdffff'])
const value = shallowRef(['foo', 'bar', 'fizz', 'buzz'])

const theme = ref('dark')
const drawer = ref(true)

function onClick() {
  theme.value = theme.value === 'light' ? 'dark' : 'light'
}
</script>
