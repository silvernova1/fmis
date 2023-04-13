<script setup lang="ts">
    import { userDetails } from "../../../api/index"
    import { computed, ref, onMounted, watch, Ref } from "vue";
    import { HotTable } from '@handsontable/vue3';
    import { ContextMenu } from 'handsontable/plugins/contextMenu';
    import { registerAllModules } from 'handsontable/registry';
    import 'handsontable/dist/handsontable.full.css';

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

    const columns = ref(['', '', '', '', '', '', '', '', '', ''])
    const hotSettings = ref({
        //contextMenu: true,
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
        data: [
            ['A1', 'B1', 'C1', 'D1', 'E1', 'F1', 'G1', 'H1', 'I1', 'J1'],
            ['A2', 'B2', 'C2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2', 'J2'],
            ['A3', 'B3', 'C3', 'D3', 'E3', 'F3', 'G3', 'H3', 'I3', 'J3'],
            ['A4', 'B4', 'C4', 'D4', 'E4', 'F4', 'G4', 'H4', 'I4', 'J4'],
            ['A5', 'B5', 'C5', 'D5', 'E5', 'F5', 'G5', 'H5', 'I5', 'J5'],
            ['A6', 'B6', 'C6', 'D6', 'E6', 'F6', 'G6', 'H6', 'I6', 'J6'],
            ['A7', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A8', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A9', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A10', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A11', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A12', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A13', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
            ['A14', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha', 'haha'],
        ]
    })

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
        const highlight_array = ["highlight_yellow", "highlight_pink", "highlight_blue", "highlight_orange", "highlight_green", "highlight_pink", "highlight_yellow", "highlight_pink"]
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
               :colHeaders="true"
               :rowHeaders="true"
               stretchH="all"
               height="auto"
               :settings="hotSettings"
               :afterSelection="highlightCell"
               :afterChange="afterChange"
               :afterRender="afterRender"
               :afterOnCellMouseDown="afterOnCellMouseDown"
               :afterUndo="onAfterUndo"
               :columns="columns"
    >
    </hot-table>
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