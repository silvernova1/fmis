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
        name: "AllotmentUpdate",
        emits: ['update-allotment-submit'],
        data() {
            return {
                modal_title: "Update Allotment Class",
                update_allotment: Object
            }
        },
        props: {
            allotment: {}
        },
        methods: {
            onSubmitUpdate(e) {
                e.preventDefault();
                $('#updateModal').modal('toggle');

                this.update_allotment = {
                    Id: this.allotment.Id,
                    Allotment_Class: this.allotment.Allotment_Class,
                    Account_Code: this.allotment.Account_Code,
                    Fund_Code: this.allotment.Fund_Code,
                    Desc: this.allotment.Desc
                }

                this.$emit('update-allotment-submit', this.update_allotment);
                this.Allotment_Class = "";
            }
        }
    }
</script>