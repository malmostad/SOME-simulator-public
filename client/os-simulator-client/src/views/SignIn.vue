<template>
    <div class="sign-in-container-wrapper" v-cloak>
        <div class="sign-in-container">
            <div class="col">
                <h1>
                    Det här är inte sociala medier.
                    <!-- <p class="sub-heading some-logo">SoMe simulatorn</p> -->
                </h1>
                <div>
                    <p>Men det är väldigt likt. Här finns följare och kommentarer. Nyheter och delningar, och flera
                        parallella flöden att hantera. Skillnaden är bara att det inte är på riktigt. <br></p>
                    <p>Det här är en sociala medier-simulator.</p>
                </div>
            </div>

            <div class="col">
                <h2 class="big">Logga in</h2>
                <p class="small error" v-if="errorText">Inloggningen misslyckades. Kontrollera användarnamn och
                    lösenord.</p>
                <div>
                    <label>
                        <input
                            v-bind:class="{'error':error}"
                            required
                            type="text"
                            v-model="email"
                            @keyup.enter="signIn()"
                            @focus="error = false"
                        />
                        <span>Användarnamn</span>
                    </label>
                </div>
                <div>
                    <label>
                        <input
                            v-bind:class="{'error':error}"
                            required
                            type="password"
                            v-model="password"
                            @keyup.enter="signIn()"
                            @focus="error = false"
                        />
                        <span>Lösenord</span>
                    </label>
                </div>
                <p class="small info-text"></p>
                <div class="button-div">
                    <button
                        class="primary"
                        :disabled="email.length < 1"
                        @click="signIn()"
                    >Logga in
                    </button>
                </div>
            </div>
        </div>
        <AlertComponent></AlertComponent>
    </div>
</template>


<script lang="ts">
    import axios, {AxiosResponse} from 'axios';
    import {Component, Prop, Vue, Watch} from 'vue-property-decorator';
    import router from '../router';
    import store from '../store';
    import {mapState} from 'vuex';
    import AlertComponent from '../components/AlertComponent.vue';
    import Alert from '../Types/Alert';
    import UserRole from '@/Types/UserRole';

    @Component({
        components: {AlertComponent},
        computed: mapState(['signedIn']),
    })
    export default class SignIn extends Vue {
        public email = '';
        private password = '';
        private error = false;
        private errorText = false;
        
        public created() {
            // this.signOut();
        }
        
        private signIn() {
            try {
            axios
                .post(process.env.VUE_APP_SOME_USER_API + '/signin',
                    {password: this.password, username: this.email})
                .then((payload: AxiosResponse<any>) => {
                    if (payload.status === 200) {
                        if (payload.data.sessionGroupId != null) {
                            this.$store.dispatch('setSessionGroupId', payload.data.sessionGroupId);
                            this.$store.dispatch('setTypeableCode', payload.data.typeableCode);
                            this.$store.dispatch('setCurrentScenario', payload.data.scenario);
                            this.$store.dispatch('setUserRoles', payload.data.userRoles);
                            this.$store.dispatch('setStatus', this.status(payload.data.status))
                            this.$store.dispatch('setGroupName', payload.data.groupName);
                        }
                        const userRoles: UserRole[] = payload.data.userRoles;
                        this.$store.dispatch('setUserRoles', userRoles);
                        
                        if(this.$store.getters.isAdmin && !payload.data.sessionGroupId) {
                            router.push({name: 'admin'});
                            store.dispatch('signIn');
                        }
                        else if(this.$store.getters.isFaciliator) {
                            router.push({name: 'facilitator'});
                            store.dispatch('signIn');
                        }
                    }
                })
                .catch((Error) => {
                    console.log(Error);
                    this.error = true;
                    this.errorText = true;
                    this.password = '';

                }).finally(() => {
            });

            } catch(Err) {
                console.error(Err);
            }
        }
        
        private status(statusCode: number) : string {
            let status = '';
            switch (statusCode) {
                case 1:
                    status = 'New';
                    break;
                case 2:
                    status = 'Running';
                    break;
                case 3:
                    status = 'Paused';
                    break;
                case 4:
                    status = 'Finished';
                    break;
                case 5:
                    status = 'Cancelled';
                    break;
            }
            return status;
        }

        private signOut() {
            axios
                .post(process.env.VUE_APP_SOME_USER_API + '/signout')
                .then((payload: AxiosResponse<any>) => {
                    store.dispatch('signOut');
                });     
            }
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';

    p.error, div.error {
        margin: $space-sm 0 $space-sm 0;
        color: $red;
    }

    .button-div,
    .info-text {
        text-align: center;
    }


    .sign-in-container-wrapper {
        position: fixed;
        top: 0;
        left: 0;
        display: flex;

        align-items: center;
        height: 100%;
    }

    .sign-in-container {
        display: flex;
        align-items: baseline;


        margin: 0 275px 0 275px;

        div.col {
            //Grow shrink basis
            flex: 0 1 auto;
            width: 100%;
            flex-direction: column;
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

        p {
            margin-bottom: 15px;
        }

        button {
            margin-top: 40px;
            width: 170px;
        }

        input {
            box-sizing: border-box;
            width: 100%;
        }

        input.error {
            box-shadow: inset 0 0 0 1px $red;
        }

        h2.big {
            margin-bottom: 10px;
            margin-left: 25px;
        }
    }
</style>