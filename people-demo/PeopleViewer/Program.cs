using System.Threading.Channels;

namespace PeopleViewer;

class Program
{
    static async Task Main(string[] args)
    {
        var start = DateTimeOffset.Now;
        Console.Clear();

        var ids = await PersonReader.GetIdsAsync();

        Console.WriteLine(ids.ToDelimitedString(","));

        // Option 1 = Run Sequentially
        //await RunSequentially(ids);

        // Option 2 = Task w/ Continuation
        //await RunWithContinuation(ids);

        // Option 3 = Channels
        await RunWithChannel(ids);

        var elapsed = DateTimeOffset.Now - start;
        Console.WriteLine($"\nTotal time: {elapsed}");

        Console.ReadLine();
    }

    // Option 1
    static async Task RunSequentially(List<int> ids)
    {
        foreach (var id in ids)
        {
            var person = await PersonReader.GetPersonAsync(id);
            DisplayPerson(person);
        }
    }

    // Option 2
    static async Task RunWithContinuation(List<int> ids)
    {
        var allTasks = new List<Task>();

        foreach (var id in ids)
        {
            Task<Person> currentTask = PersonReader.GetPersonAsync(id);

            Task continuation = currentTask.ContinueWith(t =>
            {
                var person = t.Result;
                lock (allTasks)
                {
                    DisplayPerson(person);
                }
            });

            allTasks.Add(continuation);
        }
        await Task.WhenAll(allTasks);
    }

    // Option 3
    static async Task RunWithChannel(List<int> ids)
    {
        var channel = Channel.CreateBounded<Person>(10);

        var consumer = ShowData(channel.Reader);
        var producer = ProduceData(ids, channel.Writer);

        await producer;
        await consumer;
    }

    static async Task ShowData(ChannelReader<Person> reader)
    {
        //while (await reader.WaitToReadAsync())
        //{
        //    while (reader.TryRead(out Person? person))
        //    {
        //        DisplayPerson(person);
        //    }
        //}

        await foreach (var person in reader.ReadAllAsync())
        {
            DisplayPerson(person);
        }
    }

    static async Task ProduceData(List<int> ids, ChannelWriter<Person> writer)
    {
        var allTasks = new List<Task>();
        foreach (int id in ids)
        {
            var currentTask = FetchRecord(id, writer);
            allTasks.Add(currentTask);
        }
        await Task.WhenAll(allTasks);
        writer.Complete();
    }

    static async Task FetchRecord(int id, ChannelWriter<Person> writer)
    {
        var person = await PersonReader.GetPersonAsync(id);
        await writer.WriteAsync(person);
    }

    static void DisplayPerson(Person person)
    {
        Console.WriteLine("--------------");
        Console.WriteLine($"{person.ID}: {person}");
        Console.WriteLine($"{person.StartDate:D}");
        Console.WriteLine($"Rating: {new string('*', person.Rating)}");
    }
}
