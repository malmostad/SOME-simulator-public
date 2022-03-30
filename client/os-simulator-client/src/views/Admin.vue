<template>
    <div class="facilitator-view view" v-cloak>
        <div>
            <div>
                <h1 class="big">Redigera
                    <span class="sub-heading">| <br>Välj scenario</span>
                </h1>
                <hr/>
                <div class="scenario-listing">
                    <div
                        v-for="(scenario, index) in user.scenarios"
                        v-bind:key="scenario.id"
                        class="list-item"
                    >
                        <scenario-listing
                            :scenario="scenario"
                            :numbering="index + 1"
                        />
                    </div>
                </div>
                
                <router-link v-if="$store.getters.isFaciliator" class="nav-link" to="facilitator">Övningsverktyg</router-link>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import ScenarioListing from '../components/ScenarioListing.vue';
import { User } from '@/Types/User';
import {Session} from '@/Types/Session';
import {mapState} from 'vuex';
import {Component, Vue} from "vue-property-decorator";



@Component({
    computed: mapState(['user']),
    components: {
        ScenarioListing
    },
})
export default class Admin extends Vue {
    private $hub: any;
    private hubStartPromise: any;
    protected user!: User;

    get sessions(): Session[] {
        return [...this.$store.state.user.sessions];
    }

    public created() {
        
        if(!this.$store.getters.isAdmin) {
           this.$router.push('signin');
        }
        
        this.$store.dispatch('loadScenarios');
        
        /*this.$hub.onclose(() => {
            this.hubStartPromise.then(() => {
                this.setupHub();
            });
        });*/
    }
}
</script>

<style  lang="scss" scoped>
    @import "../assets/scss/style";
    
    
    .big{
        padding: $space-lg 0;
    }
    .scenario-listing{
        display: flex;
        margin-top: $space-lg;
        margin-left: -15px;
        margin-right: -15px;
        .list-item{
            display: flex;
            width: 50%;
            .description{
                display: flex;
            }
            .edit{
                cursor: pointer;
            }
        }
    }
</style>