<template>
    <table class="table table-striped" style="width: 1200px">
        <thead>
            <tr>
                <th>
                    Fund Source
                </th>
                <th>
                    Fund Code (Current)
                </th>
                <th>
                    Fund Code (Conap)
                </th>
                <th>
                    Actions
                </th>
            </tr>
        </thead>
        <tbody :key="fund.FundId" v-for="fund in fund_data">
            <fund :fund="fund"></fund>
        </tbody>
    </table>
</template>
<script>
    import Fund from './Fund'

    export default {
        name: "Funds",
        components: {
            Fund
        },
        props: {
            funds: ""
        },
        data() {
            return {
                fund_data: Array
            }
        },
        created() {
            this.fund_data = JSON.parse(this.funds);
            console.log(this.fund_data);
        }
    }
</script>