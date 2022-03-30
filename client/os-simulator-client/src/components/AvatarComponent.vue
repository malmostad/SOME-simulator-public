<template>
    <div class="avatar">
        <img alt="" src="" />
    </div>
</template>

<script lang="ts">
    import {Component, Vue} from 'vue-property-decorator'

    @Component({})
    export default class AvatarComponent extends Vue {

    }
</script>

<style lang="scss" scoped>
    @import '../assets/scss/typography';
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';

    .avatar {
        width: 50px;
        height: 50px;
        min-width: 50px;
        margin-right: $space-sm;
        padding: 0 0;
        background-image: url('../assets/circle.svg');
        background-size: 50px 50px;

        img {
            max-width: 100%;
            height: 100%;
            width: 50px;
            border-radius: 50%;
            background-size: contain;
        }
    }
</style>