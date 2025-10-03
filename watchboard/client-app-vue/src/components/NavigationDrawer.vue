<script setup lang="ts">
import { computed } from 'vue'

const model = defineModel<boolean>()

const drawer = computed(() => model.value)
</script>

<template>
  <v-navigation-drawer v-model="drawer" :location="$vuetify.display.mobile ? 'bottom' : 'right'">
    <v-container>
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
</template>

<style scoped></style>
