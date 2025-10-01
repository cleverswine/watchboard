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
    <v-toolbar density="comfortable">
      <template v-slot:prepend>
        <v-menu>
          <template v-slot:activator="{ props }">
            <v-btn icon="mdi-filter" v-bind="props"></v-btn>
          </template>
          <v-list>
            <v-list-subheader>Saved Filters</v-list-subheader>
            <v-list-item v-for="(item, i) in items" :key="i" :value="item" color="primary">
              <template v-slot:append>
                <v-btn variant="flat" icon="mdi-delete" class="ms-2"></v-btn>
              </template>
              <v-list-item-title v-text="item"></v-list-item-title>
            </v-list-item>
            <v-divider></v-divider>
            <v-list-item @click="dialog = true">
              <v-list-item-title>New Filter...</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <span class="text-center text-grey-lighten-2">
          <v-chip v-if="chip" class="ma-2" size="small" closable @click:close="chip = false">US TV Shows: TV, EN </v-chip>
          <v-btn density="compact" v-if="!chip" color="primary" @click="chip = true"> Reset Chip </v-btn>
        </span>
      </template>
      <template v-slot:append>
        <v-btn icon="mdi-magnify"></v-btn>
        <v-btn icon="mdi-heart"></v-btn>
        <v-btn icon="mdi-dots-vertical"></v-btn>
      </template>
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
