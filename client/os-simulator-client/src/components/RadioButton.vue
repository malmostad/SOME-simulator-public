<template>
    <div class="radiobutton" @click="toggle()" :class="{'checked': checked}">
        <div><slot></slot></div>
        <div class="box">
            
            <svg v-if="checked" width="11px" height="11px" viewBox="0 0 13 14" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink">
                <title>Group 4</title>
                <g id="Facilitator" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                    <g id="Facilitator-PRINT-VIEW" transform="translate(-288.000000, -319.000000)">
                        
                        <g id="Group-8" transform="translate(143.000000, 265.000000)">
                            <g id="Group-10" transform="translate(0.000000, 40.000000)">
                                <g id="Group" transform="translate(0.000000, 0.000000)">
                                    <rect id="Rectangle" x="0" y="0" width="178" height="44" rx="6"></rect>
                                </g>
                            </g>
                            
                            <g id="Group-4" transform="translate(146.000000, 55.878419)" stroke="#7A7A7A" stroke-linecap="round" stroke-width="2">
                                <line x1="0.357142857" y1="6.37107396" x2="5" y2="10.9544073" id="Line-2"></line>
                                <line x1="5.33333333" y1="0.321073961" x2="10.6666667" y2="10.5877406" id="Line-2" transform="translate(8.000000, 5.454407) scale(-1, 1) translate(-8.000000, -5.454407) "></line>
                            </g>
                        </g>
                    </g>
                </g>
            </svg>
            
        </div>
    </div>
</template>

<script lang="ts">
    import {Component, Prop, Vue} from 'vue-property-decorator'

    @Component({})
    export default class RadioButton extends Vue {
        
        private val: boolean = false;
        
        public created() {
            this.val = this.checked;
        }
        
        @Prop({type: Boolean})checked!: boolean;
        
        
        public toggle() {
            this.val = !this.val;
            this.$emit('change', this.val);
        }
    }
</script>

<style lang="scss" scoped>

    @import '../assets/scss/colors';
    @import '../assets/scss/typography';
    @import '../assets/scss/elements';
    @import '../assets/scss/spacings';
    
.radiobutton {
    cursor: default;
    background: $gray4;
    border-radius: 6px;
    line-height: 100%;
    padding: 8px 12px 8px 8px;
    display: inline-flex;
    align-items: center;
    justify-content: flex-start;
    flex-direction: row;
    
    

    div {
        flex-grow: 0;
        flex-shrink: 0;
    }
    
    .box {
        margin-left: 25px;
        display: flex;
        height: 30px;
        width: 30px;
        background: $gray3;
        border-radius: 6px;
        align-items: center;
        justify-content: center;
    }
    
    &.checked{
        
        .box {
            background: $icon-hightlight  url('../assets/icons/check.svg') no-repeat;    
            background-position-x: center;
            background-position-y: center;
        }
    }
    
}
</style>