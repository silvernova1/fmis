<template>
    <appropriation-header></appropriation-header>
    <appropriations v-bind:appropriations="appropriations"></appropriations>
</template>

<script>
    import AppropriationHeader from './AppropriationHeader.vue'
    import Appropriations from './Appropriations.vue'

    export default {
        name: 'AppropriationContent',
        components: {
            AppropriationHeader,
            Appropriations
        },
        data() {
            return {
                modal_content_event: "add" //default button
            }
        },
        props: {
            appropriations: String,
        }
    }
</script>