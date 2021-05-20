# Using Channels in C# to Enhance Concurrent Code

## Abstract  
Producer/consumer problems show up in a lot of programming scenarios, including data processing and machine learning. Channels were added to .NET Core 3.0 and give us a thread-safe way to communicate between producers and consumers, and we can run them all concurrently. In this presentation, we will explore channels by comparing parallel tasks with continuations to using a producer/consumer model. In the end, we'll have another tool in our toolbox to help us with concurrent programming.  

Recorded Presentation: [Using Channels in C# to Enhance Concurrent Code](https://www.youtube.com/watch?v=YxDORrTvIGM) - Dot Net North (May 2021)

## Project Layout
To build and run the code, you will need to have .NET 5 installed on your machine. The demo project will run wherever .NET 5 will run (Windows, macOS, Linux).

**/people-demo/PeopleViewer** contains a console application that uses channels program  
**/people-demo/People.Service** contains a service (used by the console application)  

The "PeopleViewer" program is a console application that calls the service and displays the output. In order to show concurrency, the application gets each record individually.

## Development Environments
**Visual Studio 2019**  
The "ChannelsDemo.sln" contains both of the projects listed above. The solution is set to automatically start both the service and the console application. So, hit "F5" to start the application & service.

**Visual Studio Code**  
In Visual Studio Code, you will want to start the service separately from the command line (see "Running the Service"). You can leave the service running while working with the console application.

## Running the Service
The **.NET service** can be started from the command line by navigating to the ".../people-demo/People.Service" directory and typing `dotnet run`. This provides endpoints at the following locations:

* http://localhost:9874/people  
Provides a list of "Person" objects. This service will delay for 3 seconds before responding. Sample result:

```json
[{"id":1,"givenName":"John","familyName":"Koenig","startDate":"1975-10-17T00:00:00-07:00","rating":6,"formatString":null},  
{"id":2,"givenName":"Dylan","familyName":"Hunt","startDate":"2000-10-02T00:00:00-07:00","rating":8,"formatString":null}, 
{...}]
```

* http://localhost:9874/people/ids  
Provides a list of "id" values for the collection. Sample:  

```json
[1,2,3,4,5,6,7,8,9]
```

* http://localhost:9874/people/1  
Provides an individual "Person" record based on the "id" value. This service will delay for 1 second before responding. Sample record:

```json
{"id":1,"givenName":"John","familyName":"Koenig","startDate":"1975-10-17T00:00:00-07:00","rating":6,"formatString":null}
```

The Console Application
---------------------
The **/people-demo/PeopleViewer** folder contains a console application that uses channels. The relevant code is in the "Program.cs" file.  

## Additional Sample
The "digit-display" folder contains an additional code sample. This application is a naive machine learning project that uses concurrent operations and channels. The version included here has a Windows-only user interface.  

For more additional information on this project (as well as a set of console applications that run cross-platform), visit the the main repository for the "digit-display" project:  

[Github: jeremybytes/digit-display](https://github.com/jeremybytes/digit-display)


## Resources
* Recorded Presentation: [Using Channels in C# to Enhance Concurrent Code](https://www.youtube.com/watch?v=YxDORrTvIGM) - Dot Net North (May 2021)
* [An Introduction to Channels in C#](https://jeremybytes.blogspot.com/2021/02/an-introduction-to-channels-in-c.html) - Jeremy Clark  
* [What's the Difference between Channel and ConcurrentQueue in C#](https://jeremybytes.blogspot.com/2021/02/whats-difference-between-channel-and.html) - Jeremy Clark  
* [An Introduction to System.Threading.Channels](https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/) - Stephen Toub  
* [Channel&lt;T&gt; Class](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels.channel-1?view=net-5.0) - Microsoft Docs  

## Additional Repositories
* [I'll Get Back to You: Task, Await, and Asynchronous Methods in C#](https://github.com/jeremybytes/using-task-core3)  
Presentation resources for understanding Task & await. Includes slides, code, videos, and articles
* [Understanding Asynchronous Programming in C# - Virtual Training](https://github.com/jeremybytes/understanding-async-programming)  
Virtual training resources for understanding Task, await, and parallel programming. Includes slides, code samples, and articles
* [Workshop: Asynchronous Programming in C#](https://github.com/jeremybytes/async-workshop-nov2020)  
Workshop resources for understanding Task, await, and parallel programming (including self-guided, hands-on labs). Includes slides, code samples, labs, and articles
