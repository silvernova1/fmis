<template>
    <tr>
        <td>
            {{ appropriation.AppropriationSource }}
        </td>
        <td>
            <a href="#" data-toggle="modal" data-target="#updateModal" class="btn btn-primary btn-xs" @click="onUpdate(appropriation)">
                <i class="ace-icon fa fa-edit"></i>Edit
            </a> |
            <button class="btn btn-danger btn-xs" @click="onDelete(appropriation.Id)">
                <i class="ace-icon fa fa-trash"></i>Delete
            </button>
        </td>
    </tr>
</template>

<script>
    export default {
        name: "Appropriation",
        emits: ['update-appropriation','delete-appropariation'],
        props: {
            appropriation: Object,
        },
        methods: {
            onDelete(id) {
                let self = this;
                Lobibox.confirm({
                    msg: "Are you sure you want to delete this Appropriation?",
                    callback: function ($this, feedback) {
                        if (feedback == "yes")
                            self.$emit('delete-appropriation', id);
                    }
                });
            },
            onUpdate(payload) {
                this.$emit('update-approrpiation',payload)
            }
        }
    }
</script>
