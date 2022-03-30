<template>
    <div class="slider">
        <div class="phase-bar">
            <div
                class="event"
                v-for="scenarioEvent in phase.scenarioEvents"
                :style="'left:' + scenarioEvent.timePercent * 100 + '%'"
            >
                <Tooltip :content="scenarioEvent.heading" class="fill"></Tooltip>
            </div>
        </div>

        <vue-slider
            v-model="timeLeft"
            :tooltip="'none'"
            :process="false"
            @drag-end="change"
            :clickable="true"
            :drag-on-click="true"
            :lazy="true"
            :max="100"
            :min="0"
            
        >
            <template v-slot:dot>
                <img
                    src="@/assets/icons/marker.svg"
                    class="slider-dot"
                    alt=""
                />
            </template>
        </vue-slider>
    </div>
</template>

<script lang="ts">
import { Component, Emit, Prop, Vue } from 'vue-property-decorator';
import EditPhase from '@/Types/Admin/EditPhase';
import VueSlider from 'vue-slider-component';
import Tooltip from "@/components/Tooltip.vue";

@Component({ components: { VueSlider, Tooltip } })
export default class extends Vue {
    
    @Prop({ required: true })
    private phase?: EditPhase;

    @Prop({ required: true})
    private initialValue?: number;
    
    @Emit()
    private change(something:any): number {
        return (this.timeLeft || 0) / 100;
    }
    public timeLeft?: number = 0;
    
    public created() {
        this.timeLeft = (this.initialValue || 0) * 100; 
    }
}
    
</script>

<style lang="scss" scoped>
@import '../assets/scss/spacings';
@import '../assets/scss/colors';

.vue-slider-rail {
    opacity: 0;
}

.fill {
    width: 100%;
    height: 100%;
}

.phase-bar {
    height: 16px;
    background-color: $third-background-color;
    border-radius: 8px;
}

div {
    position: relative;
    .event {
        position: absolute;
        width: 16px;
        height: 16px;
        border-radius: 50%;
        background-color: $dark-blue1;
        z-index: 11;
        bottom: 0;
        cursor: pointer;
        margin-left: -8px;
    }
}
</style>
