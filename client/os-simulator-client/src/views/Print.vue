<template>
    <div class="view">
        <div class="flex">
            <div class="flex-col-full-width">
                <h1>Utskrift</h1>
                <h2>Sessionsnamn: {{ user.groupName }}</h2>
            </div>

            <div class="buttons hide-print flex-col-full-width text-right">
                <button @click="print">Skriv ut</button>
                <router-link :to="{ name: 'facilitator' }">
                    <button class="secondary button">Tillbaka</button>
                </router-link>
            </div>
        </div>
        
        <div class="show-print-only">
             
            <table v-if="sum().length > 0">
                <tr v-for="row in sum()">
                    <td>{{row.participant}}</td>
                    <td><img src="../../src/assets/icons/heart.svg"/> Empati {{ row.emotional }}
                    </td>
                    <td><img src="../../src/assets/icons/speaker.svg"/> Tonalitet {{  row.tone }}
                    </td>
                    <td><img src="../../src/assets/icons/document.svg"/> Faktamässighet {{ row.facts }}
                    </td>
                </tr>
            </table>
            
        </div>
        
        
        <div class="hide-print">
            <h3 class="bold-label">Filtrera</h3>
            
            <div class="participant-filer">
                <ParticipantFilterList
                    :sessions="user.sessions"
                    @change="sessionFilter"
                    ref="participantFilter"
                >
                </ParticipantFilterList>
            </div>
            
            <div class="tag-filter">
                <TagFilterList @change="setTags" ref="tagFilter"></TagFilterList>
            </div>
            
            <div class="selected-filters">
                <span class="bold-label">Valt filter: </span>
                <span v-for="session in selectedSessions" class="selected-user">
                    <span v-bind:class="classObject(session)" class="blob"></span>
                    <span>{{session.participant}}</span>
                </span>
                
                <span v-if="selectedTags & tags.emotional"> <img src="../../src/assets/icons/heart.svg"/> </span>

                <span v-if="selectedTags & tags.tone"> <img src="../../src/assets/icons/speaker.svg"/> </span>

                <span v-if="selectedTags & tags.facts"> <img src="../../src/assets/icons/document.svg"/> </span>

                <button @click="clearFilters" class="btn">Rensa filter</button>
            </div>
            
            
        </div>

        <div
            class="print-list"
            v-for="(session, index) in sessions"
            v-bind:key="index"
        >
            
            <div
                v-for="(message, index) in filter(session.sessionLogs)"
                v-bind:key="index"
            >
                <message-component
                    :message="message"
                    :enableComment="false"
                    :rootOnly="false"
                />
                <div class="flags">
                    <FacilitatorMessageFlags
                        :textMode="true"
                        :message="message"
                    ></FacilitatorMessageFlags>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import FacilitatorMixin from '../mixins/FacilitatorMixin';
    import {Component, Ref} from 'vue-property-decorator';
    import {mapState} from 'vuex';
    import MessageComponent from '@/components/MessageComponent.vue';
    import {Session} from '../Types/Session';
    import SearchFilter from '../Types/SearchFilter';
    import FacilitatorActivityList from '@/components/FacilitatorActivityList.vue';
    import {User} from '../Types/User';
    import {SessionLog} from '../Types/SessionLog';
    import FacilitatorMessageFlags from '@/components/FacilitatorMessageFlags.vue';
    import {mixins} from 'vue-class-component';
    import Tooltip from '@/components/Tooltip.vue';
    import RadioButton from "@/components/RadioButton.vue";
    import ParticipantFilterList from "@/components/ParticipantFilterList.vue";
    import TagFilterList from "@/components/TagFilterList.vue";
    import {SessionLogTag} from "@/Types/SessionLogTag";
    
    @Component({
    computed: {...mapState(['user']), tags : () => SessionLogTag },
    components: {
        TagFilterList,
        RadioButton,
        FacilitatorActivityList,
        MessageComponent,
        FacilitatorMessageFlags,
        Tooltip,
        ParticipantFilterList,
    },
    
})


export default class Print extends mixins(FacilitatorMixin) {
        [x: string]: any;
    public searchFilter: SearchFilter = new SearchFilter();
    public sessionId!: number;
    public user!: User;
    private selectedSessions: Session[] = [];
    @Ref('tagFilter')
    private tagFilter!:TagFilterList;
    @Ref('participantFilter')
    private participantFilter!:ParticipantFilterList;

    private classObject(session: Session): any {
        const index = this.sessions.indexOf(session);
        const color = (index % 6) + 1;
        return {['color' + color]: true};
    }
    
    private sessionFilter( selectedSessions:Session[] ) {
        this.selectedSessions = selectedSessions;
    }
    
    public sum():ParticipantRow[] {
        return this.selectedSessions.map(s => {
            let pr = new ParticipantRow();
            pr.participant = s.participant;
            pr.tone = s.sessionLogs.filter((x) => {
                return (x.sessionLogTag & this.tags.tone) == this.tags.tone;
            }).length;
            pr.emotional = s.sessionLogs.filter((x) => {
                return (x.sessionLogTag & this.tags.emotional) == this.tags.emotional;
            }).length;
            pr.facts = s.sessionLogs.filter((x) => {
                return (x.sessionLogTag & this.tags.facts) == this.tags.facts;
            }).length;
            return pr;
        });
    }
    
    public created() {
        this.loadSessions();
        this.loadScenario();
        this.loadActivity();

        this.$store.dispatch('loadScenarios');
    }

    public  clearFilters() {
        this.participantFilter.clear();
        this.tagFilter.clear();
    }

    private selectedTags: number = 1;
    
    private setTags(tags: number) {
        
        this.selectedTags = tags;
    }

    private get sessions(): Session[] {
        return this.selectedSessions.length > 0 ? this.selectedSessions : this.user.sessions;
    }

    private filter(sessionLogs: SessionLog[]) {
        let result = sessionLogs.filter((sl) => {
            return (
                sl.messageType !== 'ScenarioEvent' &&
                (sl.botReplyProperties > 1 || sl.sessionLogTag > 1)
            );
        });
        if (this.selectedTags > 1) {
            result = result.filter((sl: SessionLog) => {
                // tslint:disable:no-bitwise
                return (sl.sessionLogTag & this.selectedTags) > 1;
                // tslint:enable:no-bitwise
            });
        }
        return [...result];
    }

    private print() {
        window.print();
    }
}

class ParticipantRow {
    public participant!: string;
    public facts!: number;
    public emotional!: number;
    public tone!: number;
}

</script>
<style scoped lang="scss">
@import '../assets/scss/colors';
@import '../assets/scss/typography';
@import '../assets/scss/elements';
@import '../assets/scss/spacings';

.show-print-only {
    display: none;
}

@media print {
    .hide-print {
        display: none;
    }

    .show-print-only {
        display: block;
    }
    
    .card {
        padding: 0 !important;
        margin: 0 !important;
    }
}


div.print-list {
    .flags {
        @media print {
            border-bottom: 1px solid;
        }

        margin-bottom: $space-md;
    }
    margin-bottom: $space-md;
}
div.flags {
    margin-left: auto;
    margin-right: auto;
}

div.buttons {
    padding: 20px 0;
}

h3 {
    margin-top: $space-sm;
    margin-bottom: $space-sm;
}
.selected-filters {
    display: flex;
    align-items: center;
    margin-bottom: $space-xl;
    
    span:first-child, span:nth-child(2) {
            border: 0;
    }
    
    span  {
        padding: 0 $space-sm 0 $space-sm ;
    }
    
    span + span {
        border-left: 1px solid $border-primary-color;
    }  
    
    button {
        margin-left: auto;
    }
}

span.bold-label {
    padding: 0;
}

@include color-dots('sm');
    
.selected-user{
    display: flex;
    align-items: center;
}
    
table {
    margin: 30px 0;
    width: 100%;
    
    td {
        padding: 10px;
        img {
            vertical-align: middle;
        }
    }
    td:first-child {
        padding-left: 0;
        width: 40%;
    }
    
}
    
</style>
