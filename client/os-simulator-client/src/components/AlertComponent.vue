<template>
    <DialogComponent :open="open" @modalClosed="close">
        <div class="dialog-content"> 
            <div class="card">
                <h1>{{title}}</h1>
                <p>{{content}}</p>
            </div>

        </div>
    </DialogComponent>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import DialogComponent from '../components/DialogComponent.vue';
import Alert from '../Types/Alert';

@Component({components: {DialogComponent}})
export default class AlertComponent extends Vue {
    public title: string = '';
    public content: string = '';
    private unsubscribe: any;

    get open() {
        return this.$store.state.alert != null;
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
                        this.content = action.payload.content;
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

</style>