<template>
    <section>
        <div v-if="commentCopy">
            <h1>Redigera kommentar</h1>

        </div>
        <div v-else>
            <h1>Skapa kommentar</h1>
        </div>

        <div class="edit-form">

            <div class="input-area"><input type="text" v-model="commentCopy.sender" placeholder="Avsändare*"/></div>
            <div class="input-area"><textarea class="large" v-model="commentCopy.text" placeholder="Text*"></textarea></div>

            <div class="checkbox-area">
                <b>Fas (Du kan välja flera faser)*</b>

                <label v-for="phase in  user.editScenario.phases">
                    <span>{{phase.description}}</span>
                    <input type="checkbox" v-model="commentCopy.phases" :value="phase.id" />
                </label>
            </div>
            <div class="checkbox-area">
            <b>Välj meddelandetyp.</b>

            <label>
                <span>Kvitter</span>
                <input type="checkbox" :checked="(commentCopy.messageFlow & MessageFlow.Short) == MessageFlow.Short" @click="changeMessageFlow(MessageFlow.Short)" />
            </label>

            <label>
                <span>Friendzone</span>
                <input type="checkbox" :checked="(commentCopy.messageFlow & MessageFlow.Long) == MessageFlow.Long" @click="changeMessageFlow(MessageFlow.Long)" />
            </label>
        </div>

            <div class="checkbox-area">
                <b>Välj meddelandetyp.</b>

                <label>
                    <span>Positiv</span>
                    <input type="checkbox" :checked="(commentCopy.props & BotReplyProperties.Positive) == BotReplyProperties.Positive" @click="changeBotReplyProperty(BotReplyProperties.Positive)" />
                </label>
                <label>
                    <span>Negativ</span>
                    <input type="checkbox" :checked="(commentCopy.props & BotReplyProperties.Negative) == BotReplyProperties.Negative" @click="changeBotReplyProperty(BotReplyProperties.Negative)" />
                </label>
                <label>
                    <span>Neutral</span>
                    <input type="checkbox" :checked="(commentCopy.props & BotReplyProperties.Neutral) == BotReplyProperties.Neutral" @click="changeBotReplyProperty(BotReplyProperties.Neutral)" />
                </label>
            </div>
            <transition name="component-fade" mode="out-in">
                <div v-if="formError" class="error">
                    Alla fällt i formuläret måste anges för att kunna spara.
                </div>
            </transition>
            <div>
                <button class="btn" @click="save">Spara</button>
                <button class="btn secondary" @click="close">Stäng</button>
            </div>
        </div>


    </section>

</template>

<script lang="ts">
    import {Component, Prop, Vue} from "vue-property-decorator";
    import {mapState} from "vuex";
    import {User} from "@/Types/User";
    import {MessageFlow} from "@/Types/MessageFlow";
    import {BotReplyProperties} from "@/Types/BotReplyProperties";
    import EditComment from "@/Types/Admin/EditComment";
    import Validator from "@/helpers/Validator";

    @Component({
        computed: {...mapState(['user']) , MessageFlow : () => MessageFlow, BotReplyProperties: () => BotReplyProperties},
        components: {
        }
    })
    export default class EditCreateCommentComponent extends Vue {
        @Prop({required: false, default: null})  public comment?: EditComment;
        public commentCopy: EditComment = new EditComment(0);
        
        public eventId = '0';
        protected user!: User;
        private canSubmit = true;
        private formError: boolean = false;
            
        public created() {
            if(this.comment)
            {
                this.commentCopy = {...this.comment};
            }
        }
        
        public ValidateForm(comment:EditComment) {
            return Validator.Enum(comment.messageFlow)
                && Validator.Enum(comment.props)
                && Validator.Array(comment.phases)
                && Validator.String( comment.text)
                && Validator.String(comment.sender);
        }

        public save() {

            let result = this.ValidateForm(this.commentCopy);

            if(!result) {
                this.formError = true;
                return;
            }
            this.canSubmit = false;
            console.log(this.commentCopy)
            if(this.commentCopy.id == undefined) {
                this.$store.dispatch('createComment', this.commentCopy).then(() => {
                    this.close();
                });
            }
            else {
                this.$store.dispatch('updateComment', this.commentCopy).then(() => {
                    this.close();
                });
            }
        }

        public changeMessageFlow(val: MessageFlow) {
            if((this.commentCopy.messageFlow & val) === val) {
                this.commentCopy.messageFlow -= val;
            }
            else {
                this.commentCopy.messageFlow += val;
            }
        }
        public changeBotReplyProperty(val: BotReplyProperties) {
            
            if((this.commentCopy.props & val) === val) {
                this.commentCopy.props -= val;
            }
            else {
                this.commentCopy.props += val;
            }
        }

        public close(){
            this.$emit('editCreateClosed');
        }
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/transitions';
    @import '../assets/scss/editform';
    
    
</style>