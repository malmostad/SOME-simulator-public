<template>
    <div class="container">
        <div v-if="this.$store.state.user.editScenario && !this.editCreate">
            <h1>Events</h1>
            <p class="intro">Här nedan kan du redigera eller ta bort de olika eventsen. Du har även möjlighet att lägga till nya event i valfri fas.</p>
            <button @click="createEvent" class="add-evt">
                Lägg till nytt event
            </button>
            
            <div class="phase" v-for="(phase, index) in this.$store.state.user.editScenario.phases">
                <h2 class="sub-heading">Fas {{index + 1}}</h2>
                <hr/>
                <div v-for="scenarioEvent in phase.scenarioEvents">
                    <header class="edit-header">
                        <a @click="editEvent(scenarioEvent)">Redigera</a>
                        <a @click="deleteEventModal(scenarioEvent)">Ta bort</a>
                    </header>
                    <article class="evt">
                    <h2>
                        <span class="sub-heading">{{scenarioEvent.sender}}</span>
                        {{scenarioEvent.heading}}
                    </h2>
                    <footer>
                        <button class="tertiary" @click="() => {dialog = scenarioEvent}">Läs mer</button>
                    </footer>
                    </article>
                </div>
            </div>
        </div>
        <EditScenarioEvent v-else-if="this.editCreate" v-bind:scenario-event="this.scenarioEventItem" @editCreateClosed="closeEditCreate"/>
        
        <DialogComponent 
            :open="dialog != null" 
            :enable-close="true"
            @modalClosed="() => {dialog  = null}">
            <div class="transparent-dialog" v-if="dialog">
                <p class="sender" v-text="dialog.sender"></p>
                <h1 class="heading" v-text="dialog.heading"></h1>
                <p class="content" v-html="dialog.text"></p>
            </div>
        </DialogComponent>
    </div>
</template>

<script lang="ts">
    import {Component, Vue} from "vue-property-decorator";
    import {mapState} from "vuex";
    import {User} from "@/Types/User";
    import axios, {AxiosResponse} from "axios";
    import ConfirmationComponent from "@/components/ConfirmationComponent.vue";

    import EditCreateScenarioEventComponent from "@/components/EditCreateScenarioEventComponent.vue";
    import EditScenarioEvent from "@/Types/Admin/EditScenarioEvent";
    import DialogComponent from "@/components/DialogComponent.vue";
    
    @Component({
        computed: mapState(['user']),
        components: {
            DialogComponent,
            ConfirmationComponent,
            EditScenarioEvent: EditCreateScenarioEventComponent
        }
    })
    export default class ScenarioEventListing extends Vue {
        public scenarioId = '0';
        protected user!: User;
        public editCreate: boolean = false;
        public scenarioEventItem: EditScenarioEvent = new EditScenarioEvent();
        private unsubscribe: any;
        private dialog: EditScenarioEvent|null = null;
        
        public created() {
            this.scenarioId = this.$route.params.scenarioId != null ? this.$route.params.scenarioId : this.$store.state.user.editScenarioId;
            this.unsubscribe = this.$store.subscribeAction({
                before: (action, state) => {
                    // no action
                },
                after: (action, state) => {
                    switch (action.type) {
                        case 'closeConfirmAlert': {
                            this.deleteScenarioEvent();
                            break;
                        }
                        case 'closeAlert': {
                            this.closeEditCreate();
                            break;
                        }
                    }
                },
            });
        }
        
        public beforeDestroy() {
            this.unsubscribe();
        }
        
        public deleteEventModal(scenarioEvent): void{
            
            this.scenarioEventItem = scenarioEvent;
            this.$store.dispatch('showAlert', {
                title: 'Är du säker på att du vill ta bort eventet?',
            });
        }
        
        public editEvent(scenarioEvent): void{
            this.scenarioEventItem = scenarioEvent;
            this.editCreate = true;
        }
        
        public createEvent():void{
            this.scenarioEventItem = new EditScenarioEvent();
            this.editCreate = true;
        }
        
        public closeEditCreate():void{
            this.editCreate = false;
        }
        
        public deleteScenarioEvent(){
            this.$store.dispatch('deleteScenarioEvent',this.scenarioEventItem );
            
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
    }
    
    .phase{
        margin-top: 120px;
        .sub-heading{
            color: #000000;
        }
        hr{
            margin-top: $space-sm;
        }
        .evt{
            
            background-color: $gray5;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            
            h2{
                padding: $space-xxl $space-xl;
                .sub-heading{
                    display: block;
                    @include typography(regular, 16, $default-text-color);
                    text-transform: uppercase;
                }
            }
            footer{
                display: flex;
                width: 100%;
                background-color: $white1;
                align-items: center;
                justify-content: center;
                height: 65px;
            }
        }
    }
</style>