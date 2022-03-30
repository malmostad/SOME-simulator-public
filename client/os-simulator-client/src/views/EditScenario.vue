<template>
    <div class="editscenario-view view" v-cloak>
        <ConfirmationComponent/>
        <div v-if="this.$store.state.user.editScenario">
            <transition name="component-fade" mode="out-in">
                <div>
                    <h1 class="big">Redigera
                        <span class="sub-heading">| <br>Scenario: {{this.$store.state.user.editScenario.name}}</span>
                    </h1>
                    <div class="owner-line">
                        <p>SoMe simulator</p>
                    </div>
                    <hr/>
                    <div class="col-container">
                        
                        <nav v-if="loaded">
                            <ul>
                                <li>
                                    <a v-bind:class="{ active: this.viewType === 'event-listing' }" 
                                       @click.prevent="setEditType('event-listing')">Events</a>
                                </li>
                                <li>
                                    <a v-bind:class="{ active: this.viewType === 'post-listing' }" 
                                       @click.prevent="setEditType('post-listing')">Posts</a>
                                </li>
                                <li>
                                    <a v-bind:class="{ active: this.viewType === 'comment-listing' }"
                                       @click.prevent="setEditType('comment-listing')">Kommentarer</a>
                                </li>
                            </ul>
                        </nav>
                        
                        <div class="listing-area">
                            <transition name="component-fade" mode="out-in">
                                <component v-bind:is="this.viewType"></component>
                            </transition>
                        </div>
                    </div>
                </div>
            </transition>
        </div>
    </div>
</template>

<script lang="ts">
    import {mixins} from "vue-class-component";
    import FacilitatorMixin from '../mixins/FacilitatorMixin';
    import ScenarioListing from '../components/ScenarioListing.vue';
    import ScenarioEventListing from '../components/ScenarioEventListing.vue';
    import PostListing from '../components/PostListing.vue';
    import CommentListing from '../components/CommentListing.vue';
    import { User } from '../Types/User';
    import {Scenario} from '@/Types/Scenario';
    import {Session} from '../Types/Session';
    import {mapState} from 'vuex';
    import {Component} from "vue-property-decorator";
    import ConfirmationComponent from "@/components/ConfirmationComponent.vue";
    
    @Component({
        computed: mapState(['user']),
        
        components: {
            ScenarioListing,
            EventListing: ScenarioEventListing,
            PostListing,
            CommentListing,
            ConfirmationComponent
        }
    })
    export default class EditScenario extends mixins(FacilitatorMixin) {
        private $hub: any;
        private hubStartPromise: any;
        protected user!: User;
        public scenarioId = '0';
        public messageToComment: string = 'sdfsdfsdfsdfsd';
        public viewType: string = 'event-listing';
        public loaded: boolean = false;

        get sessions(): Session[] {
            return [...this.$store.state.user.sessions];
        }
        public setEditType(viewType){
            this.viewType = viewType;
        }

        public created() {
            this.loaded = false;
            
            this.scenarioId = this.$route.params.scenarioId != null ? this.$route.params.scenarioId : this.$store.state.user.editScenarioId;
            this.$store.dispatch('setEditScenario', this.scenarioId);
            this.$store.dispatch('loadEditScenario',this.scenarioId).then(() => {
                this.loaded = true;
            });
            
            this.$hub.onclose(() => {
                this.hubStartPromise.then(() => {
                    this.setupHub();
                });
            });
        }
    }
</script>

<style lang="scss" scoped>
    @import "../assets/scss/style";
    @import '../assets/scss/transitions';
    hr{
        margin-top: 60px;
    }
    .owner-line{
        position: absolute;
        right: 30px;
        top: 10px;
    }
    .col-container {
        margin-top: 60px;
        display: flex;
        .listing-area {
            width: 75%;
        }
        
        nav{
            width:25%;
            ul{
                li{
                    list-style: none;
                    a{
                        display: inline-block;
                        color: #000000;
                        text-decoration: none;
                        font-weight: bold;
                        position: relative;
                        cursor: pointer;
                        padding: 12px $space-lg $space-sm 0;
                        &:after{
                            content: "";
                            position: absolute;
                            top: 16px;
                            right: 0;
                            display: block;
                            width: 11px;
                            height: 18px;
                            transform: rotate(90deg);
                            background-repeat: no-repeat;
                            background-image: url('../assets/icons/marker.svg');
                            opacity: 0;
                            transition: all .3s ease-in-out;
                        }
                        &.active{
                            &:after{
                                right: -5px;
                                opacity: 1;
                            }
                        }
                    }
                }
            }
        }
    }
    
</style>