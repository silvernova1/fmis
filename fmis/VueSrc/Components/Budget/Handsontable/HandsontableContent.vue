<script setup lang="ts">
    import { userDetails, obligationData } from "../../../api/index"
    import { computed, ref, onMounted, watch, Ref } from "vue";
    import { HotTable } from '@handsontable/vue3';
    import { ContextMenu } from 'handsontable/plugins/contextMenu';
    import { registerAllModules } from 'handsontable/registry';
    import 'handsontable/dist/handsontable.full.css';

    import moment from "moment"

    //register Handsontable's modules
    registerAllModules();

    const userInfo = ref({
        userid: String,
        fname: String,
        lname: String
    })

    const select_push = ref([])
    const user_connected = ref([])
    const onlineUser = ref([])

    const _userDetails = async () => {
        const response = await userDetails()
        userInfo.value = response
    }
    _userDetails()

    interface MyCustomElement extends HTMLInputElement {
        hotInstance: any; // Change "any" to the appropriate type for your use case
    }

    const hotTableComponent = ref<MyCustomElement | null>(null)
    const socket: Ref<WebSocket> = ref();
    const userInfoLoaded = ref(false)
    const watchUserInfo = computed(() => userInfo.value);
    const urlObject = new URL(window.location.href)
    const domain = urlObject.hostname
    watch(watchUserInfo, (value) => {
        userInfoLoaded.value = true
        const unique_id = userInfo.value.userid + "websocket" + userInfo.value.fname + "websocket" + userInfo.value.lname
        socket.value = new WebSocket("ws://"+domain+":8080", [unique_id.replace(" ", "_")]);
        socket.value.addEventListener('open', (event: Event) => {
            console.log('WebSocket connection established');
        });

        socket.value.addEventListener('close', (event) => {
            console.log(event)
            console.log('WebSocket connection closed');
        });

        socket.value.addEventListener('error', (event) => {
            console.error('WebSocket error.:', event);
        });

        socket.value.addEventListener('message', (event) => {
            const data = JSON.parse(event.data)
            if (data.selectFlag) {
                data['color'] = getHighlightColor(userInfo.value.userid === data.userid ? userInfo.value.userid : data.userid)
                select_push.value.push(data)
                const select_checker = JSON.parse(JSON.stringify(select_push.value))
                if (select_checker[select_checker.length - 2]) {
                    const dataforReset = select_checker[select_checker.length - 2]
                    highlightCellClientReset(dataforReset.row, dataforReset.col, getHighlightColor(data.userid))
                }
                previosUserData(select_checker, 2)
                highlightCellClient(data.row, data.col, getHighlightColor(userInfo.value.userid === data.userid ? userInfo.value.userid : data.userid))
            }
            else if (data.connected_clients) {
                user_connected.value = data.data
                onlineUser.value = data.data.map((item: any, index: any) => {
                    const [userid, fname = "", lname = ""] = item.split("websocket");
                    return {
                        id: index++,
                        userid: userid,
                        fullname: fname + " " + lname,
                        color: getHighlightColor(userid)
                    }
                })
            }

            if (userInfo.value.userid !== data.userid) {
                afterChangeFlag.value = true;
                if (data.afterChange) {
                    changeDataCell(data);
                } else if (data.insertedAbove) {
                    insertAboveRow(data.row);
                } else if (data.insertedBelow) {
                    insertBelowRow(data.row);
                } else if (data.removeRow) {
                    removeRow(data.start, data.end);
                } else if (data.onAfterUndo) {
                    const actionType = data.actionType
                    if (actionType === 'remove_row') {
                        const rowStart = data.index
                        const rowEnd = data.index + data.data.length
                        let count = 0
                        for (var i = rowStart; i < rowEnd; i++) {
                            console.log("row=" + i)
                            insertAboveRow(i)
                            data.data[count].forEach((item: any, column: Number) => {
                                changeDataCell({ row: i, col: column, data: item })
                            })
                            count++
                        }
                    } else if (actionType === 'change') {
                        changeDataCell({ row: data.row, col: data.col, data: data.data })
                    } else if (actionType === 'insert_row') {
                        removeRow(data.row, data.row)
                    } else if (actionType === 'printOrs') {
                        
                    }

                    console.log(data);
                }
            }
            else {
                afterChangeFlag.value = false
            }

        }); 
    })

    const previosUserData = (select_checker: [{userid:String,row:any,col:any}], decrement: any) => {
        const user_clicking = select_checker[select_checker.length - 1].userid
        if (select_checker[select_checker.length - decrement]) {
            const handler_checker = select_checker[select_checker.length - decrement]
            if (handler_checker.userid == user_clicking) {
                highlightCellClientReset(handler_checker.row, handler_checker.col, getHighlightColor(handler_checker.userid))
            } else {
                decrement++;
                previosUserData(select_checker, decrement)
            }
        }
        return;
    }

    const retainHighlightColor = () => {
        const jsonArray: any[] = JSON.parse(JSON.stringify(select_push.value)).reverse();
        const distinctNames: string[] = [...new Set(jsonArray.map((obj: any) => obj.userid))];

        type ResultItem = {
            row: number;
            col: number;
            color: string;
        };

        const resultArray: ResultItem[] = distinctNames.reduce((accumulator: ResultItem[], currentValue: string) => {
            const matchingRows = jsonArray.filter((obj: any) => obj.userid === currentValue);
            if (matchingRows.length > 0) {
                accumulator.push(matchingRows[0]);
            }
            return accumulator;
        }, []);

        resultArray.forEach((item: ResultItem) => {
            highlightCellClient(item.row, item.col, item.color);
        });
    }




    const handsondData = ref([])
           
    const colHeaders = ['FUND SOURCE', 'DATE', 'DV', 'PO #', 'PR #', 'PAYEE', 'ADDRESS', 'PARTICULARS', 'ORS #', 'CREATED BY', 'TOTAL AMOUNT', 'ID', 'OBLIGATION TOKEN',
        'OBLIGATION AMOUNT TOKEN 1', 'EXP CODE 1', 'AMOUNT 1',
        'OBLIGATION AMOUNT TOKEN 2', 'EXP CODE 2', 'AMOUNT 2',
        'OBLIGATION AMOUNT TOKEN 3', 'EXP CODE 3', 'AMOUNT 3',
        'OBLIGATION AMOUNT TOKEN 4', 'EXP CODE 4', 'AMOUNT 4',
        'OBLIGATION AMOUNT TOKEN 5', 'EXP CODE 5', 'AMOUNT 5',
        'OBLIGATION AMOUNT TOKEN 6', 'EXP CODE 6', 'AMOUNT 6',
        'OBLIGATION AMOUNT TOKEN 7', 'EXP CODE 7', 'AMOUNT 7',
        'OBLIGATION AMOUNT TOKEN 8', 'EXP CODE 8', 'AMOUNT 8',
        'OBLIGATION AMOUNT TOKEN 9', 'EXP CODE 9', 'AMOUNT 9',
        'OBLIGATION AMOUNT TOKEN 10', 'EXP CODE 10', 'AMOUNT 10',
        'OBLIGATION AMOUNT TOKEN 11', 'EXP CODE 11', 'AMOUNT 11',
        'OBLIGATION AMOUNT TOKEN 12', 'EXP CODE 12', 'AMOUNT 12',
        'BEGINNING BALANCE', 'REMAINING BALANCE', 'PAP TYPE']

    const hotSettings = ref({

        colWidths: [200, 100, 70, 80, 80, 100, 100, 100, 80, 80, 150, 125, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100, 100,
            100, 100,
            150, 150, 10,
        ],

        columns : [
            {   
                //FundSource
                type: 'dropdown',
              
            },
            {
                //Date
                type: 'date'
            },
            {
                //Dv
                type: 'text'
            },
            {
                //Po#
                type: 'text'
            },
            {
                //Pr#
                type: 'text'
            },
            {
                //Payee
                type: 'text'
            },
            {
                //Address
                type: 'text'
            },
            {
                //Particulars
                type: 'text'
            },
            {
                //Ors#
                type: 'text'
            },
            {
                //CreatedBy
                type: 'text', 
            },
            {
                //TotalAmount
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //Id
                type: 'text'
            },
            {
                //ObligationToken
                type: 'text'
            },
            {
                //ObligationAmountToken1
                type: 'text'
            },
            {
                //ExpenseCode 1
                type: 'dropdown'
            },
            {
                //Amount1
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken2
                type: 'text'
            },
            {
                //ExpenseCode 2
                type: 'dropdown'
            },
            {
                //Amount2
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken3
                type: 'text'
            },
            {
                //ExpenseCode 3
                type: 'dropdown'
            },
            {
                //Amount 3
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken4
                type: 'text'
            },
            {
                //ExpenseCode 4
                type: 'dropdown'
            },
            {
                //Amount 4
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken5
                type: 'text'
            },
            {
                //ExpenseCode 5
                type: 'dropdown'
            },
            {
                //Amount 5
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken6
                type: 'text'
            },
            {
                //ExpenseCode 6
                type: 'dropdown'
            },
            {
                //Amount 6
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken7
                type: 'text'
            },
            {
                //ExpenseCode 7
                type: 'dropdown'
            },
            {
                //Amount 7
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken8
                type: 'text'
            },
            {
                //ExpenseCode 8
                type: 'dropdown'
            },
            {
                //Amount 8
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken9
                type: 'text'
            },
            {
                //ExpenseCode 9
                type: 'dropdown'
            },
            {
                //Amount 9
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken10
                type: 'text'
            },
            {
                //ExpenseCode 10
                type: 'dropdown'
            },
            {
                //Amount 10
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken11
                type: 'text'
            },
            {
                //ExpenseCode 11
                type: 'dropdown'
            },
            {
                //Amount11
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //ObligationAmountToken12
                type: 'text'
            },
            {
                //ExpenseCode 12
                type: 'dropdown'
            },
            {
                //Amount 12
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //Begenning Balance
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //Remaining Balance 
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //Pap Type
                type: 'text'
            },
        ],

        hiddenColumns : {
            columns: [9, 11, 12, 13, 16, 19, 22, 25, 28, 31, 34, 37, 40, 43, 46, 49, 50, 51],
            indicators: true, // show the indicator in the header
        },

        wordWrap: false,
        manualRowResize: true,
        manualColumnResize: true,
        height: '800px',
        //fixedColumnsLeft: 6,
        afterRemoveRow: (index: any, amount: any) => {

        },
        contextMenu: {
            undo: true,
            items:
            {
                'row_above': {
                    name: 'Insert row above',
                    callback: function (changes: any, options: any) {
                        console.log(changes)
                        insertAboveRow(options[0].start.row)
                        const send_data = { row: options[0].start.row, insertedAbove: true, userid: userInfo.value.userid, status: options }
                        socket.value.send(JSON.stringify(send_data));
                    }
                },
                'row_below': {
                    name: 'Insert row below',
                    callback: function (changes: any, options: any) {
                        console.log(changes)
                        insertBelowRow(options[0].start.row)
                        const send_data = { row: options[0].start.row, insertedBelow: true, userid: userInfo.value.userid, status: options }
                        socket.value.send(JSON.stringify(send_data));
                    }                       
                },
                'separator': ContextMenu.SEPARATOR,
                'remove_row': {
                    name: 'Remove row',
                    callback: function (changes: any, options: any) {
                        console.log(options)
                        const start = options[0].start.row
                        const end = options[0].end.row
                        removeRow(start, end)
                        const send_data = { start: start, end: end, removeRow: true, userid: userInfo.value.userid, status: options }
                        socket.value.send(JSON.stringify(send_data));
                    }
                },
                'printOrs': {
                    name: 'Print Ors',
                    callback: function (changes: any, options: any) {                                       
                        console.log(options);  

                        const start = options[0].start.row
                        const end = options[0].end.row
                        const send_data = { start: start, end: end, printOrs: true, userid: userInfo.value.userid, status: options }
                        socket.value.send(JSON.stringify(send_data));

                        var selected = handsondData.value || [];
                        var holder_data = this.hotSettings.value();
                        var first_column = selected[0][0];
                        var last_column = selected[0][2];
                        var many_token = [];

                        for (let j = first_column; j <= last_column; j++) {
                            many_token.push(holder_data[j][12]);
                        }
                        var url = `${this.$router.options.base}/PrintOrs/Obligations?`;
                        for (let i = 0; i < many_token.length; i++) {
                            if ((i + 1) != many_token.length) {
                                url += `token=${many_token[i]}&`;
                            } else {
                                url += `token=${many_token[i]}`;
                            }
                        }
                        window.open(url);

                    }
                },
                'undo': {
                    name: 'Undo',
                    callback: function (changes: any, options: any) {
                        const hot = hotTableComponent.value.hotInstance;
                        var undo = hot.undoRedo && hot.undoRedo.activeStack && hot.undoRedo.activeStack.undoList;
                        if (undo) {
                            console.log(undo);
                        }
                    }
                }
            }
        },
        licenseKey: 'non-commercial-and-evaluation',

    })

    const printOrs = (start: any, end: any) => {
        const cell = hotTableComponent.value.hotInstance
      
        cell.alter('printOrs', start, end + 1);
    }

    const insertAboveRow = (row: Number) => {
        const cell = hotTableComponent.value.hotInstance;
        cell.alter('insert_row_above', row);
    }

    const insertBelowRow = (row: Number) => {
        const cell = hotTableComponent.value.hotInstance;
        cell.alter('insert_row_below', row);
    }

    const removeRow = (start: any, end: any) => {
        const cell = hotTableComponent.value.hotInstance;
        end = end - start
        cell.alter('remove_row', start, end + 1); //2nd param para sa row, 3rd param kung pila ka row ang i delete
    }

    const getHighlightColor = (userid: String) => {
        const highlight_array = ["highlight_yellow", "highlight_pink", "highlight_blue", "highlight_orange", "highlight_green", "highlight_pink", "highlight_gray", "highlight_salmon"]
        let parse_data = JSON.parse(JSON.stringify(user_connected.value))
        parse_data = parse_data.map((item: any) => item.split("websocket")[0])
        const color_pick = highlight_array[parse_data.indexOf(userid.toString())]
        return color_pick
    }

    const highlightCell = () => {
        const selectedCell = hotTableComponent.value.hotInstance.getSelected()[0];
        const send_data = { row: selectedCell[0], col: selectedCell[1], selectFlag: true, userid: userInfo.value.userid }
        socket.value.send(JSON.stringify(send_data));
    }

    const highlightCellClient = (row: Number, col: Number, highlight_color: String) => {
        const cell = hotTableComponent.value.hotInstance;
        try {
            cell.getCell(row, col).classList.add(highlight_color);
        } catch (e) { }
    }

    const highlightCellClientReset = (row: Number, col: Number, highlight_color: String) => {
        const cell = hotTableComponent.value.hotInstance.getCell(row, col);
        try {
            cell.classList.remove(highlight_color);
        } catch (e) { }
    }

    const changeDataCell = (data: any) => {
        const cell = hotTableComponent.value.hotInstance;
        cell.setDataAtCell(data.row, data.col, data.data);
    }

    const afterChangeFlag = ref(false)
    const afterChange = (changes: any, source: any) => {
        if (changes && source === 'edit' && !afterChangeFlag.value) {
            const [row, col, oldValue, newValue] = changes[0];
            const data = hotTableComponent.value.hotInstance.getDataAtCell(row, col);
            const send_data = { row: row, col: col, afterChange: true, userid: userInfo.value.userid, data: data }
            socket.value.send(JSON.stringify(send_data));
        }
    }

    const afterRender = () => {
        retainHighlightColor()
    }

    const afterOnCellMouseDown = (event: any, coords: any, tdElement: any) => {
        if (event.button === 2) {
            //console.log('Right-click detected!');
        }
    }

    const onAfterUndo = (action: any, changes: any) => {
        console.log(action);
        if (action.actionType === 'remove_row') {
            socket.value.send(JSON.stringify({ index: action.index, data: action.data, onAfterUndo: true, actionType: action.actionType, userid: userInfo.value.userid }));
        } else if (action.actionType === 'change') {
            socket.value.send(JSON.stringify({ row: action.selected[0][0], col: action.selected[0][1], data: action.changes[0][2], onAfterUndo: true, actionType: action.actionType, userid: userInfo.value.userid }));
        } else if (action.actionType === 'insert_row') {
            socket.value.send(JSON.stringify({ row: action.index, onAfterUndo: true, actionType: action.actionType, userid: userInfo.value.userid }));
        }
    }

    const ObligationAmountToken = () => {
        const s4 = () => {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }

    const totalObligation = ref(0)
    const __obligationData = async () => {
        const response = await obligationData()
        const data = await Promise.all(response.map(async (item: any) => {
            //console.log(item);
            const amount: number[] = item.obligationAmounts.map((item) => item.amount);
            const obligation_amount: number = amount.reduce((total, amount) => total + amount, 0);

            const obligationAmountBody = item.obligationAmounts.map((obliAmount: any) => {
                return [obliAmount.obligation_amount_token, obliAmount.expense_code, obliAmount.amount]
            })

            const ObligationAmountTokenBody = Array.from({ length: 12 - item.obligationAmounts.length }, () => {
                return [
                    ObligationAmountToken(), //obligation amount token
                    "", // expense code
                    "" // amount
                ];
            });

            const dataBody = [
                "fundsource", //0
                moment(item.date, "YYYY-MM-DD").format('MM/DD/YYYY'), //1
                item.dv, //2
                item.po_no, //3
                item.pr_no, //4
                item.payee, //5
                item.address, //6
                item.particulars, //7
                item.ors_no, //8
                item.created_by, //9
                obligation_amount, //10
                item.id, //11
                item.obligation_token, //12
            ]

            //return dataBody
            return dataBody.concat(...obligationAmountBody, ...ObligationAmountTokenBody)

            totalObligation.value += obligation_amount
        }))

        handsondData.value = data
        console.log(data)
    }
    __obligationData()
</script>


<template>
    <div class="row" style="padding:20px;">
        <h3>Online User's</h3>
        <div style="display:flex;" v-for="online in onlineUser" :key="online.id">
            <div class="online-indicator" :class="online.color">
                <span class="blink" :class="online.color"></span>
            </div>
            <small>{{ online.fullname }}</small>
        </div>
    </div>
    <hot-table ref="hotTableComponent"
               :data="handsondData"
               :colHeaders="colHeaders"
               :colWidth ="true"
               :rowHeaders="true"
               stretchH="all"
               height="auto"
               :settings="hotSettings"
               :afterSelection="highlightCell"
               :afterChange="afterChange"
               :afterRender="afterRender"
               :afterOnCellMouseDown="afterOnCellMouseDown"
               :afterUndo="onAfterUndo"
               v-if="handsondData.length > 0"
          
    >
    </hot-table>
    <h3 v-else>Processing...</h3>
</template>

<style>
    .highlight_yellow {
        background-color: yellow !important;
    }

    .highlight_pink {
        background-color: pink !important;
    }

    .highlight_blue {
        background-color: lightblue !important;
    }

    .highlight_orange {
        background-color: orange !important;
    }

    .highlight_green {
        background-color: lightgreen !important;
    }

    .highlight_gray {
        background-color: lightgray !important;
    }

    .highlight_gray {
        background-color: lightsalmon !important;
    }

    div.online-indicator {
        width: 15px;
        height: 15px;
        margin-right: 10px;
        /* background-color: #0fcc45; */
        border-radius: 50%;
    }

    span.blink {
        display: block;
        width: 15px;
        height: 15px;
        /* background-color: #0fcc45; */
        opacity: 0.7;
        border-radius: 50%;
        animation: blink 1s linear infinite;
    }

    @keyframes blink {
        100% {
            transform: scale(2, 2);
            opacity: 0;
        }
    }
</style>