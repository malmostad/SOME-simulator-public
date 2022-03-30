<template>
    <div class="facilitator-messages-list">
        <div class="feed-buttons">
            <button 
                v-for="messageFlow in messageFlows"
                :class="{ active: messageFlow === user.selectedFlow }" @click="selectMessageFlow(messageFlow)"
                :key="messageFlow">
                {{messageFlow == 1 ? 'Kwitter': 'Friendzone'}}
            </button>
        </div>
        <div class="messages">
            <div class="message-container" :id="'sl-' + message.id" v-for="message in messages" v-bind:key="message.id">
                <message-component
                    :message="message"
                    @openCommentDialog="openCommentDialog"
                    :enableComment="false"
                    :rootOnly="true"
                    :showCommentCount="false"
                />    
                
            </div>
            
        </div>

        <DialogComponent :open="showEvent" :dark="true" @modalClosed="closeDialog">
            <div class="transparent-dialog" v-if="dialog.newsEvent">
                <p class="sender" v-text="dialog.sender"></p>
                <h1 class="heading" v-text="dialog.title"></h1>
                <p class="content" v-html="dialog.content"></p>
            </div>
        </DialogComponent>
    </div>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator';
    import {mapState} from 'vuex';
    import {SessionLog} from '@/Types/SessionLog';
    import MessageComponent from '@/components/MessageComponent.vue';
    import {User} from '@/Types/User';
    import SearchFilter from '@/Types/SearchFilter';
    import {MessageFlow} from '@/Types/MessageFlow';
    import DialogComponent from '@/components/DialogComponent.vue';
    import Dialog from '@/Types/Dialog';

    @Component({
    computed: mapState(['user', 'dialog']),
    components: {
        MessageComponent, DialogComponent,
    },
})
export default class FacilitatorMessagesList extends Vue {

    public searchFilter: SearchFilter = new SearchFilter();
    private user!: User;
    private dialog!: Dialog;
    private messageFlows: number[] = [MessageFlow.Short, MessageFlow.Long];
    get messages(): SessionLog[] {
        let sessionLog: SessionLog[] = this.$store.state.user.eventLog;
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
    public openCommentDialog() {
        return false;
    }

    public created() {
        this.loadEvents();
        this.$store.watch((state) => state.user.selectedFlow, (newFlow) => {
           this.searchFilter.messageFlow = newFlow;
        });
    }

    private loadEvents() {
        this.$store
            .dispatch('loadEvents', this.user.sessionGroupId)
            .catch(() => {
                console.log('Failed to load activity');
            });
    }
    private selectMessageFlow(flow: number) {
        this.searchFilter.messageFlow = flow;
        this.$store.dispatch('setSelectedFlowFacilitator', flow);
    }

    private closeDialog() {
        this.$store.dispatch('closeDialog');
    }

        get showEvent() {
            return this.dialog.open === true && this.dialog.newsEvent === true;
        }
}
</script>

<style lang="scss" scoped>
@import '../assets/scss/colors';
@import '../assets/scss/typography';
@import '../assets/scss/elements';
    .feed-buttons {
        display:flex;
        flex-direction:row;
        padding-bottom:23px;
        button {
            position:relative;
            flex:1;
            background-color: $secondary-button-background-color;
            color: $secondary-button-foreground-color;
            &:first-child {
                margin-right: 5px;
            }
            &.active {
                background-color:$secondary-background-color;
            }
        }
    }
</style>
