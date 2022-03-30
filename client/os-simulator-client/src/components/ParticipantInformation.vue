<template>
    <div class="particpant-information">
        <div>
            <div class="avatar">
                <img src="" />
            </div>
            <p class="org">
                <span class="org-dot"></span
                ><span class="organization">Organisation</span>
            </p>
            <p class="participant">( {{ session.participant }} )</p>
            
            <time-left class="time-left"></time-left>

            <p class="status" v-if="statusText">Status: {{statusText}}</p>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { mapState } from 'vuex';
import { Session } from '@/Types/Session';
import TimeLeft from '@/components/TimeLeft.vue';
import Format from '@/helpers/Format.ts';

@Component({
    computed: mapState(['session', 'progress', 'duration', 'status']),
    components: {TimeLeft},
})
export default class ParticipantInformation extends Vue {
    public progress!: number;
    public session!: Session;
    public duration!: number;
    public status!: string;

    get statusText(): string {
        switch (this.status) {
            case 'Paused':
                return 'Pausad';
            case 'Closed':
                return 'Stängd';
            case 'Cancelled':
                return 'Avbruten';
            case 'Finished':
                return 'Avslutad';
            default:
                return '';
        }
    }

    private timeLeft(): string {
        const duration = this.duration;

        const minutesLeft = Math.ceil(duration - duration * this.progress);

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

.status {
    margin-top: 25px;
    margin-left: 35px;
}

.org {
    font-weight: bold;
    display: table;
    .org-dot {
        display: table-cell;
        vertical-align: middle;
        margin-right: 10px;
        @include dot($blue);
    }

    .organization {
        display: table-cell;
        vertical-align: middle;
    }
}

.participant{
    margin-bottom: 25px;
    margin-left: 35px;
}

div.avatar {
    color: $teal;
    width: 176px;
    height: 176px;
    border-radius: 50%;
    padding: 0;
    margin: 0 0 35px 0;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 0 0 1px #979797;
    background-color: #fdfdfd;
    img {
        height: 116px;
    }
}
</style>
