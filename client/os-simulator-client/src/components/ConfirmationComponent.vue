<template>
    <DialogComponent :open="open" @modalClosed="close" class="confirmation">
        <div class="confirmation dialog-content">
            <div class="card">
                <h3>{{title}}</h3>
                <footer>
                    <button @click="closeAndConfirm">Ja</button>
                    <button @click="close">Nej</button>
                </footer>
            </div>
        </div>
    </DialogComponent>
</template>

<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import DialogComponent from '../components/DialogComponent.vue';
    import Alert from '../Types/Alert';

    @Component({components: {DialogComponent}})
    export default class ConfirmationComponent extends Vue {
        public title: string = '';
        public confirmed: boolean = false;
        private unsubscribe: any;

        get open() {
            return this.$store.state.alert != null;
        }
        
        private closeAndConfirm(){
            this.confirmed = true;
            this.$store.dispatch('closeConfirmAlert');
        }
        
        private close() {
            this.$store.dispatch('closeAlert');
        }

        private created() {
            this.unsubscribe = this.$store.subscribeAction({
                before: (action, state) => {
                    // no action
                },
                after: (action, state) => {
                    switch (action.type) {
                        case 'showAlert':
                            this.title = action.payload.title;
                    }
                },
            });
        }

        private destroyed() {
            this.unsubscribe();
        }
    }
</script>

<style scoped lang="scss">
    @import '../assets/scss/colors';
    @import "../assets/scss/spacings";
    h3{
        padding: 0 $space-xl;
        text-align: center;
        color: $dark-gray2;
    }
    .confirmation{
        color: #000;
        .dialog-content{
            width: 50vw;
            margin-left: 12.5vw;
            .card{
                height: 25vh;
                display: flex;
                flex-flow: column;
                justify-content: center;
                footer{
                    display: flex;
                    width: 100%;
                    justify-content: center;
                    align-items: center;
                    margin-top: $space-sm;
                    button{
                        width: 170px;
                        margin: $space-sm $space-sm/2 $space-sm $space-sm/2;
                        cursor: pointer;
                    }
                }
            }
        }
    
    }

    
</style>