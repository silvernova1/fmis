<template>
    <!-- Modal -->
    <div class="modal fade" id="createModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <allotment-add @add-allotment-class="addAllotment"></allotment-add>
        </div>
    </div>

    <div class="modal fade" id="updateModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <allotment-update v-bind:allotment="allotment" @update-allotment-submit="updateAllotment"></allotment-update>
        </div>
    </div>
</template>

<script>
    import AllotmentAdd from './AllotmentAdd.vue'
    import AllotmentUpdate from './AllotmentUpdate.vue'

    export default {
        name: "AllotmentModal",
        emits: ["add-allotment-class", "update-allotment-class","update-allotment-submit"],
        components: {
            AllotmentAdd,
            AllotmentUpdate
        },
        props: {
            allotment: {}
        },
        methods: {
            addAllotment(allotment) {
                this.$emit('add-allotment-class', allotment);
            },
            updateAllotment(payload) {
                this.$emit('update-allotment-submit', payload);
            }
        }
    }
</script>

<style scoped>

</style>