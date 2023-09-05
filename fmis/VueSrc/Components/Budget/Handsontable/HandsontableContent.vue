<script setup lang="ts">
    import { userDetails, obligationData, fundSub, expCode, saveObligation, saveObligationAmount, deleteObligation, getRemainingObligated, calculateObligatedAmount } from "../../../api/index"
    import { computed, ref, onMounted, watch, Ref } from "vue";
    import { HotTable } from '@handsontable/vue3';
    import Handsontable from 'handsontable';
    import { ContextMenu } from 'handsontable/plugins/contextMenu';
    import { registerAllModules } from 'handsontable/registry';
    import 'handsontable/dist/handsontable.full.css';
    import axios from 'axios';
    import moment from "moment"
    import exp from "constants";

    import { useToast } from 'vue-toast-notification';
    import 'vue-toast-notification/dist/theme-sugar.css';

    
    //register Handsontable's modules
    registerAllModules();

    const select_push = ref([])
    const user_connected = ref([])
    const onlineUser = ref([])

    const props = defineProps({
        xsrf: {
            type: String,
            xsrf: null,
        }
    });

    const userInfo = ref({
        userid: String,
        fname: String,
        lname: String,
        color: String
    })

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
    const $toast = useToast();

    const watchHotTableComponent = computed(() => hotTableComponent.value);
    watch(watchHotTableComponent, (value) => {
        const cellValue = hotTableComponent.value.hotInstance.getDataAtCol(8);
        console.log(totalRowFetchFromBackend.value);
        let ors_number: any = incrementORS(cellValue);
        for (let i = totalRowFetchFromBackend.value; i < 10000; i++) {
            handsondData.value[i][8] = ors_number.toString().padStart(4, '0');
            ors_number++;
        }
    });

    watch(watchUserInfo, (value) => {
        userInfoLoaded.value = true
        const unique_id = userInfo.value.userid + "websocket" + userInfo.value.fname + "websocket" + userInfo.value.lname + "websocket" + userInfo.value.color
        socket.value = new WebSocket("ws://" + domain + ":8080", [unique_id.replace(" ", "_")]);
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

        socket.value.addEventListener('message', async (event) => {
            const data = JSON.parse(event.data)
            if (data.selectFlag) {
                //data['color'] = getHighlightColor(userInfo.value.userid === data.userid ? userInfo.value.userid : data.userid)
                data['color'] = userInfo.value.userid === data.userid ? userInfo.value.color : data.color
                select_push.value.push(data)
                const select_checker = JSON.parse(JSON.stringify(select_push.value))
                if (select_checker[select_checker.length - 2]) {
                    const dataforReset = select_checker[select_checker.length - 2]
                    highlightCellClientReset(dataforReset.row, dataforReset.col, getHighlightColor(data.userid))
                }
                previosUserData(select_checker, 2)
                highlightCellClient(data.row, data.col, userInfo.value.userid === data.userid ? userInfo.value.color : data.color)
                //highlightCellClient(data.row, data.col, getHighlightColor(userInfo.value.userid === data.userid ? userInfo.value.userid : data.userid))
            }
            else if (data.connected_clients) {
                user_connected.value = data.data
                onlineUser.value = data.data.map((item: any, index: any) => {
                    const [userid, fname = "", lname = "", color = ""] = item.split("websocket");
                    return {
                        id: index++,
                        userid: userid,
                        fullname: fname + " " + lname,
                        //color: getHighlightColor(userid)
                        color: color
                    }
                })
            }

            if (userInfo.value.userid !== data.userid) {
                afterChangeFlag.value = true;
                if (data.afterChange) {
                    await changeDataCell(data);
                } else if (data.insertedAbove) {
                    insertAboveRow(data.row,data.data);
                    const send_data = { row: data.row , col: 12, data: data.data }
                    await changeDataCell(send_data);
                } else if (data.insertedBelow) {
                    insertBelowRow(data.row, data.data);
                    //need the same token to all users
                    const send_data = { row: data.row+1, col: 12, data: data.data }
                    await changeDataCell(send_data);
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
                            insertAboveRow(i, data.data)
                            data.data[count].forEach((item: any, column: number) => {
                                changeDataCell({ row: i, col: column, data: item })
                            })
                            count++
                        }
                    } else if (actionType === 'change') {
                        await changeDataCell({ row: data.row, col: data.col, data: data.data })
                    } else if (actionType === 'insert_row') {
                        removeRow(data.row, data.row)
                    }
                    console.log(data);
                }
            }
            else {
                afterChangeFlag.value = false
            }

        });
    })

    interface Checker {
        userid: string;
        row: number;
        col: number;
        color?: string; 
    }

    const previosUserData = (select_checker: Checker[], decrement: number) => {
        const user_clicking = select_checker[select_checker.length - 1].userid
        if (select_checker[select_checker.length - decrement]) {
            const handler_checker = select_checker[select_checker.length - decrement]
            if (handler_checker.userid == user_clicking) {
                highlightCellClientReset(handler_checker.row, handler_checker.col, handler_checker.color)
                //highlightCellClientReset(handler_checker.row, handler_checker.col, getHighlightColor(handler_checker.userid))
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

    function generateUniqueToken(length: number): string {
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        for (let i = 0; i < length; i++) {
            const randomIndex = Math.floor(Math.random() * characters.length);
            result += characters.charAt(randomIndex);
        }
        return result;
    }

    const handsondData = ref([])
    /*const watchHandsonddata = computed(() => handsondData.value);
    const executeWithInterval = (length, index) => {
        if (index < length) {
            handsondData.value[index][3] = generateUniqueToken(4);
            console.log("executing...");
            setTimeout(() => executeWithInterval(length, index + 1), 2000);
        }
    }
    watch(watchHandsonddata, (newData) => {
        executeWithInterval(handsondData.value.length, 0);
    })*/

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
        'BEGINNING BALANCE', 'REMAINING BALANCE', 'PAP TYPE', 'OBLIGATED BALANCE']

    const totalAmountReadOnly = 9;

    const debounceFnWrapper = (colIndex: number, event: Event) => {
        const debounceFn: () => void = Handsontable.helper.debounce(() => {
            const filtersPlugin = hotTableComponent.value.hotInstance.getPlugin('filters');
            filtersPlugin.removeConditions(colIndex);
            filtersPlugin.addCondition(colIndex, 'contains', [event.target['value']]);
            filtersPlugin.filter();
        }, 1000);

        debounceFn(); // Call the actual debounce function inside the wrapper.
    };


    const addEventListeners = (input, colIndex) => {
        input.addEventListener('keydown', event => {
            debounceFnWrapper(colIndex, event);
        });
    };

    // Build elements which will be displayed in header.
    const getInitializedElements = colIndex => {
        const div = document.createElement('div');
        const input = document.createElement('input');

        div.className = 'filterHeader';

        addEventListeners(input, colIndex);

        div.appendChild(input);

        return div;
    };

    // Add elements to header on `afterGetColHeader` hook.
    const addInput = (col, TH) => {
        // Hooks can return a value other than number (for example `columnSorting` plugin uses this).
        if (typeof col !== 'number') {
            return col;
        }

        if (col >= 0 && TH.childElementCount < 2) {
            TH.appendChild(getInitializedElements(col));
        }
    };

    const insertedRowFlag = ref(false);
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
            100, 100, 100,100
        ],


        columns: [
            {
                //FundSource
                type: 'dropdown',
                async source(query, callback) {
                    const response = await __fundSub()
                    callback(response)
                }
            },
            {
                //Date
                type: 'date',
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
                type: 'text',
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
                readOnly: function (column) {
                    return column === totalAmountReadOnly;
                },
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    console.log(data)
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
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
                type: 'dropdown',
                async source(query, callback) {
                    const data = hotTableComponent.value.hotInstance.getDataAtCell(this.row, 51);
                    if (data) {
                        callback(await __expCode(data))
                    }
                }
            },
            {  //48
                //Amount 12
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //49
                //Begenning Balance
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            {
                //50
                //Remaining Balance
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
            
            {
                //51
                //ALLOTMENT CLASS
                type: 'text'
            },
            {
                //52
                //Obligated Balance
                type: 'numeric',
                numericFormat: {
                    pattern: '0,0.00',
                }
            },
        ],
        //SET MINIMUM ROW
        minRows: 10000,
        hiddenColumns: {
            columns: [9, 11, 12, 13, 16, 19, 22, 25, 28, 31, 34, 37, 40, 43, 46, 49, 50, 51, 52 ],
            indicators: true, // show the indicator in the header
        },
        wordWrap: false,
        manualRowResize: true,
        manualColumnResize: true,
        height: '600px',
        filters: true,
        //fixedColumnsStart: 6,
        afterGetColHeader: addInput,
        beforeOnCellMouseDown(event, coords) {
            // Deselect the column after clicking on input.
            if (coords.row === -1 && event.target.nodeName === 'INPUT') {
                event.stopImmediatePropagation();
                this.deselectCell();
            }
        },
        afterFilter: () => {
            const cellValue: any[][] = hotTableComponent.value.hotInstance.getData();
            const totalSumObligation: number = cellValue.reduce((acc: number, curr: any[]) => acc + curr[10], 0);
            totalObligation.value = totalSumObligation;
        },
        contextMenu: {
            undo: true,
            items:
            {
                'row_above': {
                    name: 'Insert row above',
                    callback: function (changes: any, options: any) {
                        console.log(changes)
                        const obligation_token_get = ObligationAmountToken();
                        insertAboveRow(options[0].start.row, obligation_token_get)
                        const send_data = { row: options[0].start.row, insertedAbove: true, userid: userInfo.value.userid, status: options, data: obligation_token_get }
                        socket.value.send(JSON.stringify(send_data));
                    }
                },
                'row_below': {
                    name: 'Insert row below',
                    callback: function (changes: any, options: any) {
                        const obligation_token_get = ObligationAmountToken();
                        insertBelowRow(options[0].start.row, obligation_token_get);
                        const send_data = { row: options[0].start.row, insertedBelow: true, userid: userInfo.value.userid, status: options, data: obligation_token_get }
                        socket.value.send(JSON.stringify(send_data));
                    }
                },
                'separator': ContextMenu.SEPARATOR,
                'remove_row': {
                    name: 'Remove row',
                    callback: async function (changes: any, options: any) {
                        const start = options[0].start.row
                        const end = options[0].end.row
                        const many_token = [];
                        for (let j = start; j <= end; j++) {
                            many_token.push({
                                many_token: handsondData.value[j][12]
                            })
                        }

                        await removeRow(start, end)
                        await __deleteObligation(many_token)
                        const send_data = { start: start, end: end, removeRow: true, userid: userInfo.value.userid, status: options }
                        $toast.error('Successfully Deleted!');
                        socket.value.send(JSON.stringify(send_data));
                    }
                },
                'print_ors': {
                    name: 'Print Ors',
                    callback: function (key: any, options: any) {
                        console.log(options);
                        const start = options[0].start.row
                        const end = options[0].end.row

                        const baseUrl = '/Obligations/PrintOrs' + "?";
                        const queryParams = [];
                        for (let j = start; j <= end; j++) {
                            queryParams.push(`token=${handsondData.value[j][12]}`);
                        }

                        const url = `${baseUrl}${queryParams.join('&')}`;
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

    const incrementORS = (array) => {
        // Find the maximum value in the array
        const maxValue = array.reduce((prevValue, currValue) => {
            const prevNum = parseInt(prevValue);
            const currNum = parseInt(currValue);
            return isNaN(prevNum) || currNum > prevNum ? currValue : prevValue;
        });

        // Extract the numeric part from the max value
        const numericPart = parseInt(maxValue);

        // Check if the last character is a letter
        const lastChar = maxValue[maxValue.length - 1];
        if (isNaN(numericPart) && lastChar >= 'A' && lastChar <= 'Z') {
            // Increment the character
            const nextChar = String.fromCharCode(lastChar.charCodeAt(0) + 1);
            const incrementedValue = maxValue.slice(0, -1) + nextChar;
            return incrementedValue; // Output: '0024B'
        } else {
            return parseInt(maxValue) + 1; // Output: '0024'
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

    const tokens = [13, 16, 19, 22, 25, 28, 31, 34, 37, 40, 43, 46];
    const insertAboveRow = (row: Number, obligation_token: String) => {
        insertedRowFlag.value = true

        const cell = hotTableComponent.value.hotInstance;

        const today = new Date();
        const dd = String(today.getDate()).padStart(2, '0');
        const mm = String(today.getMonth() + 1).padStart(2, '0');
        const yyyy = today.getFullYear();
        const dateToday = dd + '/' + mm + '/' + yyyy;
        const address = "CEBU CITY";
        cell.alter('insert_row_above', row);

        const ors_number = incrementORS(cell.getDataAtCol(8)).toString().padStart(4, '0');
        const rows = row.valueOf() 
        const autoFill = [
            [rows, 1, dateToday],
            [rows, 6, address],
            [rows, 8, ors_number],
            [rows, 12, obligation_token],
            ...tokens.map(token => [rows, token, ObligationAmountToken()]),
        ];
          
        cell.setDataAtCell(autoFill);
    }

    const insertBelowRow = (row: Number, obligation_token: String) => {
        insertedRowFlag.value = true


        const cell = hotTableComponent.value.hotInstance;
        const ors_number = incrementORS(cell.getDataAtCol(8)).toString().padStart(4, '0');

        const today = new Date();
        const dd = String(today.getDate()).padStart(2, '0');
        const mm = String(today.getMonth() + 1).padStart(2, '0');
        const yyyy = today.getFullYear();
        const dateToday = dd + '/' + mm + '/' + yyyy;
        const address = "CEBU CITY";

        cell.alter('insert_row_below', row);

        const rows = row.valueOf() + 1

        const autoFill = [
            [rows, 1, dateToday],
            [rows, 6, address],
            [rows, 8, ors_number],
            [rows, 12, obligation_token],
            ...tokens.map(token => [rows, token, ObligationAmountToken()]),
        ];

        cell.setDataAtCell(autoFill);
    }

    const removeRow = (start: any, end: any) => {
        const cell = hotTableComponent.value.hotInstance;
        end = end - start
        cell.alter('remove_row', start, end + 1); //2nd param para sa row, 3rd param kung pila ka row ang i delete
    }

    const getHighlightColor = (userid: String) => {
        const highlight_array = ["highlight_yellow", "highlight_pink", "highlight_blue", "highlight_orange", "highlight_green", "highlight_gray", "highlight_salmon", "highlight_seagreen", "highlight_lightyellow"]
        let parse_data = JSON.parse(JSON.stringify(user_connected.value))
        parse_data = parse_data.map((item: any) => item.split("websocket")[0])
        const color_pick = highlight_array[parse_data.indexOf(userid.toString())]
        return color_pick
    }

    const highlightCell = (source: any) => {
        console.log(source)
        const selectedCell = hotTableComponent.value.hotInstance.getSelected()[0];
        const send_data = { row: selectedCell[0], col: selectedCell[1], selectFlag: true, userid: userInfo.value.userid, color: userInfo.value.color }
        try {
            displayCellTop.value = handsondData.value[selectedCell[0]][selectedCell[1]]
            socket.value.send(JSON.stringify(send_data));
        } catch (e) {
        }
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

    interface CellData {
        row: number;
        col: number;
        data: any; // Replace 'any' with the specific data type you expect to handle in the cells
    }

    const changeDataCell = async (data: any) => {
        handsondData.value[data.row][data.col] = data.data;
    }

    const checkTheColumnToken = (row: number, column: number) => {
        const cellValue = handsondData.value[row][column]?.trim() ?? '';
        if (!cellValue) {
            const token = ObligationAmountToken();
            handsondData.value[row][column] = token;
            if (column === 12) {
                const send_data = { row: row, col: column, afterChange: true, userid: userInfo.value.userid, data: token }
                socket.value.send(JSON.stringify(send_data));
            }
        }
    }

    const addTokenIfColumnIsEmpty = (row: number) => {
        checkTheColumnToken(row, 12);
        for (const column of tokens) {
            checkTheColumnToken(row,column)
        }
    }

    const displayCellTop = ref("")
    const afterChangeFlag = ref(false)

    const afterChange = async (changes: any, source: any) => {
        //if (changes && source === 'edit' && !afterChangeFlag.value) {

        if (changes && source === 'edit') {

            const [row, col, oldValue, newValue] = changes[0];
            const hot = hotTableComponent.value.hotInstance;

            if (insertedRowFlag.value) {
                insertedRowFlag.value = false;
                return;
            }

            if (!hot.getDataAtCell(row, 0)) {
                alert("Please Select FundSource or Sub Allotment");
                return;
            }
            addTokenIfColumnIsEmpty(row)
            if (col >= 14 && col <= 48 && hot.getDataAtCell(row, 0)) {
                let obligation_amount_data = new FormData()
                obligation_amount_data.append('obligationId', hot.getDataAtCell(row, 11));
                obligation_amount_data.append('obligation_token', hot.getDataAtCell(row, 12));
                if (gridExpenseAmountToken().find(item => item[0] === col)[1] === 'expense') {
                    obligation_amount_data.append('expense_code', hot.getDataAtCell(row, col));
                    obligation_amount_data.append('obligation_amount_token', hot.getDataAtCell(row, col - 1));
                } else if (gridExpenseAmountToken().find(item => item[0] === col)[1] === 'amount') {
                    if (!hot.getDataAtCell(row, col - 1)) {
                        alert("please select expense code!")
                        return;
                    }
                    const send_check_remaining = {
                        "obligation_id": 0,
                        "obligation_token": hot.getDataAtCell(row, 12),
                        "beginning_balance": 0,
                        "remaining_balance": 0,
                        "obligated_amount": newValue
                    };
                    const checkRemaining = await __getRemainingObligated(send_check_remaining);
                    console.log(checkRemaining);
                    if (checkRemaining['remaining_balance'] + oldValue >= newValue || checkRemaining['remaining_balance'] >= newValue) {
                        let calculated_remaining = 0;
                        let calculated_obligated = 0;
                        if (newValue > oldValue) {
                            calculated_remaining = checkRemaining['remaining_balance'] - (newValue - oldValue);
                            calculated_obligated = checkRemaining['obligated_amount'] + (newValue - oldValue);
                        } else if (newValue < oldValue) {
                            calculated_remaining = checkRemaining['remaining_balance'] + (oldValue - newValue);
                            calculated_obligated = checkRemaining['obligated_amount'] - (oldValue - newValue);
                        }
                        handsondData.value[row][50] = calculated_remaining;
                        handsondData.value[row][52] = calculated_obligated;
                        const send_calculated = {
                            "obligation_id": 0,
                            "obligation_token": hot.getDataAtCell(row, 12),
                            "obligation_amount_token": hot.getDataAtCell(row, col - 2),
                            "remaining_balance": calculated_remaining,
                            "obligated_amount": calculated_obligated,
                            "amount": newValue
                        }
                        console.log(send_calculated);
                        const check = await __calculateObligatedAmount(send_calculated);
                        console.log(check)
                        obligation_amount_data.append('expense_code', hot.getDataAtCell(row, col - 1));
                        obligation_amount_data.append('amount', hot.getDataAtCell(row, col));
                        obligation_amount_data.append('obligation_amount_token', hot.getDataAtCell(row, col - 2));
                        handsondData.value[row][10] = newValue > oldValue ? handsondData.value[row][10] + (newValue - oldValue) : handsondData.value[row][10] - (oldValue - newValue);
                        const send_data = { row: row, col: 10, afterChange: true, userid: userInfo.value.userid, data: handsondData.value[row][10] }
                        socket.value.send(JSON.stringify(send_data));
                    }
                    else {
                        console.log("insufficient");
                        alert("insufficient balance!");
                        handsondData.value[row][col] = oldValue;
                        return;
                    }
                }

                const send_data = { row: row, col: col, afterChange: true, userid: userInfo.value.userid, data: newValue }
                socket.value.send(JSON.stringify(send_data));

                await __saveObligationAmount(obligation_amount_data)
                $toast.success('Successfully Save Obligation Amount!');
                return;

            }

            let obligation_data = new FormData();
            obligation_data.append('source_id', hot.getDataAtCell(row, 0) ? fund_sub_data_array.value[hot.getDataAtCell(row, 0)].source_id : 0);
            obligation_data.append('source_type', hot.getDataAtCell(row, 0) ? fund_sub_data_array.value[hot.getDataAtCell(row, 0)].source_type : 0);
            obligation_data.append('date', hot.getDataAtCell(row, 1) ? hot.getDataAtCell(row, 1) : "");
            obligation_data.append('dv', hot.getDataAtCell(row, 2) ? hot.getDataAtCell(row, 2) : "");
            obligation_data.append('po_no', hot.getDataAtCell(row, 3) ? hot.getDataAtCell(row, 3) : "");
            obligation_data.append('pr_no', hot.getDataAtCell(row, 4) ? hot.getDataAtCell(row, 4) : "");
            obligation_data.append('payee', hot.getDataAtCell(row, 5) ? hot.getDataAtCell(row, 5) : "");
            obligation_data.append('address', hot.getDataAtCell(row, 6) ? hot.getDataAtCell(row, 6) : "");
            obligation_data.append('particulars', hot.getDataAtCell(row, 7) ? hot.getDataAtCell(row, 7) : "");
            obligation_data.append('ors_no', hot.getDataAtCell(row, 8) ? hot.getDataAtCell(row, 8) : "");
            obligation_data.append('created_by', "");
            obligation_data.append('gross', hot.getDataAtCell(row, 10) ? hot.getDataAtCell(row, 10) : "");
            obligation_data.append('obligation_token', hot.getDataAtCell(row, 12));

            await __saveObligation(obligation_data, row)
            $toast.success('Successfully Save Obligation!');

            const send_data = { row: row, col: col, afterChange: true, userid: userInfo.value.userid, data: newValue }
            socket.value.send(JSON.stringify(send_data));
        }
    }

    const gridExpenseAmountToken = () => {
        return Array.from({ length: 36 }, (_, index) => {
            const number = index + 13;
            const label = index % 3 === 0 ? 'token' : '';
            const attribute = index % 3 === 2 ? 'amount' : 'expense';

            const value = [number, label + (label !== '' ? '' : attribute)];
            return value;
        });
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

    const get_fund_sub = ref([]);
    const fund_sub_data_array = ref([]);
    const __fundSub = async () => {
        const response = await fundSub()
        const fundSubDataDropdown: Promise<string[]> = Promise.all(response.map((item: any) => {
            const json_data: { source_id: string; source_type: string } = {
                source_id: item.source_id,
                source_type: item.source_type,
            };
            get_fund_sub.value[item.source_id + item.source_type] = item.source_title;
            fund_sub_data_array.value[item.source_title] = json_data;
            return item.source_title
        }));
        return fundSubDataDropdown
    }

    const totalObligation = ref(0)

    const totalRowFetchFromBackend = ref(0)
    const __obligationData = async () => {
        await __fundSub();
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
                item.source_type == "fund_source" ? get_fund_sub.value[item.fundSourceId + item.source_type] : get_fund_sub.value[item.subAllotmentId + item.source_type],
                moment(item.date, "YYYY-MM-DD").format('DD/MM/YYYY'), //1
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

            const beginningRemainingAllotmentBody = [
                item.source_type == "fund_source" ? item.fundSource.beginning_balance : item.subAllotment.beginning_balance,
                //item.source_type == "fund_source" ? item.fundSource.beginning_balance : 0,
                item.source_type == "fund_source" ? item.fundSource.remaining_balance : item.subAllotment.remaining_balance,
                //item.source_type == "fund_source" ? item.fundSource.remaining_balance : 0,
                item.source_type == "fund_source" ? item.fundSource.allotmentClassId : item.subAllotment.allotmentClassId,
                //item.source_type == "fund_source" ? item.fundSource.allotmentClassId : 0,
                item.source_type == "fund_source" ? item.fundSource.obligated_amount : item.subAllotment.obligated_amount
            ]

            totalObligation.value += obligation_amount;

            return dataBody.concat(...obligationAmountBody, ...ObligationAmountTokenBody, ...beginningRemainingAllotmentBody)

        }))

        totalRowFetchFromBackend.value = data.length
        handsondData.value = data
    }

    function formatNumber(number: number): string {
        const formattedNumber = number.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return formattedNumber;
    }

    const __expCode = async (allotmentId: Number) => {
        const response = await expCode({ allotmentId: allotmentId })
        return response.items
    }
    
    const __saveObligation = async (data: {}, row: number) => {
        /*console.log(data);*/
        const response = await saveObligation(data);
        const allotmentClassId = response.source_type == "fund_source" ? response.fundSource.allotmentClassId : response.subAllotment.allotmentClassId;
        handsondData.value[row][51] = allotmentClassId;
        const send_data = { row: row, col: 51, afterChange: true, userid: userInfo.value.userid, data: allotmentClassId }
        socket.value.send(JSON.stringify(send_data));
    };

    const __saveObligationAmount = async (data: {}) => {

        const cellValue: any[][] = hotTableComponent.value.hotInstance.getData();
        const totalSumObligation: number = cellValue.reduce((acc: number, curr: any[]) => acc + curr[10], 0);
        totalObligation.value = totalSumObligation;

        const response = await saveObligationAmount(data);

        const send_data = { afterChange: true, userid: userInfo.value.userid, totalObligation: totalObligation.value }
        socket.value.send(JSON.stringify(send_data));
    }

    const __deleteObligation = async (data: {}) => {
        await deleteObligation(data);
    }

    const __getRemainingObligated = async (data: {}) => {
        return await getRemainingObligated(data);
    }

    const __calculateObligatedAmount = async (data: {}) => {
        return await calculateObligatedAmount(data);
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
    <b>{{ displayCellTop }}</b>
    <br>
    <br>
    <!--<li v-for="item in handsondData">
        {{ item[3] }} <input type="text" name="name" :value="item[3]" /><input type="text" name="name" v-model="item[4]" />
    </li>-->
    <div v-if="handsondData.length > 0">
        <hot-table ref="hotTableComponent"
                   v-model:data="handsondData"
                   :colHeaders="colHeaders"
                   :colWidth="true"
                   :rowHeaders="true"
                   stretchH="all"
                   :settings="hotSettings"
                   :afterSelection="highlightCell"
                   :afterChange="afterChange"
                   :afterRender="afterRender"
                   :afterOnCellMouseDown="afterOnCellMouseDown"
                   :afterUndo="onAfterUndo">
        </hot-table>
        <br>
        <br>
        <div class="infobox infobox-blue2 pull-right" style="width: 230px">
            <div class="infobox-icon">
                <i class="ace-icon fa fa-money"></i>
            </div>
            <div class="infobox-data">
                <span class="infobox-data-number" style="font-size:11pt; color:grey;">
                    <span >
                        ₱{{ formatNumber(totalObligation) }}
                    </span>
                </span>
                <div class="infobox-content">
                    <span class="label label-info arrowed-in arrowed-in-right"> Total Obligated Amount </span>
                </div>
            </div>
        </div>
    </div>
    <h3 v-else>Processing....</h3>
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

    .highlight_salmon {
        background-color: lightsalmon !important;
    }

    .highlight_seagreen {
        background-color: lightseagreen !important;
    }

    .highlight_lightyellow {
        background-color: lightyellow !important;
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

    .wtHider {
        margin-bottom: 200px;
    }

    .filterHeader {
        color: black;
    }

    input {
        width: 750px;
    }
</style>