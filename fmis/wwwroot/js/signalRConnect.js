////realigned_notification_list
//var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//connection.start().then(function () {

//}).catch(function (err) {
//    return console.error(err.toString());
//});

//if (document.getElementById("sendButton")) {
//    document.getElementById("sendButton").addEventListener("click", function (event) {
//        var user = document.getElementById("userInput").value;
//        var message = document.getElementById("messageInput").value;
//        connection.invoke("SendMessage", user, message).catch(function (err) {
//            return console.error(err.toString());
//        });
//        event.preventDefault();
//    });
//}
//connection.on("ReceiveMessage", function (user, message) {
//    console.log(message);
//    var li = document.createElement("li");
//    document.getElementById("realigned_notification_list").appendChild(li);
//    li.textContent = `${user} says ${message}`;
//});
