<template>
    
    <div>
        <RadioButton v-for="(session, index) in sessions" class="radiobutton" :checked="selected.some(s => s === session)" @change="select(session)">
            <span v-bind:class="classObject(index)" class="blob"></span>
            <span class="text">{{ session.participant }}</span>
        </RadioButton>
    </div>
    
</template>

<script lang="ts">
    import {Component, Emit, Prop, Vue} from 'vue-property-decorator'
    import {Session} from '@/Types/Session';
    import RadioButton from '@/components/RadioButton.vue';
    
    
    @Component({
        components: {RadioButton}
    })
    export default class ParticipantFilterList extends Vue {
        
        @Prop({default: null})
        private sessions!: Session[];
        
        private selected: Session[] = [];
        
        private classObject(index: number): any {
            const color = (index % 6) + 1;
            return {['color' + color]: true};
        }

        @Emit('change')
        public clear() : Session[] {
            this.selected.splice(0,this.selected.length);
            return this.selected;
        }
        
        @Emit('change')
        public select(session: Session ) {
            
            let index = this.selected.indexOf(session);
            
            if(index >= 0)
                this.selected.splice(index, 1);
            else
                this.selected.push(session);
            
            return this.selected;
            
        }
    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/elements';
    @import '../assets/scss/colors';
    @import '../assets/scss/typography';

    @include color-dots('sm');
    
    .text {
        color: #647088;
        letter-spacing: 1.14px;
        position: relative;
        top: -6px;
        margin-left:14px;
       @include typography(bold,16,$dark-blue1);
    }
    
    div {
        
        
        .radiobutton {
            margin-right: 10px;
        }
        .radiobutton:last-child {
            margin-right: 0px;
        }
    }
    
    
</style>