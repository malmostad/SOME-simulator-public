<template>
  <div class="home">
    hem
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import ParticipantInformation from '../components/ParticipantInformation.vue';

@Component({
  components: {
    ParticipantInformation,
  },
})
export default class Home extends Vue {}
</script>
