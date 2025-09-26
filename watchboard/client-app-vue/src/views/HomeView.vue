<script setup lang="ts">
import { shallowRef } from 'vue'

const items = shallowRef(['foo', 'bar', 'fizz', 'buzz'])
const value = shallowRef(['foo', 'bar', 'fizz', 'buzz'])
</script>

<template>
  <v-container>
    <v-toolbar elevation="8" density="compact">
      <v-toolbar-items>
        <v-btn color="medium-emphasis" density="comfortable" icon="mdi-crosshairs-gps"></v-btn>

        <v-btn class="ms-1" color="medium-emphasis" density="comfortable" icon="mdi-dots-vertical"></v-btn>

        <v-menu>
          <template v-slot:activator="{ props }">
            <v-btn icon="mdi-filter-menu" variant="text" v-bind="props"></v-btn>
          </template>

          <v-list>
            <v-list-item v-for="i in 6" :key="i" :value="i">
              <v-list-item-title>{{ i }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>

        <v-text-field
          class="me-2"
          density="compact"
          placeholder="Search"
          prepend-inner-icon="mdi-magnify"
        ></v-text-field>
      </v-toolbar-items>
    </v-toolbar>

    <v-row>
      <v-col v-for="n in 14" :key="n" lg="2" md="4" sm="6" xl="2  " xs="12">
        <v-card link to="/ItemDetail">
          <v-img class="align-end text-white" cover src="@/assets/logo.png">
            <v-card-title>Top 10 Australian beaches Top 10 Australian beaches</v-card-title>
          </v-img>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped></style>
