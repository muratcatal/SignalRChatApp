In server, we are going to implement a self-hosting signal-r application which clients will connect from
.net application and web application.

To start up, at first we are creating a new console application. Next we are going to include signal-r self hosting
dlls from NuGet package manager. After importing required dlls, we can now add our hub class. Add a new class into
Hubs folder and extend it from Hubs (Asp.Net SignalR) class. Than we are adding a StartUp.cs class to tell our console
application to map signalR class. Next, in our main function, we start to serve our server from the url we specified.