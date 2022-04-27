<template>
    <!-- CREATE MODAL -->
    <div class="modal fade" id="CreateModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">

            </div>
        </div>
    </div>

    <h3 class="grey lighter smaller">
        {{ title }}
    </h3>
    <hr>
    <p>
        <a asp-action="Create" class="btn btn-success btn-sm" data-toggle="modal" data-target="#CreateModal">
            <i class="ace-icon fa fa-plus "></i>Create New
        </a>
    </p>
    <br />
</template>

<script>
    export default {
        name: 'FundHeader',
        data() {
            return {
                title: "Fund Source & Fund Code "
            }
        }
    }
</script>

<style scoped>
</style>