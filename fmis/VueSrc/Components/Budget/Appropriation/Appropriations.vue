<template>
    <appropriation-modal v-bind:appropriation="appropriation" @add-appropriation="addAppropriation" @update-appropriation-submit="updateAppropriationSubmit"></appropriation-modal>
    <table class="table table-striped" style="width:900px;">
        <thead>
            <tr>
                <th>
                    Appropriation Source
                </th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody :key="appropriation.Id" v-for="appropriation in appropriation_data">
            <appropriation :appropriation="allotment" @update-appropriation="updateAppropriation" @delete-appropriation="deleteAppropriation"></appropriation>
        </tbody>
    </table>
</template>

<script>
    import Appropriation from './Appropriation'
    import AppropriationModal from './Appropriation'
    import axios from 'axios'

    export default {
        name: "Appropriations",
        components: {
            Appropriation,
            AppropriationModal
        },
        props: {
            appropriations: String //supposedly this is array, to accept the JSON parse, we will declare it as string as default
        },
        data() {
            return {
                appropriations_data: Array,
                appropriation: {}
            }
        },
        methods: {
            addAppropriation(appropriation) {
                let self = this;
                axios.post('Appropriation/Create', appropriation)
                    .then(response => {
                        appropriation.Id = response.data.id;
                        appropriation.AppropriationSource = response.data.AppropriationSource;
                        self.appropriation_data = [...self.appropriation_data, appropriation];
                        console.log(response.data);
                        console.log(self.appropriation_data);
                        Lobibox.notify("success", {
                            sound: false,
                            msg: 'Successfully added allotment class!'
                        });
                    })
                    .catch(error => {
                        return Promise.reject(error);
                    });
            },
            deleteAppropriation(id) {
                this.appropriation_data = this.appropriation_data.filter((appropriation) => appropriation.Id !== id);
                Lobibox.notify("warning", {
                    sound: false,
                    msg: 'Appropriation deleted!'
                });
                axios.delete(`Appropriation/Delete/${id}`)
                    .then(response => {
                    })
                    .catch(error => {
                        return Promise.reject(error);
                    });
            },
            updateAppropriation(payload) {
                this.appropriation = payload
            },
            updateAppropriationSubmit(payload) {
                Lobibox.notify("success", {
                    sound: false,
                    msg: 'Successfully updated appropriation!'
                });
                axios.post('Appropriation/Edit', payload)
                    .then(response => {

                    })
                    .catch(error => {
                        return Promise.reject(error);
                    });
            }
        },
        created() {
            this.appropriations_data = JSON.parse(this.appropriations)
        }
    }
</script>