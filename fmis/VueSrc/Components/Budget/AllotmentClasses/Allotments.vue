<template>
    <allotment-modal v-bind:allotment="allotment" @add-allotment-class="addAllotment" @update-allotment-submit="updateAllotmentSubmit"></allotment-modal>
    <table class="table table-striped" style="width:900px;">
        <thead>
            <tr>
                <th>
                    Allotment Class
                </th>
                <th>
                    Account Code
                </th>
                <th>
                    Fund Code
                </th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody :key="allotment.Id" v-for="allotment in allotment_data">
            <allotment :allotment="allotment" @update-allotment-class="updateAllotment" @delete-allotment-class="deleteAllotment"></allotment>
        </tbody>
    </table>
</template>

<script>
    import Allotment from './Allotment'
    import AllotmentModal from './AllotmentModal'
    import axios from 'axios'

    export default {
        name: "Allotments",
        components: {
            Allotment,
            AllotmentModal
        },
        props: {
            allotments: String //supposedly this is array, to accept the JSON parse, we will declare it as string as default
        },
        data() {
            return {
                allotment_data: Array,
                allotment: {}
            }
        },       
        methods: {
            addAllotment(allotment) {
                console.log(allotment);
                //axios.post('AllotmentClasses/Create', JSON.parse(JSON.stringify(allotment)))
                let self = this;
                axios.post('AllotmentClasses/Create', allotment)
                    .then(response => {
                        allotment.Account_Code = response.data.account_Code;
                        allotment.Fund_Code = response.data.fund_Code;
                        self.allotment_data = [...self.allotment_data, allotment];
                        Lobibox.notify("success", {
                            sound: false,
                            msg: 'Successfully added allotment class!'
                        });
                        console.log(response.data);
                    })
                    .catch(error => {
                        return Promise.reject(error);
                    });
            },
            deleteAllotment(id) {
                this.allotment_data = this.allotment_data.filter((allotment) => allotment.Id !== id);
                Lobibox.notify("warning", {
                    sound: false,
                    msg: 'Allotment Class deleted!'
                });
                axios.delete(`AllotmentClasses/Delete/${id}`)
                    .then(response => {
                        console.log(response.data);
                    })
                    .catch(error => {
                        return Promise.reject(error);
                    });
            },
            updateAllotment(payload) {
                this.allotment = payload
            },
            updateAllotmentSubmit(payload) {
                Lobibox.notify("success", {
                    sound: false,
                    msg: 'Successfully updated allotment class!'
                });
                axios.post('AllotmentClasses/Edit', payload)
                    .then(response => {

                    })
                    .catch(error => {
                        return Promise.reject(error);
                    });
            }
        },
        created() {
            this.allotment_data = JSON.parse(this.allotments)
        }
    }
</script>