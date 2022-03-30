<template>
    <div class="participant-view" v-cloak>
        <ToasterComponent v-bind:messageFlow="searchFilter.messageFlow">
        </ToasterComponent>
        <div class="tabs">
            <!-- 1 == MessageFlow.Short -->
            <div
                @click="selectTab(1)"
                class="tab"
                v-bind:class="{ active: searchFilter.messageFlow === 1 }"
            >
                <span>Kwitter</span>
                <span class="badge badge-shortmessage">{{this.unread(1)}}</span>
            </div>
            <!-- 2 == MessageFlow.Long -->
            <div
                @click="selectTab(2)"
                class="tab"
                v-bind:class="{ active: searchFilter.messageFlow === 2 }"
            >
                <span>Friendzone</span>
                <span class="badge">{{ this.unread(2) }}</span>
            </div>
        </div>

        <div class="view">
            <div class="participant-content">
                <DialogComponent
                    :open="dialog.open"
                    @modalClosed="dialogClosed()"
                    :dark="dialog.newsEvent"
                    :enableClose="status != 'Paused'"
                    v-bind:high-priority="true"
                >
                    <div class="transparent-dialog" v-if="dialog.newsEvent">
                        <p class="sender" v-text="dialog.sender"></p>
                        <h1 class="heading" v-text="dialog.title"></h1>
                        <p class="content" v-html="dialog.content"></p>
                    </div>

                    <div class="card" v-if="!dialog.newsEvent">
                        <h1 v-text="dialog.title"></h1>
                        <p v-text="dialog.content"></p>
                    </div>
                </DialogComponent>

                <div class="layout">
                    <div class="col-l">
                        <div v-if="joined">
                            <participant-information></participant-information>
                        </div>
                    </div>

                    <div class="col-c">
                        <div
                            v-if="
                                status === 'Finished' || status === 'Cancelled'
                            "
                        >
                            <button @click="leave()" id="leave-session">
                                Lämna övningen
                            </button>
                        </div>

                        <div v-if="joined">
                            <div class="messages">

                                <MessageFieldComponent v-if="status !== 'Finished' && status !== 'Cancelled'"
                                                         :flow="searchFilter.messageFlow" 
                                                         :text-input="textInput"
                                                         @submit="sendComment"
                                                         :focus="true"
                                                         :compact="true"
                                >
                                </MessageFieldComponent>
                                
                                <MessageComponent
                                    v-for="message in messages"
                                    v-bind:key="message.id"
                                    :enable-comment="true"
                                    :message="message"
                                />
                                
                                
                                <DialogComponent
                                    :open="openMessageDialog"
                                    @modalClosed="openMessageDialog = false"
                                    v-if="messageToComment"
                                    v-bind:high-priority="false"
                                >
                                    <MessageComponent
                                        v-bind:enable-comment="true"
                                        v-bind:message="messageToComment"
                                        :showInput="true"
                                        v-if="messageToComment"
                                        v-bind:commentsScroll="true"
                                    />
                                </DialogComponent>

                                <DialogComponent
                                    v-bind:high-priority="true"
                                    :open="showMessageDialog"
                                    @modalClosed="showMessageDialog = false"

                                >
                                    <h1 class="dialog-heading">Pausad</h1>
                                    <MessageComponent
                                        v-bind:enable-comment="false"
                                        v-bind:message="show"
                                        :showInput="false"
                                        v-bind:commentsScroll="true"
                                    />
                                </DialogComponent>
                                
                                
                            </div>
                        </div>
                    </div>
                    <!-- <div class="col-r">
          

                </div> -->
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue, Watch } from 'vue-property-decorator';
import axios, { AxiosResponse } from 'axios';
import { SessionLog } from '../Types/SessionLog';
import Dialog from '../Types/Dialog';
import MessageComponent from '@/components/MessageComponent.vue';
import DialogComponent from '@/components/DialogComponent.vue';
import { mapState, MutationPayload } from 'vuex';
import ParticipantInformation from '@/components/ParticipantInformation.vue';
import { MessageFlow } from '../Types/MessageFlow';
import SearchFilter from '../Types/SearchFilter';
import router from '../router';
import { MessageType } from '@aspnet/signalr';
import { SessionLogType } from '../Types/SessionLogType';
import ToasterComponent from '@/components/ToasterComponent.vue';
import MessageFieldComponent from "@/components/MessageFieldComponent.vue";
import CommentData from "@/Types/CommentData";

@Component({
    computed: mapState(['status', 'session', 'show', 'shortRead', 'longRead']),
    components: {
        MessageFieldComponent,
        MessageComponent,
        ParticipantInformation,
        DialogComponent,
        ToasterComponent,
    },
})
export default class Participant extends Vue {
    public answer: string = '';
    public comment: string = '';
    public openMessageDialog: boolean = false;
    public selectedSessionGroup: any = null;
    public searchFilter: SearchFilter = new SearchFilter(MessageFlow.Short);
    public status!: string;
    public session!: any;
    public show!: SessionLog;
    public shortRead!: number;
    public longRead!: number;
    private $hub!: any;
    public textInput: string = '';

    public messageToComment: SessionLog = {
        sessionGuid: '',
        botReplyProperties: 0,
        sessionLogTag: 0,
        avatar: '',
        children: [],
        heading: '',
        id: 0,
        scenarioEventId: '',
        postId: '',
        sendDateTime: new Date(),
        sender: '',
        sessionId: '',
        text: '',
        messageType: '',
        messageFlow: 1,
        level: 0,
    };
    private showMessageDialog: boolean = false;

    private unsubscibe!: () => void;

    public sendComment(message:string) {
        const messageFlow = this.searchFilter.messageFlow;
        const commentData: CommentData = {
            sender: 'Organisation',
            avatar: null,
            messageType: 'Participant',
        };

        if (this.status !== 'Running') {
            return;
        }

        if(!message)
            return;

        let args = {
            Text: message,
            Sender: commentData.sender,
            Avatar: commentData.avatar,
            MessageType: commentData.messageType,
            SessionGroup: this.session.sessionId,
            SessionLogId: null,
        };
        
        Vue.prototype.$hub
            .invoke('SendPost', args, messageFlow);
    }
    
    private openShowMessageDialog() {
        this.showMessageDialog = true;
    }

    public selectTab(tab: number) {
        this.resetUnread(tab);
        this.searchFilter.messageFlow = tab || MessageFlow.Short;
    }

    private resetUnread(flow: number) {
        if (flow === MessageFlow.Short) {
            this.$store.dispatch('setShortRead', this.total(MessageFlow.Short));
        } else if (flow === MessageFlow.Long) {
            this.$store.dispatch('setLongRead', this.total(MessageFlow.Long));
        }
        this.$store.dispatch('clearNewSessionLogs');
    }

    public unread(flow: number): number {
        let total: number = 0;
        switch (flow) {
            case MessageFlow.Short:
                total = this.total(flow) - this.shortRead;
                return total < 0 ? 0 : total;
            case MessageFlow.Long:
                total = this.total(flow) - this.longRead;
                return total < 0 ? 0 : total;
            default:
                return 0;
        }
    }

    private countBranch(message: SessionLog): number {
        let sum = 1;
        if (message.children) {
            message.children.forEach((child) => {
                sum += this.countBranch(child);
            });
        }

        return sum;
    }

    private countFlow(flow: MessageFlow): number {
        const flowSessionLogs = this.$store.state.session.messages.filter(
            (s) => s.messageFlow === flow && s.type !== SessionLogType.event
        );

        let count = 0;

        flowSessionLogs.forEach((s) => {
            count += this.countBranch(s);
        });

        return count;
    }

    public total(flow: number): number {
        switch (flow) {
            case MessageFlow.Short:
                return this.countFlow(MessageFlow.Short);

            case MessageFlow.Long:
                return this.countFlow(MessageFlow.Long);
            default:
                return 0;
        }
    }

    get joined(): boolean {
        return this.session.status !== '' && this.session.sessionId !== '';
    }

    get messages(): SessionLog[] {
        let sessionLog: SessionLog[] = this.$store.state.session.messages;
        sessionLog = sessionLog.filter((sl) => sl.id !== 0);
        sessionLog.sort((a, b) =>
            a.sendDateTime < b.sendDateTime
                ? 1
                : b.sendDateTime < a.sendDateTime
                ? -1
                : 0
        );

        return this.searchFilter.filter(sessionLog);
    }

    get dialog(): Dialog {
        return this.$store.state.dialog;
    }

    public dialogClosed() {
        this.$store.dispatch('closeDialog');
    }
    
    
    public leave(): void {
        this.$store.dispatch('setStatus', '');
        this.$store.dispatch('setSessionId', '');
        this.$store.dispatch('setParticipant', '');
        this.$store.dispatch('clearMessages');
        this.$router.push({ name: 'join' });
    }

    public selectSessionGroup(sessionGroup) {
        this.selectedSessionGroup = sessionGroup;
    }

    

    public loadAll() {
        this.$store
            .dispatch('reloadMessages', this.session.sessionId)
            .then(() => {
                Vue.prototype.hubStartPromise
                    .then(() => {
                        Vue.prototype.$hub.invoke(
                            'JoinSessionGroup',
                            this.session.sessionId
                        );
                    })
                    .catch((error) => {
                        if (error.response.status === 404) {
                            this.$store.dispatch('setSessionId', '');
                        }
                    });
            })
            .catch(() => {
                this.leave();
            });
    }

    private addMessageHandler(mutation: MutationPayload) {
        const sl: SessionLog = mutation.payload.sessionLog;

        if (this.searchFilter.messageFlow !== sl.messageFlow) {
            return;
        }

        if (this.searchFilter.messageFlow === MessageFlow.Short) {
            this.$store.dispatch('setShortRead', this.total(MessageFlow.Short));
        } else if (this.searchFilter.messageFlow === MessageFlow.Long) {
            this.$store.dispatch('setLongRead', this.total(MessageFlow.Long));
        }
    }

    public destroyed() {
        this.unsubscibe();
    }

    public created() {
        this.unsubscibe = this.$store.subscribeAction({
            before: (action, state) => {
                // no action
            },
            after: (action, state) => {
                if (action.type === 'showForGroup') {
                    this.openShowMessageDialog();
                }
                if (
                    action.type === 'addMessage' ||
                    action.type === 'addComment'
                ) {
                    this.addMessageHandler(action);
                }
                
                if(action.type == 'commentDialog') {
                    this.messageToComment = action.payload;
                    this.openMessageDialog = true;
                    
                }
            },
        });

        if (this.session.sessionId) {
            this.$store.dispatch('setSessionId', this.session.sessionId);
        } else {
            router.push({ name: 'join' });
        }

        this.$store
            .dispatch('getStatus', this.session.sessionId)
            .catch((error) => {
                if (error.payload.status === 404) {
                    router.push({ name: 'join' });
                }
            });

        this.$store.dispatch('loadNewSessionGroups');
        this.loadAll();

        Vue.prototype.hubStartPromise.then((promise) => {
            console.log('connection');
            if (!this.session.sessionId) {
                Vue.prototype.$hub.invoke(
                    'JoinSessionGroup',
                    this.session.sessionId
                );
            }
        });
    }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style lang="scss" scoped>
@import '../assets/scss/spacings.scss';
@import '../assets/scss/typography.scss';
@import '../assets/scss/colors.scss';

.participant-view {
    .participant-content {
        .messages {
            margin: 0 auto;
            width: 100%;
        }
    }

    #leave-session {
        margin-bottom: $space-md;
    }

    .col-l {
        margin-top: 54px;
    }

    .col-c {
        margin-top: 40px;
    }

    .tabs {
        display: flex;

        .tab {
            flex: 1;
            height: 65px;
            background-color: $primary-button-foreground-color;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
            cursor: pointer;
            &.active {
                background-color: #fff;

                &:after {
                    content: '';
                    display: block;
                    position: absolute;
                    top: 65px;
                    width: 0;
                    height: 0;
                    border-left: 10px solid transparent;
                    border-right: 10px solid transparent;

                    border-top: 10px solid #fff;
                }
            }

            .badge {
                margin-left: 10px;
                background-color: $primary-button-background-color;
                width: 26px;
                height: 26px;
                color: #fff;
                border-radius: 50%;
                text-align: center;
                display: flex;
                align-items: center;
                justify-content: center;
                &.badge-shortmessage {
                    background-color: $pink;
                }
            }
        }
    }
}
</style>
