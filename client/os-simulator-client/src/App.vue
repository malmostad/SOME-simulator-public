<template>
    <div id="app">
        <router-view />
    </div>
</template>
<script lang="ts">
    import Vue from 'vue';
    export default Vue.extend();
</script>
<style lang="scss">
#app {
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}
#nav {
    padding: 0 0 30px 0;
    a {
        display: inline-block;
        margin: 0 10px 0 10px;
    }
}
</style>
