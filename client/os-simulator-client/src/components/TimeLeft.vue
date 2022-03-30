<template>
    <div class="time-left">Tid kvar: {{ timeLeft }}</div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { mapState } from 'vuex';
import Format from '@/helpers/Format';

@Component({
    computed: mapState(['progress', 'duration']),
})
export default class TimeLeft extends Vue {
    public progress!: number;
    public duration!: string | null;

    get timeLeft(): string {
        if (this.duration == null ||
            typeof this.duration !== 'string' ||
            !this.duration.indexOf(':')
        ) {
            return '00:00';
        }
        const durationArr = this.duration.split(':');
        const duration = parseInt(durationArr[0], 10) * 3600
            + parseInt(durationArr[1], 10) * 60
            + parseInt(durationArr[2], 10);

        const minutesLeft = Math.ceil( (duration - duration * this.progress) / 60 );

        return Format.minutes(minutesLeft);
    }
}
</script>

<style lang="scss" scoped>
@import '../assets/scss/colors';
@import '../assets/scss/typography';
@import '../assets/scss/elements';

.time-left {
    padding-left: 35px;
    background: url('../assets/icons/clock.svg') no-repeat;
    background-position-x: left;
    background-size: contain;
}

</style>
