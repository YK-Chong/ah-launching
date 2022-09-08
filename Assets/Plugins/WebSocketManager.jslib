mergeInto(LibraryManager.library, {
    WebGL_ConnectWebSocket: function (callback) {
        let connected = false;
        let onReceiveMessageCallback;
        let ws;
        connect("User", onMessageReceived);

        function onMessageReceived(msg) {
            if (msg.includes("CurrentState:") && callback !== 0) {
                let state = parseInt(msg.split(":")[1]);
                Module.dynCall_vi(callback, state);
            }
        };

        function connect(userType, onReceiveMessage) {
            if (!connected) {
                connected = true;
                tryConnectToWS(userType);
                onReceiveMessageCallback = onReceiveMessage;
                setInterval(() => tryConnectToWS(userType), 1000);
                setInterval(() => ws.send("Ping"), 60000);
            }
        }

        function tryConnectToWS(userType) {
            if (ws == undefined || ws.readyState === ws.CLOSED) {
                ws = new WebSocket("wss://quixotic-grey-ceiling.glitch.me/");
                ws.addEventListener("open", () => {
                    console.log("We are connected at " + new Date());
                    ws.send("Connect:" + userType);
                });

                ws.addEventListener("message", function (event) {
                    onReceiveMessageCallback(event.data);
                });

                ws.addEventListener("error", function (event) {
                    console.log(event.data);
                });
            } else if (ws.readyState === ws.OPEN) {

            }
        }
    }
});