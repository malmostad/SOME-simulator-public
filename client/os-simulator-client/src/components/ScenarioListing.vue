<template>
    <div class="container">
            <div class="card">
                <h2 class="big">
                    Scenario {{ numbering }}
                    <span class="sub-heading">{{ scenario.name }}</span>
                </h2>
                <span>{{ scenario.description }}</span>
            </div>
            <div class="edit">
                <button
                    class="edit"
                    @click="edit"
                    @keyup.native.enter="edit">
                    Redigera
                </button>
            </div>
        </div>
</template>

<script lang="ts">
    import {Component, Prop, Vue} from 'vue-property-decorator';
    import router from '../router'
    import { Scenario } from '../Types/Scenario';
    import axios, {AxiosResponse} from "axios";

    @Component({})
    export default class ScenarioListing extends Vue {
        @Prop({ default: Object as () => Scenario })
        public scenario!: Scenario;
        @Prop({ default: 1 })
        public numbering!: number;

        public edit(): void {
            //router.push('/editscenario');
            router.push({ name: 'editscenario', params: { scenarioId: this.scenario.id.toString() } })
        }
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';

    .container {
        display: flex;
        flex-direction: column;
        margin: 0 15px 15px 15px;
        
        .edit-scenario{
            display: flex;
        }
        .card {
            padding: 40px 50px 65px 50px;
            background-color: $gray4;
        }
    }

    .edit {
        text-align: center;
        margin: auto 0 $space-sm 0;
        cursor: pointer;
    }

    button {
        width: 200px;
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

    button.edit {
        margin-top: 30px;
    }
</style>