<template>
    <div
        :id="'sessionLog'+comment.id"
        class="comment">
        <div class="avatar">
            <AvatarComponent v-if="comment.messageType == 'Participant'"></AvatarComponent>
            <div v-else>
                <div class="avatar" :style="{ 'background': 'url(' + comment.avatar + ')' }" alt=""></div>
            </div>
        </div>

        <p class="contents">
            <span class="comment-name" v-text="getSender(comment)"></span>
            <span class="comment-text" v-text="comment.text"></span>
        </p>

        <p class="break"></p>
        
        <p class="comment-link" v-if="canComment()">
            <a @click="openCommentDialog">Svara</a>
        </p>
        
        <div class="children" v-for="child in comment.children">
            <comment :name="child.id" :comment="child">
            </comment>
        </div>
    </div>
</template>

<script lang="ts">
    import {Component, Emit, Prop, Vue} from 'vue-property-decorator';
    import {SessionLog} from '../Types/SessionLog';
    import AvatarComponent from '@/components/AvatarComponent.vue';

    @Component({
        components: {AvatarComponent},
        name: 'Comment'
    })
    export default class Comments extends Vue {
        
        @Prop({ default: '' })
        private sessionId!: string;

        @Prop({ required: true, default: Object as () => SessionLog })
        private comment!: SessionLog;

        private canComment():boolean {
            return this.isParticipant() && (this.comment.messageType == 'Comment' || this.comment.messageType == 'Participant') && this.comment.level <= 2;
        }
        
        private openCommentDialog(): void {
            this.$store.dispatch('commentDialog', this.comment);
        }
        
        private isParticipant () : boolean {
            return !this.isFaciliator();
        }
        
        private isFaciliator(): boolean {
            return this.$store.getters.isFaciliator;
        }
        
        private getSender() {
            if (this.isParticipant()) {
                // Participant
                return this.comment.sender;
            } else if (!this.comment.sessionGuid) {
                // The sessionlog is a bot comment
                return this.comment.sender;
            } else {
                // The user is a facilitator and the message is from a participant
                const userSession = this.$store.state.user.sessions.filter(
                    (s) => s.sessionGuid === this.comment.sessionGuid
                )[0];
                return `${this.comment.sender} (${
                    userSession ? userSession.participant : ''
                })`;
            }
        }
    }
</script>

<style lang="scss">
    @import '../assets/scss/spacings.scss';
    @import '../assets/scss/colors.scss';
    @import '../assets/scss/typography.scss';


    .scroll-y {
        max-height: 25vh;
        @media screen and (min-height:620px) {
            max-height: 25vh;
        }
        overflow-y: scroll;
        overflow-x: hidden;
    }

    .comments {
        
        @media print {
            border-top: 0;
        }

        margin-top: $space-sm;
        padding-top: $space-sm;
        border-top: 1px solid #ccc;

        .comment {
            margin-bottom: 15px;
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            
            .children {
                margin-top: 18px;
                width: 100%;
            }

            .avatar {
                margin-right: 10px;
                width: 50px;
                height: 50px;
                flex-basis: 0;
            }

            p.contents {
                flex-basis: 0;
                flex-grow: 1;
                background-color: $primary-button-foreground-color;
                border-radius: 30px;
                padding: 10px 20px;
                page-break-after: always;
                display: flex;
                flex-wrap: wrap;
                align-items: flex-start;

                .comment-text {
                    overflow-wrap: break-word;
                    white-space: pre-wrap;
                    display: inline-block;
                }

                .comment-name {
                    font-weight: bold;
                    display: inline-block;
                    margin-right: 10px;
                }
            }

            p.comment-link {
                margin-left: 75px;
                margin-top: 10px;
                a {
                    @include typography(regular, 12px, $dark-blue1);
                    letter-spacing: 1px;
                    cursor: pointer;
                }
            }
            
            p.break {
                height: 0;
                flex-basis: 100%;
            }
        }
    }
</style>
