import Vue from 'vue';
import App from './App.vue';
import router from './router';
import store from './store';
import messageHub from './services/messagesHub';
import VueScrollTo from 'vue-scrollto';
import './settings/AxiosSettings';

import './assets/scss/style.scss';
import './assets/scss/fonts/opensans_extrabold_macroman/stylesheet.css';
import './assets/scss/fonts/opensans_light_macroman/stylesheet.css';
import './assets/scss/fonts/opensans_regular_macroman/stylesheet.css';
import './assets/scss/fonts/opensans_bold_macroman/stylesheet.css';

Vue.config.productionTip = false;

Vue.config.errorHandler = (err, vm, info) => {
    console.error(err);
    console.log(vm);
    console.info(info);
};

Vue.use(messageHub);
Vue.use(VueScrollTo, {
    container: 'body',
    duration: 500,
    easing: 'ease',
    offset: 0,
    force: true,
    cancelable: true,
    onStart: false,
    onDone: false,
    onCancel: false,
    x: false,
    y: true,
});
new Vue({
    router,
    store,
    render: (h) => h(App),
}).$mount('#app');
