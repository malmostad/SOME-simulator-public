<template>
    <div class="container">
        <div v-if="this.$store.state.user.editScenario && !this.editCreate">
            <h1>Poster</h1>
            <p class="intro">Här nedan kan du redigera eller ta bort de olika postningarna. Du har även möjlighet att lägga till nya posts i en eller flera faser.</p>
            <button @click="createPost" class="add-evt">
                Lägg till ny post
            </button>
            <hr/>
            <search-filter @change="filter">
            </search-filter>
            
            
            <div v-for="(post, index) in filteredPosts" class="post-container">
            
                    <header class="edit-header">
                        <a @click="editEvent(post)">Redigera</a>
                        <a @click="deletePostModal(post)">Ta bort</a>
                    </header>
                    <article class="post">
                        <div class="sender">
                            <img v-bind:src="post.avatar" alt="">
                            <h3>{{post.sender}}</h3>
                        </div>
                        <p>{{post.text}}</p>
                    </article>
            </div>
        </div>
        <EditPost v-else-if="this.editCreate" v-bind:post="this.postItem" @editCreateClosed="closeEditCreate"/>
    </div>
</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import {mapState} from "vuex";
    import {User} from "@/Types/User";
    import axios, {AxiosResponse} from "axios";
    import ConfirmationComponent from "@/components/ConfirmationComponent.vue";
    import EditCreatePostComponent from "@/components/EditCreatePostComponent.vue";
    import EditSearchFilterComponent from "@/components/SearchFilter.vue";
    import {EditSearchFilterData, EditSearchFilterService} from "@/services/EditSearchFilterService";
    import Post from "@/Types/Admin/EditPost";

    @Component({
        computed: mapState(['user']),
        components: {
            ConfirmationComponent,
            EditPost: EditCreatePostComponent,
            SearchFilter: EditSearchFilterComponent,
        }
    })
    export default class PostListing extends Vue {

        protected user!: User;

        public postItem:Post|null = null;
        private unsubscribe: any;
        public scenarioId = '0';
        public editCreate: boolean = false;
        
        public created() {
            this.scenarioId = this.$route.params.scenarioId != null ? this.$route.params.scenarioId : this.$store.state.user.editScenarioId;
            this.unsubscribe = this.$store.subscribeAction({
                before: (action, state) => {
                    // no action
                },
                after: (action, state) => {
                    switch (action.type) {
                        case 'closeConfirmAlert': {
                            this.deletePost();
                            break;
                        }
                        case 'closeAlert': {
                            this.closeEditCreate();
                            break;
                        }
                        case 'deletePost': {
                            this.filter(this.editSearchFilterData);
                            break;
                        }
                        case 'updatePost': {
                            this.filter(this.editSearchFilterData);
                            break;
                        }
                        case 'createPost': {
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
        
        public listFilter: EditSearchFilterService = new EditSearchFilterService();
        
        private editSearchFilterData:EditSearchFilterData = new EditSearchFilterData();
        
        public filter(editSearchFilterData:EditSearchFilterData|null){
            if(editSearchFilterData == null)
                editSearchFilterData = this.editSearchFilterData;
            else
                this.editSearchFilterData = editSearchFilterData;
            
            this.filteredPosts =  [...this.listFilter.filter(editSearchFilterData, this.$store.state.user.editScenario.posts)];           
        }
        public filteredPosts = [...this.$store.state.user.editScenario.posts];

        public editEvent(post): void{
            this.postItem = post;
            this.editCreate = true;
        }
        public createPost():void{
            this.postItem = new Post();
            this.editCreate = true;
        }
        public closeEditCreate():void{
            console.log("Closing");
            this.postItem = null;
            this.editCreate = false;
        }
        public deletePostModal(post): void{
            this.postItem = post;
            this.$store.dispatch('showAlert', {
                title: 'Är du säker på att du vill ta bort posten?',
            });
        }

        public deletePost(){
            if(this.postItem != null){
                this.$store.dispatch('deletePost', this.postItem);
            }
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
        margin-bottom: $space-lg;
        width: auto;
        min-width: 200px;
    }

    .post-container{
        margin-top: $space-xl;
        .post{
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
    }
    
</style>