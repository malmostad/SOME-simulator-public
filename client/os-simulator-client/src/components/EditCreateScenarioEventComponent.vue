<template>
    <section>
        <div v-if="scenarioEventCopy.id">    
            <h1>Redigera event</h1>
            
        </div>
        <div v-else>
            <h1>Skapa event</h1>
        </div>

        <div class="edit-form">
            
            <div class="input-area">
                <input type="text" v-model="scenarioEventCopy.sender" placeholder="Avsändare*"/>
            </div>
            <div class="input-area">
                <input type="text" v-model="scenarioEventCopy.heading" placeholder="Rubrik*"/>
            </div>
            
            <div class="input-area">
                <textarea class="large" v-model="formattedText" placeholder="Text*"></textarea>
            </div>
            <div>
                <select v-model="scenarioEventCopy.phaseId" placeholder="Avsändare">
                    <option :value="null" disabled hidden>Fas*</option>
                    <option v-for="p in user.editScenario.phases" :value="p.id">{{p.description}}</option>
                </select>
            </div>
            <transition name="component-fade" mode="out-in">
            <div class="slider-container"  v-if="selectedPhase">
                <div>
                    <b>Välj position.</b> Vart i {{selectedPhase.description}} vill du att eventet ska ligga? Justera slidern till där du tycker den ska in i {{selectedPhase.description}}. De blåa prickarna motsvarar de andra eventen.
                </div>
                <div>
                    <div>
                        <div class="phase-descrition">
                            {{selectedPhase.description}}
                        </div>
                        <phase-event-slider :class="'phase-event-slider'" :initial-value="scenarioEventCopy.timePercent" :phase="selectedPhase" @change="eventSliderChange">
                        </phase-event-slider>
                    </div>
                    
                </div>
            </div>
            </transition>


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
    import axios, {AxiosResponse} from "axios";
    import Validator from "@/helpers/Validator";

    import PhaseEventSlider from "@/components/PhaseEventSlider.vue";
    import EditPhase from "@/Types/Admin/EditPhase";
    import {Computed} from "vuex/types/helpers";
    import EditScenarioEvent from "@/Types/Admin/EditScenarioEvent";
    import ScenarioEvent from "@/Types/ScenarioEvent";


    @Component({
        computed: mapState(['user']),
        components: {
            PhaseEventSlider
        }
    })
    export default class EditCreateScenarioEventComponent extends Vue {
        @Prop({required: true}) public scenarioEvent!: EditScenarioEvent;
        public scenarioEventCopy: EditScenarioEvent = new EditScenarioEvent();
        
        protected user!: User;
        private formError: boolean = false;
        private formattedText?: string;
        private canSubmit: boolean = true;
        
        public created() {
            if(this.scenarioEvent)
                this.scenarioEventCopy =  {...this.scenarioEvent};
            
            this.formattedText = this.scenarioEventCopy.text.replace(/<br[ ]*\/>/gi , '\n');
        }
        

        public ValidateForm(event:EditScenarioEvent) {
            return Validator.String(event.sender)
                && Validator.String(event.heading)
                && Validator.String(this.formattedText || '')
                && Validator.WithinRange(event.timePercent, 0, 1);
        }
        
        public close(){
            this.$emit('editCreateClosed');
        }
        public save(){
            
            let result = this.ValidateForm(this.scenarioEventCopy);
            
            if(!result) {
                this.formError = true;
                return;
            }

            this.canSubmit = false;
            
            if(this.formattedText) {
                this.scenarioEventCopy.text = this.formattedText.replace(/\n/gi,'<br/>');
            }

            if(this.scenarioEventCopy.id == undefined) {
                this.$store.dispatch('createScenarioEvent', this.scenarioEventCopy).then(() => {
                    this.close();
                });
            }
            else {
                this.$store.dispatch('updateScenarioEvent', this.scenarioEventCopy).then(() => {
                    this.close();
                });
            }
            
        }
        
        public eventSliderChange(payload) {
            console.log(payload);
            
            this.scenarioEventCopy.timePercent = payload;   
        }
        
        public get selectedPhase():EditPhase|null {
            let result = this.$store.state.user.editScenario.phases.filter(p => p.id == this.scenarioEventCopy.phaseId);
            if(Array.isArray(result) && result.length == 1){
                return result[0];
            }

            return null;
        }
    }
</script>

<style lang="scss" scoped>

@import '../assets/scss/transitions';
@import '../assets/scss/editform';

    .slider-container {
        div {
            div {
                margin-top: 5px;
            }    
        }
        
    }
    .slider-container {
        
        display: flex;
        
        div:first-child {
            flex-grow: 0;
            
        }
        div + div {
            flex-grow: 1;
            min-width: 60%;
            margin-left: 35px;
        }

        .phase-descrition {
            margin-left: 28px;
            margin-bottom: 10px;
            padding: 0;
        }
    }
    
</style>