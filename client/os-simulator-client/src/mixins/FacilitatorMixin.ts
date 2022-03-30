import {Component, Vue, Watch} from 'vue-property-decorator';
import { mapState } from 'vuex';
import { Session } from '../Types/Session';
import { User } from '../Types/User';
import ScenarioEvent from '../Types/ScenarioEvent';
import Phase from '../Types/Phase';
import {Route} from "vue-router";
import State from "@/Types/State";

@Component({ computed: mapState(['user']) })
export default class FacilitatorMixin extends Vue {
    protected user!: User;

    protected scenario: any = {
        Name: '',
        Description: '',
    };

    //protected sessionId: number | null = null;

    protected scenarioEvents: ScenarioEvent[] = [
        {heading: '', progressPoint: 0},
    ];
    protected phases: Phase[] = [{start: 0, end: 0, heading: ''}];

    protected setupHub() {
    }

    public created() {
        if(!this.$store.getters.isFaciliator && !this.$store.getters.isAdmin && !this.$store.state.signedIn)
            this.$router.push('signin');
        
        if(this.$store.getters.hasSessionGroupId && this.$store.state.signedIn) {
            this.loadSessions().finally(() => {
                this.loadScenario();
                this.loadActivity();
            });    
        }
        if(this.$store.state.signedIn)
        this.loadScenarios();
    }

    private loadScenarios() {
        console.log("load scenarios")
        this.$store.dispatch('loadScenarios');
    }

    public loadSessions() {
        return this.$store
            .dispatch('loadSessions')
            .then(() => {
                const sessions: Session[] = this.$store.state.user
                    .sessions;
                this.setupHub();
            })

            .catch((error) => {
                console.error(error);
                console.trace();
                if (
                    error.response.status === 404 ||
                    error.response.status === 401
                ) {
                    console.log(
                        'Clearing SessionGroupId because it has been removed.'
                    );
                    this.clearSessionGroupData();
                }
            });
    }

    public loadScenario() {
        this.$store
            .dispatch('loadScenario')

            .catch((error) => {
                console.error(error);
                console.trace();
                console.log(
                    'Clearing SessionGroupId because it has been removed.'
                );
                this.clearSessionGroupData();
            });
    }

    protected loadActivity() {

        this.$store
            .dispatch('loadActivity', this.user.sessionGroupId)
            .catch((error) => {
                console.error(error);
                console.trace();
                if (
                    error == 'Missing sessionGroupId' ||
                    error.response.status == 404
                ) {
                    console.log(
                        'Clearing SessionGroupId because it has been removed.'
                    );
                    this.clearSessionGroupData();
                }
            });
    }

    protected clearSessionGroupData() {
        this.$store.dispatch('setSessionGroupId', '');
    }

}