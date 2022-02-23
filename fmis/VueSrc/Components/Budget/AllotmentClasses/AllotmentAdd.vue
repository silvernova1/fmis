<template>
    <div class="modal-content">
        <div class="modal-header">
            <h3 class="grey lighter">{{ modal_title }}</h3>
        </div>
        <form @submit="onSubmitCreate">
            <div class="modal-body">
                <div class="form-group">
                    <h5>Allotment Class:</h5>
                    <input class="form-control" v-model="Allotment_Class" type="text" required />
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
        name: "AllotmentAdd",
        emits: ["add-allotment-class"],
        data() {
            return {
                Allotment_Class: "",
                new_allotment: Object,
                modal_title: "Create new Allotment Class"
            }
        },
        methods: {
            onSubmitCreate(e) {
                e.preventDefault();
                $('#createModal').modal('toggle');

                this.new_allotment = {
                    Allotment_Class: this.Allotment_Class,
                    Account_Code: "0", //the back end will set
                    Fund_Code: "0" //the back end will set
                }

                this.$emit('add-allotment-class', this.new_allotment);
                this.Allotment_Class = "";
            }
        }
    }
</script>