using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PeopleViewer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var start = DateTimeOffset.Now;

            var ids = PersonReader.GetIds();

            Console.WriteLine(ids.ToDelimitedString(","));

            // Option 1 = Run Synchronously
            //RunSynchronously(ids);

            // Option 2 = Task w/ Continuation
            //await RunWithContinuation(ids);

            // Option 3 = Channels
            await RunWithChannel(ids);

            var elapsed = DateTimeOffset.Now - start;
            Console.WriteLine($"\nTotal time: {elapsed}");

            Console.ReadLine();
        }

        static void DisplayPerson(Person person)
        {
            Console.WriteLine("--------------");
            Console.WriteLine($"{person.ID}: {person}");
            Console.WriteLine($"{person.StartDate:D}");
            Console.WriteLine($"Rating: {new string('*', person.Rating)}");
        }

        // Option 1
        static void RunSynchronously(List<int> ids)
        {
            foreach (var id in ids)
            {
                var person = PersonReader.GetPerson(id);
                DisplayPerson(person);
            }
        }

        // Option 2
        static async Task RunWithContinuation(List<int> ids)
        {
            var allTasks = new List<Task>();

            foreach (var id in ids)
            {
                Task<Person> currentTask = Task.Run(() =>
                {
                    return PersonReader.GetPerson(id);
                });

                Task continuation = currentTask.ContinueWith(t =>
                {
                    Person person = t.Result;
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

            Task listener = ShowData(channel.Reader);

            // Single producer
            //Task producer = ProduceData(ids, channel.Writer);
            //await producer;

            // Multiple producers
            Task producer1 = ProduceData(ids.GetRange(0, 3), channel.Writer);
            Task producer2 = ProduceData(ids.GetRange(3, 3), channel.Writer);
            Task producer3 = ProduceData(ids.GetRange(6, 3), channel.Writer);
            await producer1;
            await producer2;
            await producer3;

            channel.Writer.Complete();

            await listener;
        }

        static Task ProduceData(List<int> ids, ChannelWriter<Person> writer)
        {
            return Task.Run(() =>
            {
                Parallel.ForEach(ids, id =>
                {
                    var person = PersonReader.GetPerson(id);
                    _ = writer.WriteAsync(person);
                });
            });
            //return Task.CompletedTask;
        }

        static async Task ShowData(ChannelReader<Person> reader)
        {
            await foreach (Person person in reader.ReadAllAsync())
            {
                DisplayPerson(person);
            }

            //while (await reader.WaitToReadAsync())
            //{
            //    while (reader.TryRead(out Person person))
            //    {
            //        Console.WriteLine($"{person.ID}: {person}");
            //    }
            //}
        }
    }
}
