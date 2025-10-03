<script setup lang="ts">
import { shallowRef } from 'vue'

const items = shallowRef(['foo', 'bar', 'fizz', 'buzz'])
const value = shallowRef(['foo', 'bar', 'fizz', 'buzz'])
const chip = shallowRef(true)
const dialog = shallowRef(false)
</script>

<template>
  <v-dialog v-model="dialog" width="auto">
    <v-card max-width="400" prepend-icon="mdi-update" title="Update in progress">
      <v-card-text>
        <v-row align="center">
          <v-col cols="6">
            <v-select multiple clearable density="compact" hide-details label="Type" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
          </v-col>
          <v-col cols="6">
            <v-select multiple clearable density="compact" hide-details label="Country" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
          </v-col>
          <v-col cols="6">
            <v-select multiple clearable density="compact" hide-details label="Language" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
          </v-col>
          <v-col cols="6">
            <v-select multiple clearable density="compact" hide-details label="Source" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
          </v-col>
          <v-col cols="6">
            <v-select multiple clearable density="compact" hide-details label="Status" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
          </v-col>
          <v-col cols="6">
            <v-btn variant="tonal">Clear All</v-btn>
          </v-col>
        </v-row>
      </v-card-text>
      <template v-slot:actions>
        <v-btn class="ms-auto" text="Ok" @click="dialog = false"></v-btn>
      </template>
    </v-card>
  </v-dialog>

  <v-container>
    <v-row class="align-center">
      <v-col cols="4">
        <v-select clearable variant="plain" density="compact" hide-details label="Saved Filter" :items="['US TV Shows', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']"></v-select>
      </v-col>
      <v-col cols="8" class="text-end">
        <v-chip variant="tonal" class="me-2">TV</v-chip>
        <v-chip variant="tonal" class="me-2">US</v-chip>
        <v-chip variant="tonal" class="me-2">Amazon</v-chip>
      </v-col>
    </v-row>
    <v-divider class="mt-2 mb-4"></v-divider>
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
