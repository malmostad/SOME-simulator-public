<template>
    <div class="stress-slider">
        <div class="slider-container">
            <p class="title">Stressnivå</p>
            <div class="meter">
                <div
                    class="meter-active"
                    :style="`width:${(stressLevel / 100) * 384}px;`"
                ></div>
            </div>
            <vue-slider
                v-model="stressLevel"
                :tooltip="'none'"
                :process="false"
                @drag-end="changeStressLevel"
                :clickable="true"
                :drag-on-click="true"
                :lazy="true"
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
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import { mapState } from 'vuex';
import VueSlider from 'vue-slider-component';

@Component({
    components: {
        VueSlider,
    },
})
export default class StressSlider extends Vue {
    @Prop(Number)
    public value!: number;

    @Watch('value')
    private onValueChange(value: number) {
        this.stressLevel = value;
    }

    public stressLevel: number = 50;

    public changeStressLevel() {
        this.$emit('changeStressLevel', this.stressLevel);
    }
}
</script>
<style lang="scss">
.stress-slider {
    .vue-slider-rail {
        background-color: transparent;
    }
}
</style>
<style scoped lang="scss">
@import '../assets/scss/typography';
@import '../assets/scss/colors';
@import '../assets/scss/spacings';

p {
    @include typography(regular, 18, $dark-gray1);
    letter-spacing: 1.14px;

    &.title {
        padding-bottom: $space-sm;
        font-weight: 600;
        color: $dark-gray1;
    }

    margin: 0;
    padding: 0;
}

.stress-slider {
    padding: 40px 0;
    display: flex;
    justify-content: center;
}

.slider-container {
    width: 384px;
}

.meter {
    position: relative;
    width: 100%;
    height: 90px;
    background: transparent url('../assets/meter.svg') no-repeat top left;
}

.meter-active {
    position: absolute;
    top: 0;
    left: 0;
    height: 90px;
    background: transparent url('../assets/meter-active.svg') no-repeat top left;
}
</style>
