<template>
    <tr>
        <td>
            {{ allotment.Allotment_Class }}
        </td>
        <td>
            {{ allotment.Account_Code }}
        </td>
        <td>
            {{ allotment.Fund_Code }}
        </td>
        <td>
            <a href="#" data-toggle="modal" data-target="#updateModal" class="btn btn-primary btn-xs" @click="onUpdate(allotment)">
                <i class="ace-icon fa fa-edit"></i>Edit
            </a> |
            <button class="btn btn-danger btn-xs" @click="onDelete(allotment.Id)">
                <i class="ace-icon fa fa-trash"></i>Delete
            </button>
        </td>
    </tr>
</template>

<script>
    export default {
        name: "Allotment",
        emits: ['update-allotment-class','delete-allotment-class'],
        props: {
            allotment: Object,
        },
        methods: {
            onDelete(id) {
                let self = this;
                Lobibox.confirm({
                    msg: "Are you sure you want to delete this Allotment Class?",
                    callback: function ($this, feedback) {
                        if (feedback == "yes")
                            self.$emit('delete-allotment-class', id);
                    }
                });
            },
            onUpdate(payload) {
                this.$emit('update-allotment-class',payload)
            }
        }
    }
</script>
