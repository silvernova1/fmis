<template>
    <h3 class="grey lighter smaller">
        {{ title }}
    </h3>
    <hr>
    <p>
        <a asp-action="Create" class="btn btn-success btn-sm" data-toggle="modal" data-target="#createModal">
            <i class="ace-icon fa fa-plus "></i>Create New
        </a>
    </p>
    <br />

</template>

<script>
    export default {
        name: 'AllotmentHeader',
        data() {
            return {
                title : "Allotment Class AGAWN NGA RAMENE"
          }
        }
    }
</script>
