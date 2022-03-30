import signalR, {
    HubConnectionBuilder,
    LogLevel,
    HubConnection,
    HttpTransportType,
} from '@aspnet/signalr';
import store from '../store';
import { SessionLog } from '@/Types/SessionLog';
import Message from '@/Types/Message';
import DetectBrowser from '@/helpers/DetectBrowser';

export default {
    install(Vue: any) {
        console.log(`${process.env.VUE_APP_SOME_SERVER}/sr`);

        var db = new DetectBrowser();
        var protocol = db.isSafari() ? 4 : 1;

        const connection = new HubConnectionBuilder()
            .withUrl(`${process.env.VUE_APP_SOME_SERVER}/sr`, {
                transport: protocol,
            })
            .configureLogging(LogLevel.Debug)
            .build();

        let startedPromise: any = null;

        function start(): Promise<void> {
            startedPromise = connection
                .start()
                .then(() => {
                    console.log('Connection to hub established.');
                })
                .catch((err) => {
                    console.error('Failed to connect with hub', err);
                    return new Promise((resolve, reject) =>
                        setTimeout(
                            () =>
                                start()
                                    .then(resolve)
                                    .catch(reject),
                            5000
                        )
                    );
                });

            return startedPromise;
        }

        connection.onclose((error) => {
            console.error(
                '--- Lost connection to hub. Trying to reconnect. ---'
            );
            Vue.prototype.hubStartPromise = start().then(() => {
                console.info('---- Connected again ----');
                
                if (
                    !store.state.user ||
                    !store.getters.isFaciliator
                ) {
                    store.dispatch(
                        'reloadMessages',
                        store.state.session.sessionId
                    );
                    Vue.prototype.$hub.invoke(
                        'JoinSessionGroup',
                        store.state.session.sessionId
                    );

                } else {
                    store.dispatch(
                        'loadActivity',
                        store.state.user.sessionGroupId
                    );

                    store.dispatch(
                        'loadEvents',
                        store.state.user.sessionGroupId
                    );
                }
            });
        });

        Vue.prototype.hubStartPromise = start();
        // use new Vue instance as an event bus
        const messageHub = new Vue();
        // every component will use this.$questionHub to access the event bus
        Vue.prototype.$messageHub = messageHub;
        // Forward server side SignalR events through $questionHub, where components will listen to them

        Vue.prototype.$hub = connection;

        function countBranch(message: SessionLog): number {
            let sum = 1;
            if (message.children) {
                message.children.forEach((child) => {
                    sum += countBranch(child);
                });
            }

            return sum;
        }

        function countSessionLogs(data: Message) {
            const sessionId = localStorage.getItem('sessionId') || null;
            
            let MessageCount = 0;
            for (let i = 0; i < store.state.session.messages.length; i++) {
                const message = store.state.session.messages[i] as SessionLog;
                MessageCount += countBranch(message);
            }
                console.log("client " + MessageCount)
                console.log("api " + data.messageCount)
            if (
                MessageCount !== data.messageCount &&
                sessionId != null
            ) {
                console.error('Client is not in sync.');
                store.dispatch('reloadMessages', sessionId);
            }
        }

        
        connection.on('messages', (message: any) => {
            const data = JSON.parse(message);
            const sessionLog = data.sessionLog;
            const sessionGroup = data.sessionGroup;

            const payload = {
                sessionGroup,
                sessionLog,
            };

            store.dispatch('addMessage', payload)
                .then(() => {
                countSessionLogs(data)
            });
            
            if (payload.sessionLog.messageType === 'ScenarioEvent') {
                store.commit('setCurrentEvent', payload.sessionLog.Heading);
            }
        });

        connection.on('comments', (message: any) => {
            
            const data = JSON.parse(message);
            store.dispatch('addComment', data);
        });

        connection.on('sessionTriggers', (sessionTrigger: any) => {
            const data = JSON.parse(sessionTrigger);
            
            store.commit('showDialog', data.Dialog);
            store.commit('setStatus', data.Status);
            if (
                store.getters.isFacilitator &&
                data.Status == 'Cancelled'
            ) {
                store.dispatch('clearSessions');
                store.dispatch('setStatus', '');
                store.dispatch('setSessionGroupId', '');
                store.dispatch('clearScenarioEvent');
                store.dispatch('clearActivityLog');
            }
        });

        connection.on('progress', (progress: any) => {
            store.commit('setProgress', progress);
        });

        connection.on('showForGroup', (sessionLog: any) => {
            const data = JSON.parse(sessionLog);
            store.dispatch('showForGroup', data);
        });
    },
};
