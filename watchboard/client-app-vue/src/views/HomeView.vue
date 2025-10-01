<script setup lang="ts">
import { shallowRef } from 'vue'

const items = shallowRef(['foo', 'bar', 'fizz', 'buzz'])
const value = shallowRef(['foo', 'bar', 'fizz', 'buzz'])
const selected = shallowRef('foo')
</script>

<template>
  <v-container>
    <v-toolbar density="compact">
      <v-btn>
        <v-icon>mdi-format-align-left</v-icon>
      </v-btn>
      <v-btn>
        <v-icon>mdi-format-align-center</v-icon>
      </v-btn>
    </v-toolbar>
    <v-row>
      <v-col cols="2">
        <v-btn-toggle density="compact">
          <v-btn>
            <v-icon>mdi-format-align-left</v-icon>
          </v-btn>
          <v-btn>
            <v-icon>mdi-format-align-center</v-icon>
          </v-btn>
        </v-btn-toggle>
      </v-col>
    </v-row>
    <v-row align="center">
      <v-col cols="2" xs="12" sm="6" md="4" lg="2" xl="2">
        <v-select multiple clearable density="compact" hide-details label="Type" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
      </v-col>
      <v-col cols="2" xs="12" sm="6" md="4" lg="2" xl="2">
        <v-select multiple clearable density="compact" hide-details label="Country" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
      </v-col>
      <v-col cols="2" xs="12" sm="6" md="4" lg="2" xl="2">
        <v-select multiple clearable density="compact" hide-details label="Language" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
      </v-col>
      <v-col cols="2" xs="12" sm="6" md="4" lg="2" xl="2">
        <v-select multiple clearable density="compact" hide-details label="Source" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
      </v-col>
      <v-col cols="2" xs="12" sm="6" md="4" lg="2" xl="2">
        <v-select multiple clearable density="compact" hide-details label="Status" :items="['California', 'Colorado', 'Florida', 'Georgia', 'Texas', 'Wyoming']" variant="solo-filled"></v-select>
      </v-col>
      <v-col cols="2" xs="12" sm="6" md="4" lg="2" xl="2">
        <v-btn variant="tonal">Clear All</v-btn>
      </v-col>
    </v-row>
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
