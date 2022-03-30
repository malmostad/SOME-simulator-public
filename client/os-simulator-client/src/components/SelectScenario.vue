<template>
    <div class="container">
        <hr />
        <div class="select-scenario">
            <div class="description">
                <div class="card">
                    <h2 class="big">
                        Scenario {{ numbering }}
                        <span class="sub-heading">{{ scenario.name }}</span>
                    </h2>
                    <span>{{ scenario.description }}</span>
                </div>
            </div>

            <div class="settings">
                <h2>Längd på övningen* (rekommenderad längd: 1:00)</h2>
                <div class="time-settings">
                    <input
                        required
                        disabled
                        class="time-select"
                        type="text"
                        v-model="formatedMinutes"
                        aria-label="Välj tidsåtgång"
                    />
                    <!--suppress RequiredAttributes -->
                    <step-input
                        v-model="minutes"
                        v-bind:min="30"
                        v-bind:max="90"
                        v-bind:step="15"
                    ></step-input>
                </div>

                <h2 class="group">Gruppnamn*</h2>
                <div class="group-name">
                    <input
                        class="group-name"
                        type="text"
                        v-model="groupName"
                        aria-label="Döp din grupp"
                        @keyup.enter="create"
                    />
                </div>
                <div class="create">
                    <button
                        class="create"
                        :disabled="!canPost()"
                        @click="create"
                        @keyup.native.enter="create"
                    >
                        Skapa anslutningskod
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import axios, { AxiosResponse } from 'axios';
import { Scenario } from '@/Types/Scenario';
import StepInput from '../components/StepInput.vue';
import Format from '../helpers/Format';

@Component({
    components: {
        StepInput,
    },
})

export default class SelectScenario extends Vue {
    @Prop({ default: Object as () => Scenario })
    public scenario!: Scenario;
    @Prop({ default: 1 })
    public numbering!: number;
    public minutes: string = '60';
    public groupName: string = '';

    public create(): void {
        if (!this.canPost()) {
            return;
        }
        axios
            .post(process.env.VUE_APP_SOME_FACILITATOR_API + '/create', {
                scenarioId: this.scenario.id,
                groupName: this.groupName,
                minutes: this.minutes,
            })
            .then((payload: AxiosResponse<any>) => {
                this.$store.dispatch('setSessionGroupId', payload.data.id);
                this.$store.dispatch(
                    'setTypeableCode',
                    payload.data.typeableCode
                );
                this.$store.dispatch('setGroupName', payload.data.groupName);
                this.$store.commit('setStatus', payload.data.status);
                this.$store.dispatch('loadScenario');

                this.$emit('scenario-selected');
            });
    }
    public canPost(): boolean {
        if (!this.groupName) {
            return false;
        }
        if (!this.minutes) {
            return false;
        }
        return RegExp('([1-9][0-9]*)').test(this.minutes.toString());
    }
    get formatedMinutes(): string {
        return Format.minutes(this.minutes);
    }
}
</script>

<style lang="scss" scoped>
@import '../assets/scss/colors';
@import '../assets/scss/spacings';

div.container {
    margin-bottom: $space-md;
}

div.create {
    text-align: center;
}

button {
    width: 266px;
}

div.description {
    .card {
        padding: 40px 50px 65px 50px;
    }
    margin-right: 25px;
}
div.settings {
    margin-left: 25px;

    input.group-name {
        width: 100%;
    }
}

hr {
    margin-top: 30px;
    margin-bottom: 40px;
}

.sub-heading {
    color: $default-text-color;
    display: block;
    margin-bottom: 24px;
}

button.create {
    margin-top: 72px;
}

div.settings > h2 {
    margin-left: 10px;
}

h2.group {
    margin-top: 55px;
}

.group-name {
    margin-top: 19px;
}

.time-settings {
    font-size: 0;
    margin-top: 19px;
    height: 50px;
}

input[type='text'] {
    font-size: 16px;
}

.time-select {
    display: inline-block;
    vertical-align: bottom;
    width: 200px;
}

.step-input {
    margin-left: 16px;
    display: inline-block;
    vertical-align: bottom;
}

div.select-scenario {
    display: flex;
}

div.select-scenario > * {
    flex: 1 100%;
}
</style>
