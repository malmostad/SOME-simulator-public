<template>
    <section>
        <div v-if="postCopy">
            <h1>Redigera post</h1>
            
        </div>
        <div v-else>
            <h1>Skapa post</h1>
        </div>
        
        <div class="edit-form">
            
            <div class="input-area"><input type="text" v-model="postCopy.sender" placeholder="Avsändare*" /></div>
            <div class="input-area"><textarea class="large"  v-model="postCopy.text" placeholder="Text*"></textarea></div>

            <div class="checkbox-area">
                <b>Fas (Du kan välja flera faser)*</b>
                
                <label v-for="phase in  user.editScenario.phases">
                    <span>{{phase.description}}</span>
                    <input type="checkbox" v-model="postCopy.phases" :value="phase.id" />
                </label>
            </div>
            <div class="checkbox-area">
                <b>Välj meddelandetyp.</b>

                <label>
                    <span>Kvitter</span>
                    <input type="checkbox" :checked="(postCopy.messageFlow & MessageFlow.Short) == MessageFlow.Short" @click="changeMessageFlow(MessageFlow.Short)" />
                </label>

                <label>
                    <span>Friendzone</span>
                    <input type="checkbox" :checked="(postCopy.messageFlow & MessageFlow.Long) == MessageFlow.Long" @click="changeMessageFlow(MessageFlow.Long)" />
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
    import EditPost from "@/Types/Admin/EditPost";
    import Validator from "@/helpers/Validator";


    @Component({
        computed: {...mapState(['user']) , MessageFlow : () => MessageFlow},
        components: {

        }
    })
    export default class EditCreatePostComponent extends Vue {
        
        @Prop({required: false, default: null})
        public post: any;
        
        public postCopy: EditPost = new EditPost();


        public eventId = '0';
        protected user!: User;
        private canSubmit = true;
        private formError: boolean = false;

        public created() {
            if(this.post) {
                this.postCopy = {...this.post};
            }
        }
        
        
        
        public ValidateForm(post:EditPost) {
            return Validator.Enum(post.messageFlow) 
                && Validator.Array(post.phases) 
                && Validator.String( post.text) 
                && Validator.String(post.sender);
        }
        
        public save() {
            
            let result = this.ValidateForm(this.postCopy);
            
            if(!result) {
                this.formError = true;
                return;
            }
            
            this.canSubmit = false;

            if(this.postCopy.id == undefined) {
                this.$store.dispatch('createPost', this.postCopy).then(() => {
                    this.close();
                }); 
            }
            else {
                this.$store.dispatch('updatePost', this.postCopy).then(() => {
                    this.close();
                });    
            }
        }
        
        public changeMessageFlow(val: MessageFlow) {
            if((this.postCopy.messageFlow & val) === val) {
                this.postCopy.messageFlow -= val;
            }
            else {
                this.postCopy.messageFlow += val;
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