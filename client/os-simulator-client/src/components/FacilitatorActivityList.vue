<template>
    <div class="messages-main">
        <div
            class="message"
            v-for="message in activityLog"
            :message="message"
            v-bind:key="message.id"
        >
            <message-component
                :message="message"
                :enableComment="false"
                @openCommentDialog="openCommentDialog"
            />

            <div class="action-buttons">
                <button
                    class="secondary handle"
                    @click="handleMessage(message)"
                >
                    Hantera kommentar
                </button>
                <button class="secondary share" @click="showForParticipants(message)">
                    Visa för grupp
                </button>
                <button class="secondary link" @click="scrollToSessionLog(message)" v-if="message.messageType != 'Participant'">
                    Se kommentar i flödet
                </button>
            </div>
            <FacilitatorMessageFlags :message="message"></FacilitatorMessageFlags>
        </div>
        <DialogComponent
            :open="openActionDialog"
            @modalClosed="openActionDialog = false"
        >
            <h1>Hantera kommentar</h1>
            <div class="message-handle">
                <message-component
                    :message="messageToHandle()"
                    :enableComment="true"
                    :showInput="true"
                    @openCommentDialog="openCommentDialog"
                    v-bind:commentsScroll="true"
                />
            </div>
            <ActionPanel :sessionLog="messageToHandle()"></ActionPanel>
            
        </DialogComponent>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { mapState } from 'vuex';
import MessageComponent from '@/components/MessageComponent.vue';
import { SessionLog } from '@/Types/SessionLog';
import DialogComponent from '@/components/DialogComponent.vue';
import SearchFilter from '../Types/SearchFilter';
import ActionPanel from '@/components/ActionPanel.vue';
import {User} from '@/Types/User';
import FacilitatorMessageFlags from '@/components/FacilitatorMessageFlags.vue';
@Component({
    computed: mapState(['user']),
    components: {
        MessageComponent,
        DialogComponent,
        ActionPanel,
        FacilitatorMessageFlags,
    },
})
export default class FacilitatorActivityList extends Vue {
    public openActionDialog: boolean = false;
    public openDialog: boolean = false;
    public user!: User;
    @Prop({ default: Object as () => SearchFilter })
    public searchFilter!: SearchFilter;
    private $hub: any;
    private messageToHandleId: any | null;

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

    public showForParticipants(message: SessionLog) {
        this.$root.$emit('showForParticipants', message);
    }

    public scrollToSessionLog(sessionLog: SessionLog) {
        if (this.$store.state.user.selectedFlow !== sessionLog.messageFlow) {
            this.$store.dispatch('setSelectedFlowFacilitator', sessionLog.messageFlow);
        }
        setTimeout(() => {
            const slElement = '#sl-' + sessionLog.id.toString();
            this.$scrollTo(slElement, 700, {offset: -70, easing: 'ease'});
            }
            , 500);
    }

    get activityLog(): SessionLog[] {
        return this.searchFilter.filter(this.$store.state.user.activityLog);
    }

    private messageToHandle() {
        return this.$store.state.user.activityLog.filter((s) => s.id === this.messageToHandleId)[0];
    }

    public handleMessage(message: SessionLog) {
        this.messageToHandleId = message.id;
        this.openActionDialog = true;
    }

    public openCommentDialog(message): void {
        console.log(message);
        this.messageToComment = message;
        this.openDialog = true;
    }

}
</script>

<style lang="scss" scoped>
    @import '../assets/scss/spacings.scss';
    @import '../assets/scss/colors.scss';
    .action-buttons{
        display: flex;
        margin-bottom: $space-lg;
        button {
            flex:1;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            padding: 11px 10px;
            margin-right: 10px;
            &:nth-child(3){
                margin-right: 0;
            }
            &::after {
                content: '';
                width: 14px;
                height: 14px;
                display: inline-block;
                margin-left: 10px;
                background-size: contain;
                background-repeat: no-repeat;
                background-position: center center;                
            }
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
    }
    
</style>
