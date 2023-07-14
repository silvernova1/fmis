/*
$(() => {

    LoadProdData();

    var connection = new signalR.HubConnectionBuilder().withUrl("/signalrServer").build();
    connection.start();
    connection.on("LoadProducts", function () {
        LoadProdData();
    }) 

    LoadProdData();

    function LoadProdData() {
        var tr = '';

        $.ajax({
            url: '/Assignee/GetAssignee',
            method: 'GET',
            success: (result) => { 
                $.each(result, (k, v) => { 
                    tr += `<tr>
                      <td>$ { v.FullName } </td>

                    </tr> `
                })
                $("#tableBody").html(tr);
            },
            error: (error) => {
                console.log(error)
            }

        });
    }

})*/