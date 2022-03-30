<template>
    <div class="message-component">
        <div
            class="message card"
            v-if="message.messageType != 'ScenarioEvent'"
            :id="'sessionLog' + message.id"
        >
            <div class="message-user">
                <div class="avatar" >
                    <div v-if="message.avatar != null">
                        <img
                            src="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"
                            v-bind:style="{
                            backgroundImage: 'url(' + message.avatar  + ')',
                        }"
                            alt=""
                        />    
                    </div>
                    <AvatarComponent v-else src=""></AvatarComponent>
                </div>
                
                <div class="sender">
                    <div class="name" v-text="message.sender"></div>
                    <div
                        class="date"
                        v-text="formatDate(message.sendDateTime)"
                    ></div>
                </div>
            </div>

            <div class="message-content">
                <b v-text="message.heading"></b>
                <p v-html="message.text"></p>
                <p
                    v-if="message.children &&  message.children.length > 0 && showCommentCount"
                    class="count"
                >
                    {{ countChildren( message ) }} kommentarer
                </p>
                <Comments
                    ref="commentsRef"
                    v-if="!rootOnly"
                    :sessionId="sessionId"
                    :comments="message.children"
                    :commentsScroll="commentsScroll"
                ></Comments>
            </div>
            <div
                class="message-comment"
                v-if="message.messageType != 'ScenarioEvent' && enableComment"
            >
                <a
                    href=""
                    @click.prevent="() => openCommentDialog(message)"
                    v-if="!showInput" 
                >
                    <span>Kommentera </span>
                    <img src="../assets/comment.svg" />
                </a>
                
                <MessageFieldComponent :focus="true"  v-if="showInput" :flow="flow" @submit="sendComment"></MessageFieldComponent>
                
            </div>
        </div>
        
        
        <div class="event"
             :class="{'new':isNew}"
             v-if="message.messageType == 'ScenarioEvent'" 
             :id="'sessionLog' + message.id">
            <div class="event-content">
                <div class="sender" v-text="message.sender"></div>
                <div class="label">Ny</div>
                <div class="heading" v-text="message.heading"></div>
            </div>
            <div class="event-bottom">
            <div class="event-bottom">
                <button class="tertiary" @click="showEvent(message)">
                    Läs mer
                </button>
            </div>
        </div>
    </div>
</div>
</template>

<script lang="ts">
    import {Component, Emit, Prop, Vue} from 'vue-property-decorator';
    import {SessionLog} from '@/Types/SessionLog';
    import Comments from '@/components/Comments.vue';
    import {mapState} from 'vuex';
    import CommentData from '../Types/CommentData';
    import MessageFieldComponent from "@/components/MessageFieldComponent.vue";
    import AvatarComponent from "@/components/AvatarComponent.vue";
    import State from "@/Types/State";
    import {User} from "@/Types/User";

    interface IScroll {
    scroll();
}

@Component({
    computed: mapState(['status']),
    components: {
        MessageFieldComponent,
        Comments,
        AvatarComponent
    },
})
export default class MessageComponent extends Vue {
    // public sessionId!: string = '';
    public comment: string = '';
    @Prop({ default: Object as () => SessionLog })
    public message!: SessionLog;
    @Prop({ default: false })
    public enableComment!: boolean;
    @Prop({ default: false })
    private rootOnly!: boolean;
    @Prop({ default: false })
    private showInput!: boolean;
    @Prop({ default: true })
    private showCommentCount!: boolean;
    @Prop({ default: false })
    private commentsScroll!: boolean;
    
    get isNew(): boolean {
        return (this.$store.state as State).newSessionLogs.indexOf(this.message.id) >= 0 ;
    }
    
    get flow() {
        return this.message.messageFlow;
    }

    //#region (mapsate)
    public status!: string;
    //#endregion

    private openCommentDialog(sessionLog:SessionLog): void {
        this.$store.dispatch('commentDialog', sessionLog);
    }

    public sendComment(textInput:string): void {
        const commentData: CommentData = {
            sender: 'Organisation',
            avatar: null,
            messageType: 'Participant',
        };

        if (this.status !== 'Running')
            return;
        
        if(!textInput)
            return;
        
        if (this.$store.getters.isFaciliator) {
            
            this.$store.dispatch('getFakeAlias').then((p => {

                commentData.sender = p.alias;
                commentData.avatar = '/circle.svg';
                commentData.messageType = 'Comment';
                
                Vue.prototype.$hub
                    .invoke('SendComment', {
                        Text: textInput,
                        Sender: commentData.sender,
                        Avatar: commentData.avatar,
                        MessageType: commentData.messageType,
                        SessionGroup: this.message.sessionGuid,
                        SessionLogId: this.message.id
                        })
                    .then(() => {
                        textInput = '';
                        this.reloadMessage();
                        if(this.$refs.commentsRef)
                            (this.$refs.commentsRef as IScroll).scroll();
                    }).catch((e) => {
                        console.error(e);
                    }
                );
                
            }))
            
        }
        else {       
            Vue.prototype.$hub
                .invoke('SendComment', {
                    Text: textInput,
                    Sender: commentData.sender,
                    Avatar: commentData.avatar,
                    MessageType: commentData.messageType,
                    SessionGroup: this.$store.state.session.sessionId,
                    SessionLogId: this.message.id
                })
                .then(() => {
                    textInput = '';
                    this.reloadMessage();
                    if(this.$refs.commentsRef)
                        (this.$refs.commentsRef as IScroll).scroll();
                }).catch(e => {console.error(e)});
        }
    }

    reloadMessage() {
        if (this.message.level == 2) {
            let sessionLog: SessionLog[] = this.$store.state.session.messages;

            const findItemNested = (arr, itemId, nestingKey) => (
                arr.reduce((a, item) => {
                    if (a) return a;
                    if (item.id === itemId) return item;
                    if (item[nestingKey]) return findItemNested(item[nestingKey], itemId, nestingKey)
                }, null)
            );
            const res = findItemNested(sessionLog, this.message.id, "children");
            this.message.children = res.children;
        }
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
        return sessionLog;
    }
    get sessionId(): string {
        return this.$store.state.session.sessionId;
    }

    public showEvent(scenarioEvent) {
        const dialog = {
            open: true,
            title: scenarioEvent.heading,
            content: scenarioEvent.text,
            sender: scenarioEvent.sender,
            newsEvent: true,
        };

        this.$store.dispatch('showDialog', dialog);
    }

    private countChildren (sessionLog:SessionLog, count: number = 0): number  {

        count += sessionLog.children.length;

        sessionLog.children.forEach(c => {
            count = this.countChildren(c, count);
        });

        return count;
    }

    private formatDate(date: string) {
        if (!date) {
            return '';
        }

        const d = new Date(date);
        let minutes = '' + d.getMinutes();
        let hours = '' + d.getHours();
        let month = '' + (d.getMonth() + 1);
        let day = '' + d.getDate();
        const year = d.getFullYear();
        if (month.length < 2) {
            month = '0' + month;
        }
        if (day.length < 2) {
            day = '0' + day;
        }
        if (hours.length < 2) {
            hours = '0' + hours;
        }
        if (minutes.length < 2) {
            minutes = '0' + minutes;
        }

        return [hours, minutes].join(':');
    }
}
</script>

<style lang="scss" scoped>
@import '../assets/scss/spacings.scss';
@import '../assets/scss/colors.scss';
@import '../assets/scss/typography.scss';
@import '../assets/scss/elements.scss';

.count {
    text-align: right;
    @include typography('regular', 14, $dark-gray2);
}

.message-form {
    .comment-input {
        width: 100%;
        margin-bottom: 15px;
    }
    .right {
        text-align: right;
    }
}

.card {
    margin-bottom: $space-sm;
}
.event {
    
    background-color: $third-background-color;
    border-radius: 6px;
    margin-bottom: $space-sm;


    .label {
        display: none;
    }
    
    &.new {
        border: 2px solid $pink;

        .label {
            display: block;
            @include typography(regular,14px,$pink);
            letter-spacing: 0.9px;
            margin-right: 15px;
        }
        
        .label::after {
            content: '\00a0';
            display: inline-block;
            position: relative;
            left: 7px;
            top: 3px;
            @include dot($pink, xs);
        }
    }
    
    
    .event-content {
        padding: $space-md;
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        
        .sender {
            @include typography('light', 20, $primary-button-background-color);
        }
        
     
        
        .heading {
            flex-grow: 2;
            flex-basis: 100%;
            min-width: 100%;
            font-size: 26px;
            font-family: 'open_sansextrabold';
        }
    }
    .event-bottom {
        background-color: #fff;
        padding: $space-md;
        border-radius: 0 0 6px 6px;
        text-align: center;
    }
}
.message {
    .message-user {
        margin-bottom: $space-sm;
        display: flex;

        .sender {
            .name {
                color: $primary-button-background-color;
                font-weight: bold;
            }
            .date {
                color: $secondary-button-foreground-color;
                font-weight: 300;
                font-size: 14px;
            }
        }
    }
    .message-content {
        padding-bottom: $space-md;
    }
    .message-comment {
        border-top: 1px solid $primary-background-color;
        padding-top: $space-md;

        a {
            display: flex;
            justify-content: center;
            align-items: center;
            text-decoration: none;
            span {
                line-height: 20px;
                height: 30px;
                font-weight: bold;
            }
            img {
                padding-bottom: 5px;
                padding-left: 5px;
            }
        }
    }
    
    .avatar {
        width: 50px;
        height: 50px;
        margin-right: 10px;
        img {
            width: 50px;
            border-radius: 50%;
            background-size: contain;
        }
    }

    .participant {
        width: 80%;
        margin-left: auto;
        color: #fff;
        padding: 5px;
        border-radius: 10px;
    }
}
</style>
