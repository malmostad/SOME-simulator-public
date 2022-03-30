<template>
    <div class="join-container-wrapper" v-cloak>
        
        <AlertComponent/>
        <div class="join-container">
            <div class="col">
                <h1>
                    Det här är inte sociala medier.
                    <!-- <p class="sub-heading some-logo">SoMe simulatorn</p> -->
                </h1>
                <div>
                <p>Men det är väldigt likt. Här finns följare och kommentarer. Nyheter och delningar, och flera parallella flöden att hantera. Skillnaden är bara att det inte är på riktigt. <br></p>
                <p class="margin-top-md">Det här är en sociala medier-simulator.</p>          
                </div>
            </div>
            <div class="col">
                <h2 class="big">Anslut till övning</h2>
                <div>
                    <label>
                        <input
                            placeholder=" "
                            required
                            type="text"
                            v-model="participantInput"
                            @keyup.enter="join"
                        />
                        <span>Namn</span>
                    </label>
                </div>
                <div>
                    <label>
                        <input
                            placeholder=" "
                            required
                            type="text"
                            v-model="typeableCodeInput"
                            @keyup.enter="join"
                        />
                        <span>Anslutningskod</span>
                    </label>
                </div>
                <p class="small info-text">Koden får du från din övningsledare.</p>
                <div class="button-div">
                    <button
                        class="primary"
                        :disabled="
                            participantInput.length < 3 ||
                                typeableCodeInput.length < 5
                        "
                        @click="join()"
                    >
                        Anslut
                    </button>
                </div>
                <div class="facilitator-link">
                    <router-link to="/signin">Inloggning för övningsledare &raquo;</router-link>
                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import axios, { AxiosResponse } from 'axios';
import router from '../router';
import AlertComponent from '../components/AlertComponent.vue';

interface TypeAbleCode {
    typeableCode: string;
}

@Component({components: {AlertComponent}})
export default class Join extends Vue {
    @Prop({ default: '' })
    public readonly typeableCode!: TypeAbleCode;
    public typeableCodeInput: string = '';
    public participantInput: string = '';
    private $hub: any;
    private hubStartPromise!: Promise<void>;

    private participate() {
        router.push({ name: 'participant', params: {} });
    }

    public join(): void {
        const hub = this.$hub;
        axios
            .post(process.env.VUE_APP_SOME_PARTICIPANT_API + '/join', {
                participant: this.participantInput,
                typeableCode: this.$data.typeableCodeInput.toUpperCase(),
            })
            .then((payload: AxiosResponse<any>) => {
                this.$store.dispatch('setSessionId', payload.data.sessionGuid);
                this.$store.dispatch('setDuration', payload.data.duration);
                this.$store.dispatch('setParticipant', this.participantInput);
                this.$store.dispatch('setShortRead', 0);
                this.$store.dispatch('setLongRead', 0);
                this.$store.dispatch('clearMessages');
                this.$store.dispatch('setUserRoles', []);
                this.$data.typeableCodeInput = '';
                this.$data.participantInput = '';

                this.hubStartPromise.then((promise) => {
                    hub.invoke('JoinSessionGroup', payload.data.sessionGuid)
                    .then(() => {
                        this.participate();
                    })
                    .catch((e) => {
                        setTimeout(() => {
                            hub.invoke(
                                'JoinSessionGroup',
                                payload.data.sessionGuid
                            ).then(() => {
                                this.participate();
                            })
                            .catch(() => {
                                this.$store.dispatch('showAlert', {
                                    title: 'Kan inte ansluta',
                                    content: 'Det gick inte att ansluta till sessionen. Kontrollera att anslutningskoden är rätt.',
                                });
                            });
                        }, 1000);
                    });

                });
            })
            .catch((payload: AxiosResponse<any>) => {
                this.$store.dispatch('setSessionId', '');
                this.$store.dispatch('showAlert', {
                    title: 'Kan inte ansluta',
                    content: 'Det gick inte att ansluta till sessionen. Kontrollera att anslutningskoden är rätt.',
                });
            });
    }

    public created() {
        if (this.$store.state.session.sessionId) {
            this.participate();
        }

        if (this.$props.typeableCode) {
            this.$data.typeableCodeInput = this.$props.typeableCode;
        }
        // tslint:disable-next-line:no-empty
        // Vue.prototype.hubStartpromise.then((promise) => {});
    }
}
</script>

<style lang="scss" scoped>
@import '../assets/scss/colors';
@import '../assets/scss/spacings';

.button-div,
.info-text {
    text-align: center;
}

.facilitator-link {
    margin-top: $space-sm;
}

.join-container-wrapper {
    position: fixed;
    top: 0;
    left: 0;
    display: flex;
    align-items: center;
    height: 100%;
}

.join-container {
    display: flex;
    margin: 0 275px 0 275px;
    align-items: baseline;

    p.margin-top-md {
        margin-top: $space-sm;
    }
    
    div.col {
        width: 100%;
        flex-direction: column;
        flex: 0 1 auto;
    }

    div.col:nth-child(1) {
        padding-right: 70px;
    }

    div.col:nth-child(2) {
        padding-left: 70px;
        border-left-color: $border-primary-color;
        border-left-width: thin;
        border-left-style: solid;
    }

    h1 {
        margin-bottom: 13px;
    }

    button {
        margin-top: 40px;
        width: 170px;
    }

    input {
        box-sizing: border-box;
        width: 100%;
    }

    h2.big {
        margin-bottom: 10px;
        margin-left: 25px;
    }
}
</style>
