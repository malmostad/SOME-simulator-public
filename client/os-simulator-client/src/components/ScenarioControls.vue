<template>
    <div
        class="scenario-controls-component"
        id="scenario-controls-component"
        :class="{ fixed: isFixed }"
    >
        <!--Close confirmation-->
        <dialog-component v-bind:open="showStopConfirmation">
            <div class="dialog-content card">
                <div class="text">Är du säker på att du vill avsluta övningen?</div>
                <div class="buttons">
                    <button @click="stop()">Ja</button>
                    <button class="secondary" @click="showStopConfirmation = false">Nej</button>    
                </div>
                    
            </div>
        </dialog-component>
        
        <div class="scenario-controls" id="scenario-controls">
            <div class="buttons">
                <button
                    v-if="status == 'New' && user.sessions.length > 0"
                    @click="manageSession('start')"
                    class="primary start"
                >
                    Starta
                    <svg
                        width="12px"
                        height="14px"
                        viewBox="0 0 12 14"
                        version="1.1"
                        xmlns="http://www.w3.org/2000/svg"
                        xmlns:xlink="http://www.w3.org/1999/xlink"
                    >
                        <g
                            id="Symbols"
                            stroke="none"
                            stroke-width="1"
                            fill="none"
                            fill-rule="evenodd"
                        >
                            <g
                                id="Starta"
                                transform="translate(-114.000000, -15.000000)"
                                fill="#F0F1F4"
                            >
                                <g id="Group-10">
                                    <path
                                        d="M115.756796,15.1718533 L125.458395,21.0843386 C126.181682,21.525389 126.180205,22.5569167 125.455933,22.996039 L115.835083,28.8294726 C115.072406,29.292214 114.086687,28.7590973 114.080287,27.8808528 L114,16.1344518 C113.99363,15.2484949 114.988211,14.7033277 115.756796,15.1718533"
                                        id="Fill-1"
                                    ></path>
                                </g>
                            </g>
                        </g>
                    </svg>
                </button>
                <button
                    v-if="status == 'Running'"
                    @click="manageSession('pause')"
                    class="primary pause"
                >
                    Pausa
                    <svg
                        width="11px"
                        height="14px"
                        viewBox="0 0 11 14"
                        version="1.1"
                        xmlns="http://www.w3.org/2000/svg"
                        xmlns:xlink="http://www.w3.org/1999/xlink"
                    >
                        <g
                            id="Symbols"
                            stroke="none"
                            stroke-width="1"
                            fill="none"
                            fill-rule="evenodd"
                        >
                            <g
                                id="Pausa"
                                transform="translate(-109.000000, -15.000000)"
                                fill="#7A7A7A"
                            >
                                <g id="Group-10">
                                    <g
                                        id="Group-5"
                                        transform="translate(109.000000, 15.000000)"
                                    >
                                        <path
                                            d="M0.720063864,14 C0.322565194,14 0,13.728409 0,13.393726 L0,0.606273985 C0,0.271098123 0.322565194,0 0.720063864,0 L2.57993614,0 C2.97802022,0 3.3,0.271098123 3.3,0.606273985 L3.3,13.393726 C3.3,13.728409 2.97802022,14 2.57993614,14 L0.720063864,14 Z"
                                            id="Fill-1"
                                        ></path>
                                        <path
                                            d="M10.2800639,14 L8.41993615,14 C8.02250798,14 7.7,13.728409 7.7,13.393726 L7.7,0.606273985 C7.7,0.271098123 8.02250798,0 8.41993615,0 L10.2800639,0 C10.677492,0 11,0.271098123 11,0.606273985 L11,13.393726 C11,13.728409 10.677492,14 10.2800639,14"
                                            id="Fill-3"
                                        ></path>
                                    </g>
                                </g>
                            </g>
                        </g>
                    </svg>
                </button>
                <button
                    v-if="status == 'Paused'"
                    @click="manageSession('unpause')"
                    class="primary resume"
                >
                    Återuppta
                </button>
                <button
                    v-if="status == 'Finished'"
                    @click="manageSession('leave')"
                    class="primary stop"
                >
                    Avsluta
                    <svg
                        width="12px"
                        height="12px"
                        viewBox="0 0 12 12"
                        version="1.1"
                        xmlns="http://www.w3.org/2000/svg"
                        xmlns:xlink="http://www.w3.org/1999/xlink"
                    >
                        <g
                            id="Facilitator"
                            stroke="none"
                            stroke-width="1"
                            fill="none"
                            fill-rule="evenodd"
                        >
                            <g
                                id="Facilitator-View-Main"
                                transform="translate(-438.000000, -303.000000)"
                                fill="#7A7A7A"
                            >
                                <g
                                    id="Group-7"
                                    transform="translate(324.000000, 286.000000)"
                                >
                                    <g id="Group">
                                        <rect
                                            id="Rectangle"
                                            x="114"
                                            y="17"
                                            width="12"
                                            height="12"
                                            rx="1"
                                        ></rect>
                                    </g>
                                </g>
                            </g>
                        </g>
                    </svg>
                </button>

                <button @click="print" v-if="status== 'Finished'" class="secondary">
                    Utskrift
                </button>

                <button
                    v-if="status == 'Running' || status == 'Paused'"
                    @click="showStopConfirmation = true"
                    class="secondary stop"
                >
                    Stoppa
                </button>
                <button
                    v-if="status == 'New'"
                    @click="manageSession('cancel')"
                    class="secondary"
                >
                    Gå tillbaka
                    <svg
                        width="12px"
                        height="12px"
                        viewBox="0 0 12 12"
                        version="1.1"
                        xmlns="http://www.w3.org/2000/svg"
                        xmlns:xlink="http://www.w3.org/1999/xlink"
                    >
                        <g
                            id="Facilitator"
                            stroke="none"
                            stroke-width="1"
                            fill="none"
                            fill-rule="evenodd"
                        >
                            <g
                                id="Facilitator-View-Main"
                                transform="translate(-438.000000, -303.000000)"
                                fill="#7A7A7A"
                            >
                                <g
                                    id="Group-7"
                                    transform="translate(324.000000, 286.000000)"
                                >
                                    <g id="Group">
                                        <rect
                                            id="Rectangle"
                                            x="114"
                                            y="17"
                                            width="12"
                                            height="12"
                                            rx="1"
                                        ></rect>
                                    </g>
                                </g>
                            </g>
                        </g>
                    </svg>
                </button>
            </div>
            <div class="currentEvent" v-if="isFixed">
                <strong>Händelse</strong>
                | <span>{{ currentEvent }}</span>
            </div>
            <div class="company" v-if="isFixed">
                <strong>Organisation</strong>
            </div>

            <Progress class="progress-bar" v-if="!isFixed" />
            
            <TimeLeft v-if="!isFixed"></TimeLeft>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import ScenarioEvent from '../Types/ScenarioEvent';
import { mapState } from 'vuex';
import Phase from '../Types/Phase';
import Progress from '@/components/Progress.vue';
import TimeLeft from '@/components/TimeLeft.vue';
import { User } from '@/Types/User';
import { SessionLog } from '@/Types/SessionLog';
import DialogComponent from '@/components/DialogComponent.vue';

@Component({
    computed: {
        ...mapState(['status', 'user']),
    },
    components: {
        Progress, DialogComponent, TimeLeft,
    },
})
export default class ScenarioControls extends Vue {
    @Prop({ default: Object as () => ScenarioEvent[] })
    private events!: ScenarioEvent[];
    @Prop({ default: Object as () => Phase[] })
    private phases!: Phase[];

    private status!: string;
    private user!: User;
    private isFixed: boolean = false;
    private showStopConfirmation: boolean = false;

    public print() {
        this.$router.push('print');
    }

    get currentEvent() {
        const scenarioEvents: SessionLog[] = this.$store.state.user.eventLog.filter(
            (e) => e.messageType === 'ScenarioEvent'
        );

        if (!scenarioEvents || scenarioEvents.length <= 0) {
            return '';
        }

        return scenarioEvents[0].heading;
    }

    private manageSession(message: string) {
        this.$emit('manage-session', message);
    }

    public stop() {
        this.manageSession('stop');
        this.showStopConfirmation = false;
    }

    public handleScroll(event: any) {
        if (document.getElementById('scenario-controls') != null) {
            // tslint:disable-next-line:max-line-length
            const scenarioControls: HTMLElement =
                document.getElementById('scenario-controls-component') ||
                new HTMLElement();
            const participantRow: HTMLElement =
                document.getElementById('participant-row') || new HTMLElement();
            const body: HTMLElement =
                document.getElementById('body') || new HTMLElement();
            const fromTop = participantRow.getBoundingClientRect().top;
            if (fromTop <= 0) {
                this.isFixed = true;
                body.classList.add('fixedTop');
            } else {
                this.isFixed = false;
                body.classList.remove('fixedTop');
            }
        }
        // console.log(event);
    }
    public created() {
        window.addEventListener('scroll', this.handleScroll);
    }
    public destroyed() {
        window.removeEventListener('scroll', this.handleScroll);
    }
}
</script>

<style lang="scss">
@import '../assets/scss/colors';
@import '../assets/scss/typography';
@import '../assets/scss/elements';
@import '../assets/scss/spacings';
body.fixedTop {
    .participant-row {
        padding-top: 140px;
    }
}
.scenario-controls-component {
    width: 100%;
    &.fixed {
        position: fixed;
        top: 0;
        background-color: #fff;
        left: 0;
        box-shadow: inset 0 -1px 0 0 #e6e6e6;
        z-index: 100;

        .scenario-controls {
            max-width: 1094px;
            margin: 0 auto;
            padding: $space-sm 30px;
        }
    }
}
.scenario-controls {
    display: flex;
    flex-direction: row;
    align-items: center;
    width: 100%;
    padding: 35px 0 55px 0;
    &.fixed {
        position: fixed;
        top: 0;
        background-color: #fff;
        left: 0;
    }
}
.buttons {
    width: 400px;
    flex-grow: 0;
    flex-shrink: 0;
    button {
        width: 170px;
        margin-right: 13px;
        svg {
            margin-left: 10px;
        }
        &.primary {
            svg {
                rect,
                path {
                    fill: #fff;
                }
            }
        }
    }
}
.currentEvent {
    flex: 1;
    padding-right: 30px;
}
.progress-bar {
    flex: 1;
}

.dialog-content.card {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: row;
    flex-wrap: wrap;
    align-content: center;
    
    div.text {
        margin-bottom: 23px;
    }
    
    div.buttons{
        flex-basis: 100%;
        text-align: center;
    }
    
    button {
        
        width: 170px;
    }
    
    button {
        margin-right: 11px !important;
    }

    button:nth-child(2) {
        margin-right: 0 !important;
    }
}
</style>
