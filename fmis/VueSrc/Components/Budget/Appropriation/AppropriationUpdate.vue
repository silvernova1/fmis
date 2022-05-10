<template>
    <div class="modal-content">
        <div class="modal-header">
            <h3 class="grey lighter">{{ modal_title }}</h3>
        </div>
        <form @submit="onSubmitUpdate">
            <div class="modal-body">
                <div class="form-group">
                    <h5>Allotment Class:</h5>
                    <input class="form-control" v-model="allotment.Allotment_Class" name="Allotment_Class" type="text" required />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                <button type="submit" class="btn btn-warning">Update changes</button>
            </div>
        </form>
    </div>
</template>

<script>
    export default {
        name: "AppropriationUpdate",
        emits: ['update-appropriation-submit'],
        data() {
            return {
                modal_title: "Update Appropriation",
                update_appropriation: Object
            }
        },
        props: {
            appropriation: {}
        },
        methods: {
            onSubmitUpdate(e) {
                e.preventDefault();
                $('#updateModal').modal('toggle');

                this.update_appropriation = {
                    Id: this.appropriation.Id,
                    Appropriation: this.appropriation.Appropriation,
                }

                this.$emit('update-appropriation-submit', this.update_appropriation);
                this.Appropriation = "";
            }
        }
    }
</script>