<template>
    <div class="dialog" v-if="open" :class="{'high-priority': highPriority}">
        <a href class="close-btn" @click.prevent="closeModal()" v-if="enableClose">
            Stäng
            <svg
                width="15px"
                height="15px"
                viewBox="0 0 15 15"
                version="1.1"
                xmlns="http://www.w3.org/2000/svg"
                xmlns:xlink="http://www.w3.org/1999/xlink"
            >
                <g
                    id="Användare"
                    stroke="none"
                    stroke-width="1"
                    fill="none"
                    fill-rule="evenodd"
                    stroke-linecap="round"
                >
                    <g
                        id="Händelse-modal"
                        transform="translate(-1332.000000, -67.000000)"
                        stroke="#FFFFFF"
                        stroke-width="2"
                    >
                        <g
                            id="Group-5"
                            transform="translate(1253.000000, 57.000000)"
                        >
                            <g
                                id="Group-4"
                                transform="translate(80.000000, 11.000000)"
                            >
                                <polyline
                                    id="Line-2"
                                    points="0.5 0.5 7.09228516 7.09228516 12.5 12.5"
                                />
                                <line
                                    x1="0.5"
                                    y1="0.5"
                                    x2="12.5"
                                    y2="12.5"
                                    id="Line-2"
                                    transform="translate(6.500000, 6.500000) scale(-1, 1) translate(-6.500000, -6.500000) "
                                />
                            </g>
                        </g>
                    </g>
                </g>
            </svg>
        </a>
        <div v-bind:class="{'cloak':true, 'dark-cloak': dark}" @click.prevent="closeModal()"></div>
        <div class="dialog-content">
            <slot></slot>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({})
export default class DialogComponent extends Vue {
    @Prop({ required: false, default: false })
    private highPriority?: boolean;
    
    @Prop({ required: true, default: true })
    public open!: boolean;

    @Prop({required: false, default: false})
    public dark!: boolean;

    @Prop({required: false, default: true})
    public enableClose!: boolean;

    public closeModal(): void {
        if (this.enableClose) {
            this.$emit('modalClosed');
        }
    }
    
}
</script>

<style scoped lang="scss">
@import '../assets/scss/spacings.scss';
@import '../assets/scss/typography.scss';
.dialog {
    width: 100%;
    height: 100%;
    z-index: 100;
    position: fixed;
    top: 0;
    left: 0;
    display: flex;
    align-items: center;
    justify-content: center;


    &.high-priority {
        z-index: 200;
    }
    
    .dialog-heading {
        @include typography('extrabold', 38, $dialog-heading-color);
        margin-bottom: $space-md;
        text-align: center
    }

    .close-btn {
        position: fixed;
        top: $space-md;
        right: $space-md;
        z-index: 120;
        color: #fff;
        border: 1px solid #fff;
        border-radius: 3px;
        text-decoration: none;
        padding: 10px 20px;
    }
    .cloak {
        position: fixed;
        background-color: rgba(0, 0, 0, 0.8);
        width: 100vw;
        height: 100vh;
        left: 0;
        top: 0;
    }
    .dark-cloak {
        background-color: rgba(45, 45, 45, 0.95)
    }
    
    .dialog-content {
        position: relative;
        z-index: 110;
        width:70vw;

        
        .card {
            height: 50vh;
            h1 {
                margin-top: 0;
            }
        }
    }
    .message-comment{
        a{
            display: none;
        }
    }


    .transparent-dialog {
        color: $white1;
        padding: $space-md;
        color: $white1;
        .sender {
            @include typography("light", 20, $white1);
            margin-bottom: $space-sm;
        }
        .heading {
            @include typography("extrabold", 26, $white1);
            margin-bottom: 21px;
        }
    }
}
</style>
