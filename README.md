# apache-mod-proxy-unrecognized-response
Reproducing issue where calling a kestrel server behind Apache mod_proxy fails with: "The server returned an invalid or unrecognized response"

A Windows client fails to call a Kestrel server when behind Apache reverse proxy.

I was able to reproduce the issue in the following environments:
Apache: CentOS 7.3 -  httpd-2.4.6-45.el7.centos.4.x86_64

.NET Core client and server: Windows 10 Pro x64, Version 1607, OS Build: 14393.1198

It's also reproducible when running the .NET Core server on CentOS (as the steps below):

To reproduce it, follow the steps:
```bash
$ chmod +x setup-server.sh
$ ./setup-server.sh
```

Now you can throw the process to background (CTRL+Z + bg) or open another terminal

Apache takes some time to bootstrap, so you can test the ASP.NET Core app:
```bash
$ curl -I http://localhost:5000 
HTTP/1.1 204 No Content
Date: Sun, 21 May 2017 15:14:07 GMT
Server: Kestrel
```

Once Apache in the container is up and running, you should see the same output when using port 12345 instead.

Now run the client, on a Windows Machine:
```shell
$ dotnet restore & dotnet run -- -c http://server:12345
```

During my tests, anything between 2 and 25 requests resulted in the following output:

```shell
$ dotnet run -- -c http://192.168.1.151:12345
NoContent
NoContent

Unhandled Exception: System.Net.Http.HttpRequestException: An error occurred while sending the request. ---> System.Net.Http.WinHttpException: The server returned an invalid or unrecognized response
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1.ConfiguredTaskAwaiter.GetResult()
   at System.Net.Http.WinHttpHandler.<StartRequest>d__105.MoveNext()
   --- End of inner exception stack trace ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1.ConfiguredTaskAwaiter.GetResult()
   at System.Net.Http.HttpClient.<FinishSendAsync>d__58.MoveNext()
--- End of stack trace from previous location where exception was thrown ---
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at Program.Main(String[] args) in A:\apache-mod-proxy-unrecognized-response\ClientServer.cs:line 30
```

The client makes a request every 5 seconds. I was unable to reproduce the issue once I changed this interval.
