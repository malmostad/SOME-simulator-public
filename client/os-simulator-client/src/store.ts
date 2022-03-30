import Vue from 'vue';
import Vuex from 'vuex';
import {SessionLog} from './Types/SessionLog';
import axios, {AxiosResponse} from 'axios';
import Dialog from './Types/Dialog';
import {Session} from './Types/Session';
import State from './Types/State';
import CurrentScenario from './Types/CurrentScenario';
import Alert from './Types/Alert';
import {Scenario} from "@/Types/Scenario";
import UserRole from "@/Types/UserRole";
import EditScenario from "@/Types/Admin/EditScenario";
import EditScenarioEvent from "@/Types/Admin/EditScenarioEvent";
import {User} from "@/Types/User";

Vue.use(Vuex);

function addSessionLog(sessionLogs: SessionLog[], payload: SessionLog): SessionLog[] {
    const index = sessionLogs.findIndex((x) => {
        if (x.messageType === 'ScenarioEvent') {
            return x.scenarioEventId === payload.scenarioEventId;
        }
        if (x.messageType === 'Post' || x.messageType === 'Message' || x.messageType == 'Participant') {
            return x.id === payload.id;
        }
        return false;
    });

    if (index > -1) {
        sessionLogs[index] = payload;
        return [...sessionLogs];
    } else {
        return [payload, ...sessionLogs];
    }
}

export default new Vuex.Store({
    state: new State(),

    getters: {
        currentScenario: (state) => state.user.currentScenario,
        
        isAdmin:(state):boolean => {
            return state.user != null && state.user.userRoles.some(roles => roles.role.name === 'Admin');
        },
        
        isFaciliator:(state):boolean => {
            return state.user != null && state.user.userRoles.some(roles => roles.role.name === 'Facilitator');
        },
        hasSessionGroupId: (state): boolean =>  {
            return state.user != null && state.user.sessionGroupId != null && state.user.sessionGroupId >= 0;
        }
    },

    mutations: {
        setUserRoles(state:State, userRoles: UserRole[]) {
            state.user.userRoles = userRoles;
        },
        
        clearNewSessionLogs (state:State) {
            state.newSessionLogs = [];
        },   
        
        addNewSessionLog(state:State, sessionLog:SessionLog) {
            state.newSessionLogs.push(sessionLog.id);
        },
        
        deleteScenarioEvent(state, scenarioEvent) {
            if(!state.user.editScenario) {
                return
            }
            
            let id = scenarioEvent.phaseId;
            
            let phase = state.user.editScenario.phases.filter(p =>  p.id == id)[0];
            
            if(phase) {
                phase.scenarioEvents = phase.scenarioEvents.filter(s => s.id != scenarioEvent.id )
                state.user.editScenario.phases = [...state.user.editScenario.phases];
            }
        },

        updateScenarioEvent(state, scenarioEvent:EditScenarioEvent) {
            if(!state.user.editScenario) {
                return
            }

            let id = scenarioEvent.phaseId;

            let phase = state.user.editScenario.phases.filter(p =>  p.id == id)[0];

            if(phase) {
                
                let index = phase.scenarioEvents.findIndex(s => s.id === scenarioEvent.id);

                phase.scenarioEvents[index] = scenarioEvent;
                
                phase.scenarioEvents = phase.scenarioEvents.sort((s1:EditScenarioEvent, s2:EditScenarioEvent) => s1.timePercent - s2.timePercent );

                state.user.editScenario.phases = [...state.user.editScenario.phases];
            }
        },

        createScenarioEvent(state, scenarioEvent: EditScenarioEvent) {
            if (state.user.editScenario == null)
                return;
            
            let id = scenarioEvent.phaseId;

            let phase = state.user.editScenario.phases.filter(p => p.id == id)[0];

            if (phase) {
                
                phase.scenarioEvents.push(scenarioEvent);

                phase.scenarioEvents = phase.scenarioEvents.sort((s1: EditScenarioEvent, s2: EditScenarioEvent) => s1.timePercent - s2.timePercent);

                state.user.editScenario.phases = [...state.user.editScenario.phases];
            }
        },
        
        deleteComment(state, comment) {

            if(!state.user.editScenario) {
                return    
            }
            
            let editComments = state.user.editScenario.comments;
            
            let index = editComments.findIndex(p => p.id == comment.id);

            if(index >= 0) {
                editComments.splice(index,1);
                editComments = [...editComments];

            }
        },

        updateComment(state, comment) {
            if(!state.user.editScenario) {
                return
            }

            let editComments = state.user.editScenario.comments;

            let index = editComments.findIndex(p => p.id == comment.id );
            editComments[index] = comment;
            state.user.editScenario.comments = [...editComments];
        },

        createComment(state, comment) {
            console.log("store.ts - createComment() - START")
            console.log("state: ", state);
            console.log("comment: ", comment);
            console.log("store.ts - createComment() - END")

            if(state.user.editScenario != null) {
                state.user.editScenario.comments.push(comment);
            }
        },
        
        deletePost(state, post) {
            if(!state.user.editScenario) {
                return ;
            }
            let posts = state.user.editScenario.posts;
            
            let index = posts.findIndex(p => p.id == post.id);
            
            if(index >= 0) {
                posts.splice(index,1);
                state.user.editScenario.posts = [...(posts)];

            }
        },
        
        updatePost(state, post) {
            if(!state.user.editScenario) {
                return ;
            }
            let posts = state.user.editScenario.posts;
            
            let index = posts.findIndex(p => p.id == post.id );
            posts[index] = post;
            state.user.editScenario.posts = [...posts];
        },
        
        createPost(state, post) {
            console.log("store.ts - createPost() - START")
            console.log("state: ", state);
            console.log("post: ", post);
            console.log("store.ts - createPost() - END")

            if(state.user.editScenario != null) {
                state.user.editScenario.posts.push(post);
            }
        },
        
        showAlert(state, alert: Alert) {
            state.alert = alert;
        },

        closeAlert(state) {
            state.alert = null;
        },

        closeConfirmAlert(state) {
            state.alert = null;
        },

        signOut(state) {
            state.signedIn = false;
        },

        setShortRead(state, read: number) {
            state.shortRead = read;
        },
        setLongRead(state, read: number) {
            state.longRead = read;
        },

        showForGroup(state, sessionLog: SessionLog) {
            state.show = sessionLog;
        },
        setSignedIn(state, payload: boolean) {
            state.signedIn = payload;
        },
        setGroupName(state, payload: string) {
            state.user.groupName = payload;
        },

        loadNewSessionGroups(state, payload: []) {
            state.sessionGroups = [...payload];
        },

        loadScenarios(state, payload: Scenario[]) {
            state.user.scenarios = [...payload];
        },
        setCurrentEditScenario(state, payload: EditScenario){
            state.user.editScenario = payload;
        },
        setProgress(state, payload: any) {
            state.progress = payload;
        },
        addMessage(state, payload: any) {
            if (state.user != null && state.user.userRoles.some(r => r.role.name == 'Faciliator')) {
                console.debug('Facilitator message incoming');

                const sessions: any[] = state.user.sessions;
                const sessionIndex = sessions.findIndex(
                    (s) => s.SessionGuid === payload.sessionGroup
                );
                sessions[sessionIndex].SessionLogs.push(payload.sessionLog);
                state.user.sessions = [...sessions];
            } else {
                console.log('Incoming message for participant.');
                state.session.messages.push(payload.sessionLog);
                state.session.messages = [...state.session.messages];
            }
        },
        
        addComment(state, payload: any) {
            let findParent = (sls:SessionLog[], sl:SessionLog, level:number):SessionLog|null => {
                console.debug();
                if (level == sessionLog.level) {
                    console.debug();
                    //If we are in the correct level we perform a search.
                    let index = sls.findIndex(x => x.id === sl.id)
                    if (index >= 0) {
                        return sls[index];
                    }
                    else {
                        return null;
                    }
                }   
                //Search in reverse order to increase performance.
                for (var i = sls.length -1 ; i >= 0 ; i--) {
                    let result = findParent(sls[i].children, sl, level + 1);
                    if(result != null) {
                        return result;
                    }
                }
                return null;
            }

            let sessionLog: SessionLog = { ...payload.sessionLog };
            
            let foundSessionLog = findParent(state.session.messages, sessionLog, 1);
            
            if(foundSessionLog != null) {
                foundSessionLog.children = [...sessionLog.children];
            }
            else {
                state.session.messages.push(sessionLog);
            }
            state.session.messages = [...state.session.messages]
            console.log(state.session.messages);
        },
        

        addScenarioEvent(state, payload: SessionLog) {
            const eventLogs = state.user.eventLog;
            state.user.eventLog = addSessionLog(eventLogs, payload);
        },

        addActivityLog(state, payload: SessionLog) {
            const activityLogs: any[] = state.user.activityLog;
            state.user.activityLog = addSessionLog(
                activityLogs,
                payload
            );
        },

        clearScenarioEvent(state) {
            state.user.eventLog = [];
        },

        clearActivityLog(state) {
            state.user.activityLog = [];
        },

        addSessionLogAction(state, sessionLogAction: any) {
            //
        },
        reloadMessages(state, messages: SessionLog[]) {
            state.session.messages = messages;
        },
        clearMessages(state, messages: SessionLog[]) {
            state.session.messages = [];
        },
        setParticipant(state, participant: string) {
            state.session.participant = participant;
        },
        setStatus(state, sessionStatus: string) {
            state.status = sessionStatus;
        },
        setEditScenario(state, scenarioId: number) {
            state.user.editScenarioId = scenarioId;
        },
        setSessionId(state, sessionId: string) {
            state.session.sessionId = sessionId;
        },
        showDialog(state, dialog: Dialog) {
            state.dialog = dialog;
        },
        closeDialog(state) {
            state.dialog.open = false;
        },
        loadSessions(state, sessions: Session[]) {
            state.user.sessions = sessions;
        },
        
        clearSessions(state) {
            state.user.sessions = [];
        },
        
        setSessionGroupId(state, id: number) {
            state.user.sessionGroupId = id;
        },
        setTypeableCode(state, code: string) {
            state.user.typeableCode = code;
        },
        setDuration(state, duration: string) {
            state.duration = duration;
        },
        setCurrentScenario(state, scenario: CurrentScenario) {
            state.user.currentScenario = { ...scenario };
        },
        setActivityLog(state, activityLog: SessionLog[]) {
            state.user.activityLog = activityLog;
        },
        setEvents(state, eventLogs: SessionLog[]) {
            state.user.eventLog = eventLogs;
        },
        setCurrentEvent(state, message: string) {
            state.user.currentEvent = message;
        },
        setSelectedFlowFacilitator(state, flow: number) {
            state.user.selectedFlow = flow;
        },
        changeStressLevel(state, level: number) {
            state.stressLevel = level;
        },
    },

    // Actions dispatched from components
    actions: {

        clearNewSessionLogs ({commit}) {
            commit('clearNewSessionLogs');
        },

        commentDialog({commit}, sessionLog: SessionLog) {
          console.log(`Comment dialog for ${sessionLog.id} - ${sessionLog.sender} -  ${sessionLog.text}`);
        },
        
        getFakeAlias() {
            return new Promise((resolve, reject) => {
                axios
                    .get(
                        process.env.VUE_APP_SOME_FACILITATOR_API + '/getAlias'
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload.data);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },
        
        showAlert({ commit }, alert: Alert) {
            commit('showAlert', alert);
        },

        closeAlert({ commit }) {
            commit('closeAlert');
        },

        closeConfirmAlert({ commit }) {
            commit('closeConfirmAlert');
        },

        setShortRead({ commit }, read: number) {
            commit('setShortRead', read);
            localStorage.setItem('shortRead', read.toString());
        },

        setLongRead({ commit }, read: number) {
            commit('setLongRead', read);
            localStorage.setItem('longRead', read.toString());
        },

        showForGroup({ commit }, sessionLog: SessionLog) {
            commit('showForGroup', sessionLog);
        },

        setSelectedFlowFacilitator({ commit }, flow: number) {
            commit('setSelectedFlowFacilitator', flow);
        },
        setProgress({ commit }, progress: number) {
            commit('setProgress', progress);
        },

        showDialog({ commit }, dialog: Dialog) {
            commit('showDialog', dialog);
        },

        closeDialog({ commit }) {
            commit('closeDialog');
        },

        setUserRoles({commit}, userRoles: UserRole[]) {
            localStorage.setItem('userRoles', JSON.stringify(userRoles));
            commit('setUserRoles', userRoles);
        },
        
        setSessionGroupId({ commit }, sessionGroupId: string) {
            localStorage.setItem('sessionGroupId', sessionGroupId);
            commit('setSessionGroupId', sessionGroupId);
        },
        setParticipant({ commit }, participant: string) {
            localStorage.setItem('participant', participant);
            commit('setParticipant', participant);
        },
        setSessionId({ commit }, id: string) {
            localStorage.setItem('sessionId', id);
            commit('setSessionId', id);
        },
        setStatus({ commit }, status: string) {
            commit('setStatus', status);
        },
        setEditScenario({ commit }, scenarioId: string) {
            localStorage.setItem('editScenarioId', scenarioId);
            commit('setEditScenario', scenarioId);
        },
        
        clearMessages({ commit }) {
            commit('clearMessages');
        },
        
        deleteScenarioEvent({ commit }, scenarioEvent) {
            return new Promise((resolve, reject) => {

                axios
                    .post(
                        process.env.VUE_APP_SOME_SCENARIO_EVENT_API +
                        '/deletescenarioevent/' + scenarioEvent.id
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('deleteScenarioEvent',scenarioEvent);
                        return resolve(payload);
                    })
                    .catch((payload: AxiosResponse<any>) => {
                        console.error(payload)
                        console.trace();
                        return reject();
                    });
            });
        },

        createScenarioEvent({ commit }, scenarioEvent) {
            
            console.log(process.env.VUE_APP_SOME_SCENARIO_EVENT_API);
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_SCENARIO_EVENT_API + '/CreateScenarioEvent',
                        scenarioEvent
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('createScenarioEvent', payload.data);
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },

        updateScenarioEvent({ commit }, comment) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_SCENARIO_EVENT_API + '/UpdateScenarioEvent',
                        comment
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('updateScenarioEvent',payload.data);
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },
        

        deleteComment({ commit }, comment) {
            return new Promise((resolve, reject) => {

                axios
                    .post(
                        process.env.VUE_APP_SOME_COMMENT_API +
                        '/deletecomment/' + comment.id
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('deleteComment',comment);
                        return resolve(payload);
                    })
                    .catch((payload: AxiosResponse<any>) => {
                        console.error(payload)
                        console.trace();
                        return reject();
                    });
            });
        },

        createComment({ commit }, comment) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_COMMENT_API + '/CreateComment',
                        comment
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('createComment', payload.data);
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },

        updateComment({ commit }, comment) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_COMMENT_API + '/UpdateComment',
                        comment
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('updateComment',payload.data);
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },
        
        deletePost({ commit }, post) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_POST_API +
                        '/deletepost/' + post.id
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('deletePost',post);
                        return resolve(payload);
                    })
                    .catch((payload: AxiosResponse<any>) => {
                        console.error(payload)
                        console.trace();
                        return reject();
                    });
            });
        },
        
        createPost({ commit }, post) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_POST_API + '/CreatePost',
                        post
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('createPost', payload.data);
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },
        
        updatePost({ commit }, post) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        process.env.VUE_APP_SOME_POST_API + '/UpdatePost',
                        post
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('updatePost',payload.data);
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },
        
        getStatus({ commit }, sessionId) {
            return new Promise((resolve, reject) => {
                axios
                    .get(
                        process.env.VUE_APP_SOME_PARTICIPANT_API +
                            '/GetStatus?guid=' +
                            sessionId
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('setStatus', payload.data.status);
                        if (payload.data.status === 'Finished') {
                            commit('setProgress', 1);
                        }
                        return resolve(payload);
                    })
                    .catch((error) => {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },

        reloadMessages({ commit }, sessionGuid) {
            console.info('Reloading messages');
            return new Promise((resolve, reject) => {
                if (!sessionGuid) return reject('Missing sessionGuid');

                axios
                    .get(`${process.env.VUE_APP_SOME_PARTICIPANT_API}`, {
                        params: { guid: sessionGuid },
                    })
                    .then((payload: AxiosResponse<any>) => {
                        commit('reloadMessages', payload.data);
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error("reloadMessages.catch: " + e)
                        console.trace();
                        return reject();
                    });
            });
        },

        loadNewSessionGroups({ commit }) {
            return new Promise((resolve, reject) => {
                axios
                    .get(
                        `${
                            process.env.VUE_APP_SOME_PARTICIPANT_API
                        }/getnewsessiongroups`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('loadNewSessionGroups', payload.data);
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },

        clearSessions({ commit }) {
            commit('clearSessions');
        },

        startSessions({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/start/${sessionGroupId}`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },

        stopSessions({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/stop/${sessionGroupId}`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
        
        cancelSessions({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/cancel/${sessionGroupId}`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
        
        pauseSessions({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        encodeURI(
                            `${
                                process.env.VUE_APP_SOME_FACILITATOR_API
                            }/pause/${sessionGroupId}?notifyParticipants=true`
                        )
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
        
        pauseSessionsNoNotification({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        encodeURI(
                            `${
                                process.env.VUE_APP_SOME_FACILITATOR_API
                            }/pause/${sessionGroupId}?notifyParticipants=false`
                        )
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
        
        unpauseSessions({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/unpause/${sessionGroupId}`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
        
        loadScenarios({ commit }) {
            return new Promise((resolve, reject) => {
                axios
                    .get(
                        `${process.env.VUE_APP_SOME_FACILITATOR_API}/scenarios`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('loadScenarios', payload.data);
                        return resolve(payload);
                    })
                    .catch((payload: AxiosResponse<any>) => {
                        return reject(payload);
                    });
            });
        },

        loadScenario({ commit }) {
            return new Promise((resolve, reject) => {
                axios
                    .get(process.env.VUE_APP_SOME_FACILITATOR_API + '/scenario')
                    .then((payload: AxiosResponse<any>) => {
                        commit('setCurrentScenario', {
                            scenario: { ...payload.data.scenario },
                            events: [...payload.data.events],
                            phases: [...payload.data.phases],
                        });
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },

        loadEditScenario({ commit }, scenarioId: string) {
            return new Promise((resolve, reject) => {
                axios
                    .get(`${process.env.VUE_APP_SOME_SCENARIO_API}/GetScenario/${scenarioId}`)
                    .then((payload: AxiosResponse<any>) => {
                        commit('setCurrentEditScenario', payload.data);
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
        
        loadEvents({ commit }, sessionGroupId: string) {
            return new Promise((resolve, reject) => {
                if (sessionGroupId == null || sessionGroupId === '') {
                    return reject();
                }
                axios
                    .get(
                        process.env.VUE_APP_SOME_FACILITATOR_API +
                            '/GetEvents/' +
                            sessionGroupId
                    )
                    .then((payload: AxiosResponse<any>) => {

                        commit('setEvents', payload.data);
                        return resolve(payload);
                    })
                    .catch(function(error) {
                        console.error(error)
                        console.trace();
                        return reject(error);
                    });
            });
        },

        loadActivity({ commit }, sessionGroupId: string) {
            console.info('Reloading activity');
            return new Promise((resolve, reject) => {
                if (sessionGroupId == null || sessionGroupId === '') {
                    return reject('Missing sessionGroupId');
                }
                axios
                    .get(
                        process.env.VUE_APP_SOME_FACILITATOR_API +
                            '/GetActivityLogs/' +
                            sessionGroupId
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('setActivityLog', payload.data);
                        return resolve(payload);
                    })
                    .catch((payload: AxiosResponse<any>) => {
                        console.error(payload)
                        console.trace();
                        return reject(payload);
                    });
            });
        },

        addMessage({ commit }, message) {
            commit('addMessage', message);
            
            if(message.sessionLog.messageType == 'ScenarioEvent') {
                commit('addNewSessionLog', message.sessionLog);
            }
        },

        addComment({ commit }, comment) {
            commit('addComment', comment);
        },

        addSessionLogAction({ commit }, sessionLogAction) {
            return new Promise((resolve, reject) => {
                axios
                    .post(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/addSessionLogAction`,
                        sessionLogAction
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('addSessionLogAction', sessionLogAction);
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },

        setTypeableCode({ commit }, code) {
            localStorage.setItem('typeableCode', code);
            commit('setTypeableCode', code);
        },

        signIn({ commit }) {
            commit('setSignedIn', true);
        },

        signOut({ commit }) {
            console.log("kommer vi ens hit?")
            localStorage.clear();
            commit('setSignedIn', false);
            
        },
        
        setDuration({ commit }, duration) {
            localStorage.setItem('duration', duration);
            commit('setDuration', duration);
        },

        setCurrentScenario({ commit }, scenario) {
            commit('setCurrentScenario', scenario);
        },
        
        setGroupName({ commit }, groupName) {
            commit('setGroupName', groupName);
        },

        uncoupleSessionGroup({commit}) {
            return axios
                .post(
                    `${
                        process.env.VUE_APP_SOME_FACILITATOR_API
                    }/leave/`
                );
        },

        loadSessions({ commit }) {
            return new Promise((resolve, reject) => {
                axios
                    .get(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/GetSessionGroup/`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit(
                            'setStatus',
                            payload.data === '' ? '' : payload.data.status
                        );
                        commit('setSessionGroupId', payload.data.id );
                        commit('loadSessions', payload.data.sessions || []);
                        commit('setDuration', payload.data.duration || '');
                        commit(
                            'setTypeableCode',
                            payload.data.typeableCode || ''
                        );
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },

        addScenarioEvent({ commit }, event: SessionLog) {
            commit('addScenarioEvent', event);
        },
        
        addActivityLog({ commit }, comment: SessionLog) {
            commit('addActivityLog', comment);
        },

        clearScenarioEvent({ commit }) {
            commit('clearScenarioEvent');
        },
        
        clearActivityLog({ commit }) {
            commit('clearActivityLog');
        },

        setStressLevel({ commit }, stressLevel: number) {
            commit('changeStressLevel', stressLevel);
        },

        changeStressLevel({ commit }, { sessionGroupId, level }) {
            return new Promise((resolve, reject) => {
                axios
                    .get(
                        `${
                            process.env.VUE_APP_SOME_FACILITATOR_API
                        }/SetStressLevel/${sessionGroupId}/${level}`
                    )
                    .then((payload: AxiosResponse<any>) => {
                        commit('changeStressLevel', level);
                        return resolve(payload);
                    })
                    .catch((e) => {
                        console.error(e)
                        console.trace();
                        return reject(e);
                    });
            });
        },
    },
});
