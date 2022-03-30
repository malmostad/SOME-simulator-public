<template>
    <div class="comments" v-if="comments.length > 0" v-bind:class="{'scroll-y':commentsScroll}">
        
        <div v-for="(comment, index) in comments"
             v-bind:key="index">
            <comment :comment="comment">
            </comment>
        </div>
    </div>
    
</template>

<script lang="ts">
    import {Component, Emit, Prop, Vue} from 'vue-property-decorator';
import { SessionLog } from '@/Types/SessionLog';
import AvatarComponent from "@/components/AvatarComponent.vue";
import Comment from '@/components/Comment.vue';

@Component({
    components: {AvatarComponent, Comment}
})
export default class Comments extends Vue {
    @Prop({ default: false })
    private commentsScroll!: boolean;

    @Prop({ default: '' })
    private sessionId!: string;

    @Prop({ default: Object as () => SessionLog[] })
    private comments!: SessionLog[];
    
    
    private openCommentDialog(sessionLog: SessionLog) {
    
        //ToDo: SKicka vidare genom store till lämpligt ställe att hantera redigeringen.
        
    }
    
    private get sortedComments(): SessionLog[] {
        return this.comments.sort((s1, s2) => {
            return s1.sendDateTime > s2.sendDateTime ? 1 : -1;
        });
    }

    public scroll() {
        const el = this.$el;
        el.scrollTop = el.scrollHeight;
    }

    private mounted() {
        this.scroll();
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

    .children {
        padding-left: 67px;
    }
}
</style>
