<template>
    <div class="modal-content">
        <div class="modal-header">
            <h3 class="grey lighter">{{ modal_title }}</h3>
        </div>
        <form @submit="onSubmitCreate">
            <div class="modal-body">
                <div class="form-group">
                    <h5>Appropriation :</h5>
                    <input class="form-control" v-model="AppropriationSource" type="text" required />
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                <button type="submit" class="btn btn-primary">Save changes</button>
            </div>
        </form>
    </div>
</template>

<script>
    export default {
        name: "AppropriationAdd",
        emits: ["add-appropriation"],
        data() {
            return {
                AppropriationSource: "",
                new_appropriation: Object,
                modal_title: "Create new Appropriation"
            }
        },
        methods: {
            onSubmitCreate(e) {
                e.preventDefault();
                $('#createModal').modal('toggle');

                this.new_appropriation = {
                    AppropriationSource: this.AppropriationSource,
                }

                this.$emit('add-appropriation', this.new_appropriation);
                this.AppropriationSource = "";
            }
        }
    }
</script>