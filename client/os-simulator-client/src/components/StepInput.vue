<template>
    <div class="step-input">
        <button @click="increase" class="increase round" aria-label="Minska">
            <span class="increase"></span>
        </button>
        <button @click="decrease" class="decrease round" aria-label="Öka">
            <span class="decrease"></span>
        </button>
    </div>
</template>

<script lang="ts">
import { Watch, Component, Prop, Vue } from 'vue-property-decorator';

@Component({})
export default class StepInput extends Vue {
    @Prop({ required: true, default: 5 })
    private value!: string;

    @Prop({ required: true, default: 1 })
    private step!: number;

    @Prop({ required: true, default: 1 })
    private min!: number;

    @Prop({ required: true, default: 60 })
    private max!: number;

    private internalValue: number = 0;

    @Watch('value')
    private change(val, oldVal) {
        this.internalValue = parseInt(val, 10);
    }

    private increase() {
        if (this.internalValue + this.step > this.max) {
            this.internalValue = this.max;
        } else {
            this.internalValue += this.step;
        }
        this.emitChange();
    }

    private decrease() {
        if (this.internalValue - this.step < this.min) {
            this.internalValue = this.min;
        } else {
            this.internalValue -= this.step;
        }
        this.emitChange();
    }

    private emitChange() {
        this.$emit('input', this.internalValue);
    }

    private created() {
        this.internalValue = parseInt(this.value, 10) || 0;
    }
}
</script>

<style lang="scss" scoped>
button {
    padding: 0px;
    margin: 0px 6px 0 0;
    height: 50px;
    width: 50px;
    overflow: hidden;
}

@mixin icon($url) {
    width: 50px !important;
    height: 50px !important;
    background-position: center;
    background-size: auto;
    display: inline-block;
    background-image: url($url);
    background-repeat: no-repeat;
}

span.increase {
    @include icon('../assets/icons/plus.svg');
}

span.decrease {
    @include icon('../assets/icons/minus.svg');
}
</style>
