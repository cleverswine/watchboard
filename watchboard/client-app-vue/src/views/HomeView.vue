<script setup lang="ts">
import NavigationDrawer from '@/components/NavigationDrawer.vue'
import { computed, ref, shallowRef } from 'vue'
import items from '@/assets/ItemAllDetails.json'

/*
Singleton: items, current filter
 */
const drawer = ref(true)

const amenitiesChoices1 = ['US', 'FR', 'DE', 'ES', 'PO']
const amenitiesChoices2 = ['Amazon Prime', 'Netflix', 'Television', 'Amazon Prime PBS Subscription', 'Hulu']

// shallowRef means nly setting the top level value triggers change, not nested properties
const amenities1 = shallowRef([1, 4])
const amenities2 = shallowRef([1, 4])

const selected1 = computed(() => {
  const result = [amenities1.value.map((i) => amenitiesChoices1[i]).join(', '), amenities2.value.map((i) => amenitiesChoices2[i]).join(', ')]
  return result.join(', ')
})
</script>

<template>
  <NavigationDrawer v-model="drawer"></NavigationDrawer>

  <v-container>
    <v-expansion-panels>
      <v-expansion-panel>
        <v-expansion-panel-title>{{ selected1 }} <v-btn variant="plain" density="compact" class="ms-2" icon="mdi-close-circle-outline"></v-btn> </v-expansion-panel-title>
        <v-expansion-panel-text>
          <v-row>
            <v-col cols="4">
              <div class="text-caption">Languages</div>
              <v-chip-group v-model="amenities1" column multiple>
                <v-chip v-for="s in amenitiesChoices1" density="compact" :text="s" variant="outlined" filter></v-chip>
              </v-chip-group>
            </v-col>
            <v-col cols="4">
              <div class="text-caption">Services</div>
              <v-chip-group v-model="amenities2" column multiple>
                <v-chip v-for="s in amenitiesChoices2" density="compact" :text="s" variant="outlined" filter></v-chip>
              </v-chip-group>
            </v-col>
            <v-col cols="4">
              <div class="text-caption">Languages</div>
              <v-chip-group v-model="amenities1" column multiple>
                <v-chip density="compact" text="Elevator" variant="outlined" filter></v-chip>
                <v-chip density="compact" text="Washer / Dryer" variant="outlined" filter></v-chip>
                <v-chip density="compact" text="Fireplace" variant="outlined" filter></v-chip>
                <v-chip density="compact" text="Wheelchair access" variant="outlined" filter></v-chip>
                <v-chip density="compact" text="Dogs ok" variant="outlined" filter></v-chip>
                <v-chip density="compact" text="Cats ok" variant="outlined" filter></v-chip>
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
