import router from '../router';
import axios from 'axios';
import store from '../store';
import Alert from '@/Types/Alert';

axios.defaults.withCredentials = true;

axios.interceptors.response.use(
    (r) => r,
    (e) => {
        if (e.response.status === 401) {
            store.dispatch('signOut');
            if (router.currentRoute.name !== 'signIn') {
                router.push('signin');
            }
        }
        return Promise.reject(e);
    }
);
