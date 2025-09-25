<script setup lang="ts"></script>

<template>
  <v-container>
    <v-row>
      <v-col v-for="n in 14" :key="n" lg="2" md="4" sm="6" xl="1" xs="12">
        <v-card link to="/ItemDetail">
          <v-img class="align-end text-white" cover src="@/assets/logo.png">
            <v-card-title>Top 10 Australian beaches</v-card-title>
          </v-img>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped></style>
