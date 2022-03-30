<template>
    <div class="flex">
        <div class="flex-col-full-width">
            <h1 class="big">
                {{ groupName }}
                <span class="sub-heading">Anslutningskod: {{ typeableCode }}</span>
            </h1>
        </div>
        <div class="flex-col-full-width right">
            <div class="some-simulator"><b>Organisation</b></div>
            <div>
                <router-link @click.native="signOut()" to="SignIn">Logga ut</router-link>
            </div>
        </div>
    </div>
</template>


<script lang="ts">
import Vue from 'vue';
import { Component, Prop } from 'vue-property-decorator';
import axios from 'axios';

@Component
export default class SessionInformation extends Vue {
    @Prop({ default: '' })
    public groupName!: string;

    @Prop({ default: '' })
    public typeableCode!: string;

    public signOut() {
        this.$store
            .dispatch('stopSessions', this.$store.state.user.sessionGroupId)
            .then(() => {
                axios.post(process.env.VUE_APP_SOME_USER_API + '/signout')
            })
            .then(() => {
                    this.$store.dispatch('signOut')
                    }).finally(() => {
            this.$router.push('signIn');
        })
            .catch((e) => {
                console.error(e);
                console.trace();
            });
    }
}
</script>

<style lang="scss" scoped>
@import '../assets/scss/typography';
@import '../assets/scss/colors';

span {
    display: block;
}

div.flex {
    margin-bottom: 42px;
}

.right {
    text-align: right;
}

h1 {
    line-height: 48px;
}

.some-simulator {
    @include typography(regular, 14, $default-text-color);
}

a {
    @include typography(regular, 12);
}
</style>
