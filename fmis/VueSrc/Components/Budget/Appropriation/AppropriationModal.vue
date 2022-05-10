<template>
    <!-- Modal -->
    <div class="modal fade" id="createModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <appropriation-add @add-appropriation="addAppropriation"></appropriation-add>
        </div>
    </div>

    <div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <allotment-update v-bind:allotment="allotment" @update-allotment-submit="updateAllotment"></allotment-update>
        </div>
    </div>
</template>

<script>
    import AppropriationAdd from './AppropriationAdd.vue'
    import AppropriationUpdate from './AppropriationUpdate.vue'

    export default {
        name: "AppropriationModal",
        emits: ["add-appropriation", "update-appropriation","update-appropriation-submit"],
        components: {
            AppropriationAdd,
            AppropriationUpdate
        },
        props: {
            appropriation: {}
        },
        methods: {
            addAppropriation(appropriation) {
                this.$emit('add-appropriation', appropriation);
            },
            updateAppropriation(payload) {
                this.$emit('update-appropriation-submit', payload);
            }
        }
    }
</script>

<style scoped>
</style>