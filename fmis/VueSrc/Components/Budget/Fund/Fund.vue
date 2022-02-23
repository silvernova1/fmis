<template>
    <tr>
        <td>
            {{ fund.Fund_description }}
        </td>
        <td>
            {{ fund.Fund_code_current }}
        </td>
        <td>
            {{ fund.Fund_code_conap }}
        </td>
        <td>
            <a asp-action="Edit" class="btn btn-xs btn-primary" data-toggle="modal" data-target="#EditModal" asp-route-id="">
                <i class="ace-icon fa fa-edit"></i>Edit
            </a> |
            <a class="btn btn-xs btn-danger" href="#" onclick="return confirm('Do you want to delete this Fund Source?');">
                <i class="ace-icon fa fa-trash"></i>Delete
            </a>
        </td>
    </tr>
</template>

<script>
    export default {
        name: "Fund",
        props: {
            fund: Object
        },
        created() {
            console.log(this.fund);
        }
    }
</script>