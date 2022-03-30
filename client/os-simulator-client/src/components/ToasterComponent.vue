<template>
    <div class="toasters">
        <transition-group name="toast" tag="p">
            <div v-for="(toast, index) in filter(toasts)" :key="index">
                <div
                    class="toast"
                    :class="{ expanded: toast.expanded }"
                    @click="toasterClick(toast)"
                >
                    <div class="heading bold">
                        <span
                            class="ball"
                            :class="{
                                blue: messageFlow === 2,
                                pink: messageFlow === 1,
                            }"
                        ></span
                        >{{ toast.descriptiveName }}
                    </div>

                    <div class="body">
                        <div class="avatar bold">
                            {{ toast.sender }}
                        </div>
                        {{ toast.text }}
                    </div>

                    <div class="footer">
                        <a
                            v-on:click="goToComment(toast)"
                        >
                            <span>Gå till kommentar</span>
                            <span>
                                <svg
                                    width="16px"
                                    height="15px"
                                    viewBox="0 0 16 15"
                                    version="1.1"
                                    xmlns="http://www.w3.org/2000/svg"
                                    xmlns:xlink="http://www.w3.org/1999/xlink"
                                >
                                    <!-- Generator: Sketch 64 (93537) - https://sketch.com -->
                                    <title>link</title>
                                    <desc>Created with Sketch.</desc>
                                    <defs>
                                        <rect
                                            id="path-1"
                                            x="1170"
                                            y="233"
                                            width="270"
                                            height="222"
                                            rx="6"
                                        ></rect>
                                        <filter
                                            x="-2.6%"
                                            y="-2.3%"
                                            width="105.2%"
                                            height="106.3%"
                                            filterUnits="objectBoundingBox"
                                            id="filter-2"
                                        >
                                            <feOffset
                                                dx="0"
                                                dy="2"
                                                in="SourceAlpha"
                                                result="shadowOffsetOuter1"
                                            ></feOffset>
                                            <feGaussianBlur
                                                stdDeviation="2"
                                                in="shadowOffsetOuter1"
                                                result="shadowBlurOuter1"
                                            ></feGaussianBlur>
                                            <feColorMatrix
                                                values="0 0 0 0 0   0 0 0 0 0   0 0 0 0 0  0 0 0 0.1 0"
                                                type="matrix"
                                                in="shadowBlurOuter1"
                                            ></feColorMatrix>
                                        </filter>
                                    </defs>
                                    <g
                                        id="Användare"
                                        stroke="none"
                                        stroke-width="1"
                                        fill="none"
                                        fill-rule="evenodd"
                                    >
                                        <g
                                            id="Desktop-HD"
                                            transform="translate(-1389.000000, -419.000000)"
                                        >
                                            <rect
                                                id="Rectangle"
                                                fill="#E6E8EB"
                                                x="0"
                                                y="0"
                                                width="1440"
                                                height="4281"
                                            ></rect>
                                            <g id="Rectangle-Copy-2">
                                                <use
                                                    fill="black"
                                                    fill-opacity="1"
                                                    filter="url(#filter-2)"
                                                    xlink:href="#path-1"
                                                ></use>
                                                <use
                                                    fill="#FDFDFD"
                                                    fill-rule="evenodd"
                                                    xlink:href="#path-1"
                                                ></use>
                                            </g>
                                            <g
                                                id="link"
                                                transform="translate(1389.000000, 419.000000)"
                                                fill="#7A7A7A"
                                                fill-rule="nonzero"
                                            >
                                                <path
                                                    d="M12.123843,0 C11.0886972,0 10.115322,0.362228272 9.38321698,1.01962223 L6.36484287,3.72997221 C6.59552461,3.70105376 6.82598641,3.66965256 7.06178953,3.66965256 C7.71195259,3.66965256 8.34904516,3.78306931 8.93473548,3.99336992 L10.8148451,2.30512491 C11.1648265,1.99128215 11.6295205,1.81816643 12.123843,1.81816643 C13.1446615,1.81816643 13.9751409,2.56389567 13.9751409,3.48054 C13.9751409,3.92444526 13.7823511,4.34168912 13.4328409,4.65595508 L10.0486499,7.69484687 C9.34865556,8.3225042 8.13061702,8.32295561 7.43062267,7.69529828 L6,8.98170379 C6.73163375,9.63822314 7.70497747,10 8.73962058,10 C9.7747664,10 10.7481101,9.63777173 11.4802466,8.98037777 L14.8645004,5.94148599 C15.5966054,5.28409203 16,4.41004951 16,3.48054 C16,1.5611731 14.2613723,0 12.123843,0 Z"
                                                    id="Path"
                                                ></path>
                                                <path
                                                    d="M7.04745896,11.0092086 L5.18510722,12.6871431 C4.83512686,13.0020424 4.37043437,13.175741 3.87611342,13.175741 C2.85529809,13.175741 2.0248213,12.4275012 2.0248213,11.5077709 C2.0248213,11.0623996 2.21761056,10.6437227 2.56711963,10.3283988 L5.95136282,7.27927644 C6.65135496,6.64950608 7.86938968,6.64905315 8.56938182,7.27882351 L10,5.98808727 C8.53626581,4.67062818 5.98400754,4.67020356 4.51977064,5.98941775 L1.13549603,9.03854008 C0.403361873,9.69814718 0,10.5751039 0,11.5077426 C0,13.4335995 1.73859084,15 3.876082,15 C4.91122457,15 5.88456523,14.6365523 6.6166994,13.9769452 L9.59500432,11.2935635 C9.37755086,11.3198901 9.1612285,11.351397 8.93808813,11.351397 C8.27790433,11.3514253 7.63848873,11.2325029 7.04745896,11.0092086 Z"
                                                    id="Path"
                                                ></path>
                                            </g>
                                        </g>
                                    </g>
                                </svg>
                            </span>
                        </a>

                        <a v-on:click="removeToast(toast)">
                            <span>Markera som läst</span>
                            <span>
                                <svg
                                    width="15px"
                                    height="15px"
                                    viewBox="0 0 15 15"
                                    version="1.1"
                                    xmlns="http://www.w3.org/2000/svg"
                                    xmlns:xlink="http://www.w3.org/1999/xlink"
                                >
                                    <title>Group 4</title>
                                    <defs>
                                        <rect
                                            id="path-1"
                                            x="0"
                                            y="0"
                                            width="288"
                                            height="274"
                                            rx="6"
                                        ></rect>
                                        <filter
                                            x="-2.4%"
                                            y="-1.8%"
                                            width="104.9%"
                                            height="105.1%"
                                            filterUnits="objectBoundingBox"
                                            id="filter-2"
                                        >
                                            <feOffset
                                                dx="0"
                                                dy="2"
                                                in="SourceAlpha"
                                                result="shadowOffsetOuter1"
                                            ></feOffset>
                                            <feGaussianBlur
                                                stdDeviation="2"
                                                in="shadowOffsetOuter1"
                                                result="shadowBlurOuter1"
                                            ></feGaussianBlur>
                                            <feColorMatrix
                                                values="0 0 0 0 0   0 0 0 0 0   0 0 0 0 0  0 0 0 0.1 0"
                                                type="matrix"
                                                in="shadowBlurOuter1"
                                            ></feColorMatrix>
                                        </filter>
                                        <filter id="filter-3">
                                            <feColorMatrix
                                                in="SourceGraphic"
                                                type="matrix"
                                                values="0 0 0 0 0.478431 0 0 0 0 0.478431 0 0 0 0 0.478431 0 0 0 1.000000 0"
                                            ></feColorMatrix>
                                        </filter>
                                    </defs>
                                    <g
                                        id="Användare"
                                        stroke="none"
                                        stroke-width="1"
                                        fill="none"
                                        fill-rule="evenodd"
                                    >
                                        <g
                                            id="Desktop-HD"
                                            transform="translate(-1389.000000, -469.000000)"
                                        >
                                            <g
                                                filter="url(#filter-3)"
                                                id="Group-4"
                                                stroke-linecap="round"
                                            >
                                                <g
                                                    transform="translate(1390.000000, 470.000000)"
                                                >
                                                    <line
                                                        x1="0.5"
                                                        y1="0.5"
                                                        x2="12.5"
                                                        y2="12.5"
                                                        id="Line-2"
                                                        stroke="#FDFDFD"
                                                        stroke-width="2"
                                                    ></line>
                                                    <line
                                                        x1="0.5"
                                                        y1="0.5"
                                                        x2="12.5"
                                                        y2="12.5"
                                                        id="Line-2"
                                                        stroke="#FDFDFD"
                                                        stroke-width="2"
                                                        transform="translate(6.500000, 6.500000) scale(-1, 1) translate(-6.500000, -6.500000) "
                                                    ></line>
                                                </g>
                                            </g>
                                        </g>
                                    </g>
                                </svg>
                            </span>
                        </a>
                    </div>
                </div>
            </div>
        </transition-group>
    </div>
</template>

<script lang="ts">
    import {Component, Prop, Vue} from 'vue-property-decorator';
    import {mapState} from 'vuex';
    import {MessageFlow} from '@/Types/MessageFlow';
    import {SessionLog} from '@/Types/SessionLog';
    import {Toast} from '@/components/Toast';
    import router from "@/router";

    @Component({
        computed: mapState(['session']),
    })
    export default class ToasterComponent extends Vue {
        public toasts: Toast[] = new Array<Toast>();

        @Prop({default: MessageFlow.Short})
        public messageFlow!: MessageFlow;

        public filter(toasts: Toast[]): Toast[] {
            if (this.toasts !== undefined) {
                return this.toasts.filter(
                    (s) => s.messageFlow === this.messageFlow || s.messageFlow === 0
                );
            }
            return [];
        }

        public toasterClick(toast: Toast) {
            this.toasts.forEach((t) => {
                if (t !== toast) {
                    t.expanded = false;
                }
            });

            toast.expanded = !toast.expanded;
        }

        private unsubscribe!: () => void;

        public destroyed() {
            this.unsubscribe();
        }

        public created() {
            this.toasts = new Array<Toast>();

            this.unsubscribe = this.$store.subscribeAction({
                before: (action, state) => {
                    // no action
                },
                after: (action) => {
                    
                    let sessionLog:any = null;
                    
                    
                    if (action.type === 'addComment') {
                        sessionLog = action.payload.sessionLog;
                        
                        if(sessionLog.children.length > 0){
                            const last =
                                sessionLog.children[sessionLog.children.length - 1];
    
                            if (last.messageType === 'Comment') {
                                this.addToast(last);
                            }
                        }
                    }
                    else if (action.type === 'addMessage') {
                        sessionLog = action.payload.sessionLog;
                        this.addToast(sessionLog);
                    }
                },
            });
        }
        
        public goToComment(toast:any)
        {
            window.location.href="#sessionLog" + toast.id;
            setTimeout(() => this.removeToast(toast), 2000);
        }
        
        public removeToast(toast: Toast) {
            const index = this.toasts.indexOf(toast);
            if (index >= 0) {
                this.toasts.splice(index, 1);
                this.toasts = [...this.toasts];
            }
        }

        private addToast(sessionLog: SessionLog) {
            const toast: Toast = new Toast(sessionLog);
            this.toasts = [toast, ...this.toasts];
            setTimeout(() => {
                toast.expanded = false;
            }, 4000);
        }
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/spacings.scss';
    @import '../assets/scss/typography.scss';
    @import '../assets/scss/colors.scss';

    .toast-enter-active,
    .toast-leave-active {
        transition: all 1s;
    }

    .toast-enter, .toast-leave-to /* .list-leave-active below version 2.1.8 */
    {
        opacity: 0;
    }

    .toasters {
        pointer-events: none;
        width: 270px;
        position: fixed;
        top: 75px;
        right: 0;
        overflow-x: hidden;
        position: absolute;
        z-index:1;

        .toast {
            pointer-events: all;
            box-shadow: 0 2px 4px 0 rgba(0, 0, 0, 0.1);
            width: 240px;
            position: relative;
            right: -212px;
            max-height: 21px;
            overflow: hidden;

            transition: right 0.2s, max-height 0.3s 0.5s;

            &.expanded {
                max-height: 500px;
                right: 0;
            }

            margin: 10px;
            border-radius: 6px 0 0 6px;
            padding: $space-sm;

            background-color: $white1;

            .heading {
                display: flex;
                justify-items: center;

                span.ball {
                    height: 20px;
                    width: 20px;

                    display: inline-block;
                    border-radius: 50%;
                    margin-right: 12px;

                    &.blue {
                        background-color: $dark-blue1;
                    }

                    &.pink {
                        background-color: $pink;
                    }
                }
            }

            .body {
                overflow: hidden;
                padding-top: 22px;
                padding-bottom: 25px;
                max-height: 8em;

                .avatar {
                    margin-bottom: 6px;
                }
            }

            .footer {
                overflow: hidden;

                a {
                    border-top: thin solid $gray3;
                    cursor: pointer;
                    display: flex;
                    text-decoration: none;
                    padding: 15px 0;

                    &:last-child {
                        padding-bottom: 5px;
                    }

                    span:first-child {
                        flex-grow: 0;
                    }

                    span:last-child {
                        flex-grow: 1;
                        text-align: right;
                    }
                }
            }

            a {
                span.truncate {
                    display: inline-block;
                    width: 145px;
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                }
            }
        }
    }
</style>
