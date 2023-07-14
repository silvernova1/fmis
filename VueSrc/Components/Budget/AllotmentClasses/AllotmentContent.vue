<template>
    <allotment-header></allotment-header>
    <allotments v-bind:allotments="allotments"></allotments>
</template>

<script>
    import AllotmentHeader from './AllotmentHeader.vue'
    import Allotments from './Allotments.vue'

    export default {
        name: 'AllotmentContent',
        components: {
            AllotmentHeader,
            Allotments
        },
        data() {
            return {
                modal_content_event: "add" //default button
            }
        },
        props: {
            allotments: String,
        }
    }
</script>