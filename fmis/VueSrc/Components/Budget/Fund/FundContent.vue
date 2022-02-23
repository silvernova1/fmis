<template>
    <fund-header></fund-header>
    <funds v-bind:funds="funds"></funds>
</template>

<script>
    import FundHeader from './FundHeader.vue'
    import Funds from './Funds.vue'

    export default {
        name: 'FundContent',
        components: {
            FundHeader,
            Funds
        },
        props: {
            funds: ""
        }
    }
</script>