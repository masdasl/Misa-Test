<script setup lang="ts">
import { useRoute } from "vue-router";
import CustomerCommon from "../../common/CustomerCommon.vue";
const route = useRoute();
const customerId = route.params.customerId as string | undefined;
</script>
<template>
  <CustomerCommon :customer-id="customerId" />
</template>
