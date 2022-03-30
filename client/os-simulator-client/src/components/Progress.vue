<template>
    <div class="progress" v-if="user &&  user.currentScenario">
        <div class="phases">
            <div
                class="phase"
                v-for="(phase, index) in user.currentScenario.phases"
                v-bind:key="index"
                :style="`width:${getPhaseWidth(phase)}%`"
            >
                <span>{{ phase.heading }}</span>
                <div class="phase-bar"></div>
            </div>
        </div>
        <div class="events" v-if="user.currentScenario.events">
            <div
                class="event"
                v-for="(sEvent, index) in user.currentScenario.events"
                :style="'left:' + sEvent.progressPoint * 100 + '%'"
                v-bind:key="index"
                @mouseover="showEventInformation('ev' + index)"
                @mouseleave="hideEventInformation('ev' + index)"
            >
                <div class="event-information" :ref="'ev' + index">
                    {{ sEvent.heading }}
                </div>
            </div>
        </div>
        <img
            class="marker"
            :style="'left:' + getProgress() + '%'"
            src="@/assets/icons/marker.svg"
        />
    </div>
</template>
<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import { mapState } from 'vuex';
import Phase from '../Types/Phase';
import { User } from '@/Types/User';

@Component({
    computed: mapState(['progress', 'user']),
    components: {},
})
export default class Progress extends Vue {
    //#region (mapsate)

    public progress!: number;
    private user!: User;
    //#endregion

    /*get scenarioEvents(): ScenarioEvent[] {
        if ( this.facilitator.currentScenario && this.facilitator.currentScenario.events
        ) {
            return this.facilitator.currentScenario.events;
        }
        return [];
    }

    get phases(): Phase[] {
        if (
            this.facilitator.currentScenario &&
            this.facilitator.currentScenario.phases
        ) {
            return this.facilitator.currentScenario.phases;
        }
        return [];
    }*/

    private getLeft(start: number, end: number): number {
        return (end - start) * 100;
    }
    private getPhaseWidth(phase: Phase): number {
        return (phase.end - phase.start) * 100;
    }
    private getProgress(): number {
        return this.progress * 100;
    }

    private showEventInformation(ref: string) {
        const eventInformationElement: HTMLElement = this.$refs[
            ref
        ] as HTMLElement;
        eventInformationElement[0].setAttribute(
            'class',
            'event-information active'
        );
    }
    private hideEventInformation(ref: string) {
        const eventInformationElement: HTMLElement = this.$refs[
            ref
        ] as HTMLElement;
        eventInformationElement[0].setAttribute('class', 'event-information');
    }
}
</script>

<style lang="scss">
@import '../assets/scss/colors';
@import '../assets/scss/typography';

.progress {
    position: relative;
    .phases {
        display: flex;
        .phase {
            position: relative;
            text-align: left;
            padding: 0;
            height: 50px;
            &:last-child {
                border-right: none;
            }
            span {
                color: $place-holder-color;
                margin-left: -8px;
            }
        }
        .phase-bar {
            position: absolute;
            bottom: 0;
            height: 16px;
            right: 21px;
            left: -8px;
            background-color: $third-background-color;
            border-radius: 8px;
        }
    }
    .event {
        position: absolute;
        width: 16px;
        height: 16px;
        border-radius: 50%;
        background-color: $dark-blue1;
        z-index: 11;
        bottom: 0px;
        cursor: pointer;
        margin-left: -8px;
    }
    .event-information {
        pointer-events: none;
        opacity: 0;
        width: 210px;
        margin-left: -125px;
        background-color: #fff;
        border-radius: 6px;
        padding: 10px 20px;
        position: absolute;
        top: 34px;
        left: 8px;
        box-shadow: 0 2px 10px 0 rgba(0, 0, 0, 0.1);
        @include typography(regular, 12, $default-text-color);
        transition: opacity 0.5s;

        &.active {
            opacity: 1;
        }
    }
    .marker {
        position: absolute;
        transition: left 3s linear;
        margin-left: -5px;
        left: 0;
        bottom: -18px;
        z-index: 10;
        &.md-icon {
            color: #448aff !important;
        }
    }
}
</style>
