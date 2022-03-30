<template>
    <div class="tooltip-container" @mouseenter="hover = true" @mouseleave="hover = false">
        <div class="tooltip" >
            <slot></slot>
        </div>
        <div>
            <div class="tooltiptext" v-if="hover" >{{content}}</div>
        </div>
    </div>    
</template>

<script lang="ts">
import { Prop, Component } from 'vue-property-decorator';
import Vue from 'vue';
import { SessionLog } from '../Types/SessionLog';

@Component({})
export default class Tooltip extends Vue {
    @Prop({required: true})
    public content!: string;

    public hover: boolean = false;
}
</script>

<style scoped lang="scss">
    @import '../assets/scss/typography';
    @import '../assets/scss/colors';

    .tooltip-container {
        position: relative;  
        display: inline-block; 
    }
    .tooltip  {
        position: relative;
        display: inline-block;
        height: 100%;
    }

    .tooltiptext {
        box-shadow: 0 2px 10px 0 rgba(0,0,0,0.10);
        width: 120px;
        position: absolute;
        top: 30px;
        z-index: 5000;
        left: 50%;
        transform: translateX(-50%);
        background-color: $gray2;
        padding: 5px;
        border-radius: 6px;
        text-align: center;
        @include typography("regular", 12, $dark-gray1);
    }

    .tooltiptext::before{
        content: "";
        z-index: 5000;
        position: absolute;
        top: -6px;
        left: 59px;
        width: 0px;
        height: 0px;
        border-left: 6px solid transparent;
        border-right: 6px solid transparent;
        border-bottom: 6px solid $gray2;
    }

    
</style>