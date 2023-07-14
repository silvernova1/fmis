const WebSocket = require('ws');
const server = new WebSocket.Server({ port: 8080 });
const clients = new Set();

server.on('connection', (socket) => {
    console.log('Client connected');

    // Add the new client to the list of connected clients
    clients.add(socket);
    socket.send(simpleStringify(socket));

    const connectedClients = Array.from(clients).map((client) => client._protocol);
    broadcastAll(JSON.stringify({ connected_clients: true, data: connectedClients }), true)

    socket.on('close', () => {
        console.log('Client disconnected');

        // Remove the disconnected client from the list of connected clients
        clients.delete(socket);

        // Send the updated list of connected clients to all clients
        const connectedClients = Array.from(clients).map((client) => client._socket.remoteAddress);
        if (clients.length > 0) {
            console.clients.forEach(function each(client) {
                client.send(JSON.stringify({ type: 'connected_clients', data: connectedClients }));
            });
        }
    });

    socket.on('message', function incoming(data) {
        broadcastAll(data, null, socket._protocol)
    });
});

function broadcastAll(data, conected_clients = null, userid = null) {
    // Broadcast to all clients
    const flag = true
    server.clients.forEach(function each(client) {
        if (client.readyState === WebSocket.OPEN) {
            if (conected_clients) {
                client.send(data);
            }
            else {
                const buffer = Buffer.from(data, 'hex');
                const convert_data = buffer.toString();
                console.log(convert_data);
                if (data.userid === userid && flag) {
                    flag = false
                    client.send(convert_data);
                }
                else if (data.userid !== userid) {
                    client.send(convert_data);
                }
            }
        }
    });
}

function simpleStringify(object) {
    var simpleObject = {};
    for (var prop in object) {
        if (!object.hasOwnProperty(prop)) {
            continue;
        }
        if (typeof (object[prop]) == 'object') {
            continue;
        }
        if (typeof (object[prop]) == 'function') {
            continue;
        }
        simpleObject[prop] = object[prop];
    }
    return JSON.stringify(simpleObject);
};

