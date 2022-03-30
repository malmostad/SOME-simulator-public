import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';
import Participant from './views/Participant.vue';
import Facilitator from './views/Facilitator.vue';
import Login from './views/Login.vue';
import SignIn from './views/SignIn.vue';
import Join from './views/Join.vue';
import Print from './views/Print.vue';
import Admin from '@/views/Admin.vue';
import EditScenario from '@/views/EditScenario.vue';

Vue.use(Router);

export default new Router({
    mode: 'history',
    base: process.env.BASE_URL,
    routes: [
        {
            path: '/',
            redirect: {name: 'join'},
        },
        {
            path: '/login',
            redirect: {name: 'signin'},
        },
        {
            path: '/deltagare/',
            name: 'participant',
            component: Participant,
        },
        {
            path: '/anslut/:typeableCode?',
            name: 'join',
            component: Join,
            props: true,
        },

        {
            path: '/signin',
            name: 'signin',
            component: SignIn,
        },
        {
            path: '/facilitator',
            name: 'facilitator',
            component: Facilitator,
        },
        {
            path: '/admin',
            name: 'admin',
            component: Admin,
        },
        {
            path: '/editscenario',
            name: 'editscenario',
            component: EditScenario,
        },
        {
            path: '/print',
            name: 'print',
            component: Print,
        },
    ],
});
