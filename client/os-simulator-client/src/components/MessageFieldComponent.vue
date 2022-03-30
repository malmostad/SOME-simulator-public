<template>
    <div class="message-field-component">
        <div class="message-form" :class="{'compact':compact}">
            <AvatarComponent v-if="compact"></AvatarComponent>
            <div class="text-area-container">
                <textarea
                    placeholder="Skriv en kommentar"
                    :disabled="status != 'Running'"
                    class="comment-input"
                    :class="{'compact':compact, 'full-size': !empty}"
                    :ref="'textField'"
                    :value="textInput"
                    @input="updateValue"
                    @keydown.enter.exact.prevent
                    @keyup.enter.exact="submit()"
                    @keydown.enter.shift.exact="newline"
                >
                </textarea>
                <p class="small" v-if="flow == MessageFlow.Short">{{textInput.length}}/280</p>
            </div>
            <div class="right" :class="{'compact':compact}">
                <button
                    :disabled="status != 'Running'"
                    v-on:click="submit()"
                    class="tertiary"
                >
                    <span v-if="flow == MessageFlow.Long">
                        Skicka    
                    </span>
                    <span v-else>
                        Kwittra
                    </span>
                    
                </button>
            </div>
        </div>
        
    </div>
</template>

<script lang="ts">
    import {Component, Emit, Prop, Vue} from 'vue-property-decorator'
    import {MessageFlow} from "@/Types/MessageFlow";
    import {mapState} from "vuex";
    import {SessionLog} from "@/Types/SessionLog";
    import AvatarComponent from "@/components/AvatarComponent.vue";
    
    interface IFocus {
        focus();
    }

    @Component({
        components: {AvatarComponent},
        computed:{ ...mapState(['status']), MessageFlow : () => MessageFlow}
    })
    export default class extends Vue {
        @Prop({required: true})
        private flow?: MessageFlow;
        
        @Prop({required: false, default: false})
        private compact?: boolean;

        @Prop({required: false, default: true})
        private focus?: boolean;

        private textInput: string = '';

        @Prop({ default: Object as () => SessionLog })
        public message!: SessionLog;
        
        @Emit('submit')
        public submit(): string {
            let tmpTextInput = this.textInput;
            this.textInput = '';
            return tmpTextInput;
        } 
        
        get empty () {
            return !this.textInput && this.textInput.length <=0;
        }

        //#region (mapsate)
        public status!: string;
        //#endregion
        
        public mounted() { 
            if(this.focus)
                (this.$refs.textField as HTMLElement).focus();
        }

        public newline() {
            this.textInput = `${this.textInput}\n`;
        }
        
        public updateValue(event) {
            const value = event.target.value;

            if (this.flow === MessageFlow.Long) {
                this.textInput = value;
            }
            if (String(value).length <= 280) {
                this.textInput = value;
            }
            this.$forceUpdate();
        }
        
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/colors';
    @import '../assets/scss/typography';
    .message-form {

        .comment-input {
            width: 100%;
            margin-bottom: 15px;
            
            &.compact {
                    overflow: hidden;
                    height: 44px;
                    transition: all 250ms;
                    &:focus, &.full-size{
                        height: 130px;
                    }
            }
        }
        .right {
            text-align: right;
        }

        .text-area-container {
            display: flex;
            flex-grow: 1;
            flex-direction: column;
            align-items: flex-end;
            
            p {
                z-index: -1;
                color: $dark-gray2;
                position: relative;
                top: -15px;
                margin-bottom: 10px;
                padding: 0;
            }
        }

        &.compact {
            display: flex;

            
            .comment-input {
                margin-bottom: 20px;
            }

            * + * {
                margin-left: 30px;
            }
        }
    }
</style>