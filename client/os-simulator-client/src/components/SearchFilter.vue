<template>
    <section>
        <div class="input-container">
            <input class="large" type="text" @input="textChange" v-model="searchFilterData.text"/>
            <span class="search-icon"></span>
        </div>
        <div class="radiobox-container">
            <span>Sök inlägg i:</span>
            <radio-button @change="filterFlowChange(MessageFlow.Short)" :checked="(searchFilterData.messageFlow & MessageFlow.Short) === MessageFlow.Short">Kwitter </radio-button>
            <radio-button @change="filterFlowChange(MessageFlow.Long)" :checked="(searchFilterData.messageFlow & MessageFlow.Long) === MessageFlow.Long">Firendzone</radio-button>
        </div>
        
    </section>
    
</template>

<script lang="ts">
    import {mapState} from 'vuex';
    import {Component, Emit, Vue} from "vue-property-decorator";
    import RadioButton from "@/components/RadioButton.vue";
    import {EditSearchFilterData} from "@/services/EditSearchFilterService";
    import {MessageFlow} from "@/Types/MessageFlow";

    @Component({
        components: {RadioButton},
        computed: {...mapState(['user']), MessageFlow : () => MessageFlow },
    })
    export default class EditSearchFilterComponent extends Vue {
        private searchFilterData : EditSearchFilterData = new EditSearchFilterData();
        
        @Emit('change')
        private filterFlowChange (flow: MessageFlow) {
         
            if( (this.searchFilterData.messageFlow & flow) === flow ) {
                this.searchFilterData.messageFlow -= flow;
            }
            else {
                this.searchFilterData.messageFlow += flow;
            }
            return this.searchFilterData;
        }
        
        @Emit('change')
        private textChange() {
            return this.searchFilterData;
        }
    }
    
</script>

<style lang="scss" scoped>
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';
    @import '../assets/scss/typography';
    
    section {
        margin-top: 35px;
        margin-bottom: 85px;
    }
    
    div.radiobox-container{
        span {
            margin-right: $space-sm;
        }
        * {
            margin-right: $space-md;
        }
    }
    
    .input-container {
        margin-bottom: 20px;
        position: relative;
        padding: 0;

    }
    
    input[type=text] {
        border-radius: 2px;
        width: 100%;
        margin: 0;
        padding-right: 90px;
    }
    .search-icon {
        width: 28px;
        height: 28px;
        right: 50px;
        top: 11px;
        position: absolute;
        display: inline-block;
        
        background-size: 20px;
        background-repeat: no-repeat;
        background-position-y: center;
        background-position-x: center;
        background-image: url('../assets/icons/search.svg');
    }
    
</style>