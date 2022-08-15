import 'package:flutter/material.dart';
import 'package:signalr_core/signalr_core.dart';
void main() => runApp(const MyApp());

class MyApp extends StatelessWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title:'SignalR Flutter',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: const MyHomePage(),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({Key? key}) : super(key: key);

  @override
  _MyHomePageState createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  final _msg = "Waiting for a message";
  var _conState = "Waiting for connection";

  _MyHomePageState() {
    _initSignalR();
  }

  //The location of the SignalR Server.
  static const serverUrl = "http://192.168.1.2/chatHub";

  //SignalR Connection
  final hubConnection = HubConnectionBuilder().withUrl(serverUrl).build();

  //Msg List
  List<String> ltMsg = [];

  //Msg
  final TextEditingController _controllerMsg = TextEditingController();

  //Name
  final TextEditingController _controllerName = TextEditingController();

  //Initialize connection
  void _initSignalR() async {
    debugPrint("Begin Connection");
    debugPrint(serverUrl);
    try {
      hubConnection.onclose((error) => debugPrint("Connection Closed"));
      await hubConnection.start();
      hubConnection.on("ReceiveMessage", (e) => {receiveMessage(e![0], e[1])});
      _conState = "Connected successfully";
    } catch (e) {
      _conState = "Connection failed";
    }
  }

  //accept message
  void receiveMessage(user, message) {
    setState(() {
      ltMsg.add(user + " - " + message);
    });
  }

  //send messages
  void sendMsg() async {
    final result = await hubConnection.invoke("SendMessage",
        args: <Object>[_controllerName.text, _controllerMsg.text]);
  }

  //Get list data
  _getListData() {
    List<Widget> widgets = [];
    for (int i = 0; i <ltMsg.length; i++) {
      widgets.add(Padding(
          padding: const EdgeInsets.all(10.0), child: Text(ltMsg[i])));
    }
    return widgets;
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("SignalR Flutter"),
      ),
      body: SingleChildScrollView(
        child: Container(
          margin: const EdgeInsets.all(20),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              Column(
                children: <Widget>[
                  TextField(
                    controller: _controllerName,
                    decoration: const InputDecoration(
                      hintText:'User',
                    ),
                  ),
                  TextField(
                    controller: _controllerMsg,
                    decoration: const InputDecoration(
                      hintText:'Message',
                    ),
                  ),
                  const Divider(),
                  ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      primary: Colors.blueAccent,
                      onPrimary: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(25),
                      ),
                      elevation: 15.0,
                    ),
                    child: const Text("Send Message"),
                    onPressed: sendMsg,
                  ),
                ],
              ),
              const Divider(),
              SizedBox(
                height: 300,
                child: ListView(children: _getListData()),
              ),
              const Divider(),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  Text(
                    _conState,
                    style: TextStyle(fontSize: 20,color: _conState.contains('successfully') ? Colors.green : _conState.contains('failed') ? Colors.red  : Colors.grey),
                  ),
                  ElevatedButton(
                    style: ElevatedButton.styleFrom(
                      primary: Colors.red,
                      onPrimary: Colors.white,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(20),
                      ),
                      elevation: 15.0,
                    ),
                    child: const Text("Reconnect"),
                    onPressed: _initSignalR,
                  ),
                ],
              )
            ],
          ),
        ),
      ),
    );
  }
}