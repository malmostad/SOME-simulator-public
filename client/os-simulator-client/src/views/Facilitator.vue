<template>
    <div class="facilitator-view view" v-cloak>
        <div v-if="!this.$store.getters.hasSessionGroupId || $store.state.status === 'Cancelled'">
            <div>
                <h1 class="big">Välj scenario</h1>
                <div
                    v-for="(scenario, index) in user.scenarios"
                    v-bind:key="scenario.id"
                >
                    <select-scenario
                        v-on:scenario-selected="setupHub"
                        :scenario="scenario"
                        :numbering="index + 1"
                    />
                </div>
            </div>           
            <router-link v-if="$store.getters.isAdmin" class="nav-link" to="admin">Redigering av scenario</router-link>            
        </div>
        <div v-else>
            <session-information
                :group-name="user.groupName"
                :typeable-code="user.typeableCode"
            />
            <hr/>

            <ScenarioControls @manage-session="manageSession"/>
            <hr/>
            <div class="participant-row" id="participant-row">
                <participants-list
                    v-bind:sessions="sessions"
                    class="participants"
                    v-bind:session-id="searchFilter.sessionId"
                    v-on:select="sessionIdSelected"
                />
                <stress-slider
                    @changeStressLevel="onChangeStressLevel"
                    class="level"
                    :value="stressLevel"
                />
            </div>
            <hr/>
            <div class="messages-row">
                <facilitator-activity-list :search-filter="searchFilter"/>
                <facilitator-messages-list class="messages-feed"/>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';
    import {SessionGroupStatus} from '@/Types/SessionGroupStatus';
    import {SessionLog} from '@/Types/SessionLog';
    import {Session} from '@/Types/Session';
    import ScenarioEvent from '../Types/ScenarioEvent';
    import Phase from '../Types/Phase';
    import MessageComponent from '@/components/MessageComponent.vue';
    import DialogComponent from '@/components/DialogComponent.vue';
    import ActionPanel from '@/components/ActionPanel.vue';
    import SelectScenario from '@/components/SelectScenario.vue';
    import ParticipantsList from '@/components/ParticipantsList.vue';
    import FacilitatorMessagesList from '@/components/FacilitatorMessagesList.vue';
    import ScenarioControls from '@/components/ScenarioControls.vue';
    import SessionInformation from '@/components/SessionInformation.vue';
    import {mapState} from 'vuex';
    import FacilitatorActivityList from '@/components/FacilitatorActivityList.vue';
    import SearchFilter from '../Types/SearchFilter';
    import StressSlider from '@/components/StressSlider.vue';
    import FacilitatorMixin from '../mixins/FacilitatorMixin';
    import {mixins} from 'vue-class-component';
    import {User} from '@/Types/User';

    @Component({
        computed: mapState(['user', 'stressLevel']),
        components: {
            ParticipantsList,
            MessageComponent,
            SelectScenario,
            ScenarioControls,
            FacilitatorMessagesList,
            SessionInformation,
            DialogComponent,
            ActionPanel,
            FacilitatorActivityList,
            StressSlider,
        },
    })
    export default class FacilitatorView extends mixins(FacilitatorMixin) {
        private $hub: any;
        private hubStartPromise: any;
        protected user!: User;
        private searchFilter: SearchFilter = new SearchFilter();

        get sessionStatus(): SessionGroupStatus {
            return this.$store.state.status;
        }

        get sessions(): Session[] {
            return [...this.$store.state.user.sessions];
        }

        private manageSession(message: string) {
            switch (message) {
                case 'start':
                    this.start();
                    break;
                case 'cancel':
                    this.cancel();
                    break;
                case 'pause':
                    this.pause();
                    break;
                case 'unpause':
                    this.unpause();
                    break;
                case 'leave':
                    this.leave();
                    break;
                case 'stop':
                    this.stop();
                    break;
            }
        }

        public sessionIdSelected(id: number | null) {
            this.searchFilter.sessionId = id;
        }

        public leave() {
            this.$store.dispatch("uncoupleSessionGroup")
            .then(() => {
                this.leaveSessionGroup();    
            });
            
        }

        private leaveSessionGroup() {
            this.$store.dispatch('clearSessions');
            this.$store.dispatch('setStatus', 'Cancelled');
            this.$store.dispatch('setSessionGroupId', '');
            this.$store.dispatch('clearScenarioEvent');
            this.$store.dispatch('clearActivityLog');
        }

        public showForParticipants(message: SessionLog) {
            if (this.$store.state.status === 'Running') {
                this.$store
                    .dispatch(
                        'pauseSessionsNoNotification',
                        this.user.sessionGroupId
                    )
                    .then(() => {
                        this.$store.dispatch('setStatus', 'Paused');
                    });
            }
            this.$hub.invoke('showForGroup', message.id);
        }

        public start(): void {
            this.$store
                .dispatch('startSessions', this.user.sessionGroupId)
                .then(() => {
                    console.log('started');
                });
        }

        public stop(): void {
            this.$store
                .dispatch('stopSessions', this.user.sessionGroupId)
                .then(() => {
                    console.log('stopped');
                });
        }

        public cancel(): void {
            this.$store
                .dispatch('cancelSessions', this.user.sessionGroupId)
                .then(() => {
                    this.leaveSessionGroup();
                });
        }

        public pause(): void {
            this.$store
                .dispatch('pauseSessions', this.user.sessionGroupId)
                .then(() => {
                    console.log('paused');
                });
        }

        public unpause(): void {
            this.$store
                .dispatch('unpauseSessions', this.user.sessionGroupId)
                .then(() => {
                    console.log('unpaused');
                });
        }

        private onChangeStressLevel(level: number) {
            this.$store
                .dispatch('changeStressLevel', {
                    sessionGroupId: this.user.sessionGroupId,
                    level,
                })
                .catch(() => {
                    console.log('Failed to set stress level');
                });
        }

        public created() {
            if(!this.$store.getters.isFaciliator) {
                this.$router.push('signin');
            }
            this.$hub.onclose(() => {
                this.hubStartPromise.then(() => {
                    this.setupHub();
                });
            });
        }

        public destroyed() {
            this.$hub.off('participantJoined', this.participantJoinedCallback);
            this.$hub.off('events', this.eventCallback);
            this.$hub.off('activityLog', this.activityCallback);
            this.$hub.off('stressLevel', this.stressLevelCallback);
        }

        private participantJoinedCallback = () => {
            console.log('Participant joined. Reloading sessions.');
            this.loadSessions();
        };
        
        private eventCallback = (event: any) => {
            this.$store.dispatch('addScenarioEvent', event.sessionLog);
        };
        
        private activityCallback = (comment: any) => {
            this.$store.dispatch('addActivityLog', comment.sessionLog);
        };
        
        private stressLevelCallback(stressLevel: number) {
            this.$store.dispatch('changeStressLevel', stressLevel);
        }

        protected setupHub() {
            if (this.user.sessionGroupId) {
                Vue.prototype.hubStartPromise.then(() => {
                    console.log(
                        'Facilitator for group ' + this.user.sessionGroupId
                    );
                    console.log('Have connection to hub');

                    this.$hub.invoke(
                        'FacilitateGroup',
                        this.user.sessionGroupId
                    );

                    this.$hub.on(
                        'participantJoined',
                        this.participantJoinedCallback
                    );
                    this.$hub.on('events', this.eventCallback);
                    this.$hub.on('activityLog', this.activityCallback);
                    this.$hub.on('changeStressLevel', this.setStressLevel);

                    console.debug(
                        `Listening on: ${this.user.sessionGroupId}`
                    );
                    this.$root.$on('showForParticipants', this.showForParticipants);
                });
            } else {
                console.error('Hub failure.');
            }
        }

        private setStressLevel(stressLevel: number) {
            this.$store.dispatch('setStressLevel', stressLevel);
        }
    }
</script>
<style scoped lang="scss">
    @import '../assets/scss/typography';
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';

    .message-handle {
        position: relative;
        height: 60vh;
        overflow: hidden;
        background-color: #fff;
        border-radius: 6px;
        width: 70vw;
    }

    .dialog-content {
        h1 {
            text-align: center;
            color: #fff;
            margin-bottom: $space-sm;
        }
    }

    .facilitator-view {
        margin-top: 80px;
    }

    .meta {
        padding-bottom: 40px;
    }

    .title {
        @include typography(extrabold, 38, $default-text-color);
    }

    .code {
        @include typography(light, 38, $default-text-color);

        span {
            letter-spacing: 7.6px;
        }
    }

    hr {
        display: block;
        height: 3px;
        width: 100%;
        border: 0;
        box-shadow: inset 0px 1px 1px 0px $border-primary-color,
        inset 0px -1px 1px 0px $border-secondary-color;
        padding: 0;
        margin: 0;
    }

    .participant-row,
    .messages-row {
        display: flex;
        flex-direction: row;
    }

    .messages-row {
        padding-top: $space-md;
    }

    .action-buttons {
        display: flex;
        justify-content: space-between;
        margin-bottom: $space-lg;

        button {
            flex: 0 0 calc(33.3% - 10px);
            margin-right: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            

            &.handle::after {
                background-image: url('../assets/icons/open.svg');
            }

            &.share::after {
                background-image: url('../assets/icons/share.svg');
                width: 16px;
                height: 16px;
            }

            &.link::after {
                background-image: url('../assets/icons/link.svg');
            }
        }

        button:nth-child(n + 3) {
            margin-right: 0px;
        }
    }

    .participants,
    .level,
    .messages-feed {
        width: 453px;
        flex-grow: 0;
        flex-shrink: 0;
    }

    .messages-main {
        flex-grow: 1;
        padding-right: $space-sm;
    }
</style>
