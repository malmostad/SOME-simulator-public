<template>
    <div class="container">
        <div v-if="this.$store.state.user.editScenario && !this.editCreate">
            <h1>Kommentarer</h1>
            <p class="intro">Här nedan kan du redigera eller ta bort de olika kommentarerna. Du har även möjlighet att lägga till nya kommentarer i en eller flera faser.</p>

            <button class="add-evt" @click="createComment">
                Lägg till ny kommentar
            </button>

            <hr/>
            <search-filter @change="filter">
            </search-filter>

            <div v-for="(comment, index) in filteredComments">
                <header class="edit-header">
                    <a @click="editComment(comment)">Redigera</a>
                    <a @click="deleteCommentModal(comment)">Ta bort</a>
                </header>
                <article class="comment" >
                    <div class="sender">
                        <img v-bind:src="comment.avatar" alt="">
                        <h3>{{comment.sender}}</h3>
                    </div>
                    <p>{{comment.text}}</p>
                </article>
            </div>
        </div>
        <EditComment v-else-if="this.editCreate" v-bind:comment-copy="this.commentItem" @editCreateClosed="closeEditCreate"/>
    </div>
</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import {mapState} from "vuex";
    import {User} from "@/Types/User";
    import {AxiosResponse} from "axios";
    import {EditSearchFilterData, EditSearchFilterService} from "@/services/EditSearchFilterService";
    import EditSearchFilterComponent from "@/components/SearchFilter.vue";
    import EditCreateCommentComponent from "@/components/EditCreateCommentComponent.vue";
    import EditComment from "@/Types/Admin/EditComment";

    @Component({
        computed: mapState(['user']),
        components:{
            EditComment:EditCreateCommentComponent,
            SearchFilter: EditSearchFilterComponent
        } 
        
    })
    export default class CommentListing extends Vue {
        public scenarioId = '0';
        protected user!: User;
        private commentItem?: EditComment | null;
        private editCreate: boolean = false;
        private unsubscribe: any;

        public listFilter: EditSearchFilterService = new EditSearchFilterService();

        private editSearchFilterData:EditSearchFilterData = new EditSearchFilterData();

        public filter(editSearchFilterData:EditSearchFilterData|null){
            if(editSearchFilterData == null)
                editSearchFilterData = this.editSearchFilterData;
            else
                this.editSearchFilterData = editSearchFilterData;

            this.filteredComments =  [...this.listFilter.filter(editSearchFilterData, this.$store.state.user.editScenario.comments)];
        }
             
        public filteredComments = [...this.$store.state.user.editScenario.comments];


        public editComment(comment): void{
            this.commentItem = comment;
            this.editCreate = true;
        }
        public createComment():void{
            if(this.user.editScenarioId != null){
                this.commentItem = new EditComment(this.user.editScenarioId);
                this.editCreate = true;
            }
        }
        public closeEditCreate():void{
            console.log("Closing");
            this.commentItem = null;
            this.editCreate = false;
        }
        public deleteCommentModal(comment): void{
            this.commentItem = comment;
            this.$store.dispatch('showAlert', {
                title: 'Är du säker på att du vill ta bort kommentaren?',
            });
        }

        public deleteComment(){
            if(this.commentItem != null){
                this.$store.dispatch('deleteComment', this.commentItem);
            }
        }


        public created() {
            this.scenarioId = this.$route.params.scenarioId != null ? this.$route.params.scenarioId : this.$store.state.user.editScenarioId;
            this.unsubscribe = this.$store.subscribeAction({
                before: (action, state) => {
                    // no action
                },
                after: (action, state) => {
                    switch (action.type) {
                        case 'closeConfirmAlert': {
                            this.deleteComment();
                            break;
                        }
                        case 'closeAlert': {
                            this.closeEditCreate();
                            break;
                        }
                        case 'deleteComment': {
                            this.filter(this.editSearchFilterData);
                            break;
                        }
                        case 'updateComment': {
                            this.filter(this.editSearchFilterData);
                            break;
                        }
                        case 'createComment': {
                            this.filter(this.editSearchFilterData);
                            break;
                        }
                    }
                },
            });
        }

        public beforeDestroy() {
            this.unsubscribe();
        }
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';
    @import '../assets/scss/typography';
    .container{
        margin-bottom: $space-xl;
    }
    .intro{
        max-width: 850px;
    }
    .add-evt{
        margin-top: $space-lg;
        width: auto;
        min-width: 200px;
        margin-bottom: $space-lg;    
    }


    .comment{
        margin-top: $space-xl;
        background-color: $gray5;
        display: flex;
        flex-direction: column;
        align-items: start;
        justify-content: center;
        padding: 30px;
        border-radius: 5px;
        .sender{
            display: flex;
            img{
                margin-right: $space-sm;
            }
            h3{
                @include typography(medium, 16, $dark-blue1);
                margin-top: 5px;
            }
        }
        p{
            margin-top: $space-sm;
        }
    }

</style>