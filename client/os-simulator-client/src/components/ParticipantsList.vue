<template>
    <div class="participants-list">
        <p class="title">Anslutna deltagare</p>
        <ul v-if="sessions != null && sessions.length > 0">
            <li
                v-for="(session, index) in sessions"
                v-bind:key="session.id"
                @click="select(session.id)"
                v-bind:class="{selected: sessionId === session.id}"
            >
                <span v-bind:class="classObject(index)" class="blob"></span>
                <span class="text">{{ session.participant }}</span>
            </li>
        </ul>
        <p class="waiting" v-else>Ingen deltagare är ansluten</p>
    </div>
</template>
<script lang="ts">
import {Component, Emit, Prop, Vue} from 'vue-property-decorator';
import {Session} from '../Types/Session';

@Component
export default class ParticipantsList extends Vue {
    @Prop({default: Object as () => Session})
    public sessions!: Session[];

    @Prop({default: null})
    public sessionId!: number | null;

    private classObject(index: number): any {
        const color = (index % 6) + 1;
        return {['color' + color]: true};
    }

    @Emit('select')
    public select(id: number | null) {
        return id === this.sessionId ? null : id;
    }
}
</script>

<style lang="scss" scoped>
    @import '../assets/scss/typography';
    @import '../assets/scss/colors';
    @import '../assets/scss/spacings';

    .participants-list {
        padding-top: 40px;
        padding-bottom: 10px;
    }

    p {
        @include typography(regular, 18, $dark-gray1);
        letter-spacing: 1.14px;

        &.title {
            padding-bottom: $space-sm;
            font-weight: 600;
            color: $dark-gray1;
        }

        margin: 0;
        padding: 0;
    }

    ul {
        display: flex;
        flex-direction: row;
        list-style: none;
        padding: 0;
        flex-wrap: wrap;
    }

    li:hover {
        cursor: pointer;
    }

    li {
        min-width: 25%;
        height: 25px;
        display: flex;
        flex-direction: row;
        @include typography(bold, 16, $dark-blue1);
        letter-spacing: 1.14px;
        overflow: hidden;
        padding: 0;
        margin: 0 0 30px 0;
    }

    .text {
        margin-left: 10px;
        height: 25px;
        line-height: 25px;
    }

    li.selected > .blob::after {
        content: '\00a0';
        background-color: $primary-button-background-color;
        border-radius: 50%;
        display: inline-block;
        width: $space-sm;
        height: $space-sm;

        position: relative;
        left: 5px;
        top: 5px;

    }

    .blob {
        width: 25px;
        height: 25px;
        border-radius: 50%;
        flex-shrink: 0;
        flex-grow: 0;
    }

    .color1 {
        background-color: #a1dee9;
    }

    .color2 {
        background-color: #a9dc92;
    }

    .color3 {
        background-color: #f3ec7a;
    }

    .color4 {
        background-color: #fbc34e;
    }

    .color5 {
        background-color: #dedd3a;
    }

    .color6 {
        background-color: #d798bf;
    }
</style>
