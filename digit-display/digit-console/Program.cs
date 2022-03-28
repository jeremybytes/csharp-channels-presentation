using digits;
using digit_console;
using System.Threading.Channels;
using CommandLine;
using System.Text;
using System.Diagnostics;

int offset = 0;
int count = 10;
string classifier_option = "";
int threads = 6;

CommandLine.Parser.Default.ParseArguments<Configuration>(args)
    .WithParsed(c =>
    {
        offset = c.Offset;
        count = c.Count;
        threads = c.Threads;
        classifier_option = c.Classifier.ToLower()
        switch
        {
            "euclidean" => "euclidean",
            "manhattan" => "manhattan",
            _ => "euclidean",
        };
    }).WithNotParsed(c =>
    {
        Environment.Exit(0);
    });

List<Prediction> errors = new();

var (training, validation) = FileLoader.GetData("train.csv", offset, count);
Console.Clear();
Console.WriteLine("Data Load Complete...");

Classifier classifier = classifier_option
    switch
{
    "euclidean" => new EuclideanClassifier(training),
    "manhattan" => new ManhattanClassifier(training),
    _ => new EuclideanClassifier(training),
};

var timer = new Stopwatch();
timer.Start();

var channel = Channel.CreateUnbounded<Prediction>();
var listener = Listen(channel.Reader, errors);

var producer = Produce(channel.Writer, classifier, validation, threads);
await producer;

await listener;

timer.Stop();
var elapsed = timer.Elapsed;

PrintSummary(classifier, offset, count, elapsed, errors.Count);
Console.WriteLine("Press any key to show errors...");
Console.ReadLine();

foreach (var item in errors)
{
    DisplayImages(item, true);
}

PrintSummary(classifier, offset, count, elapsed, errors.Count);



static void DisplayImages(Prediction prediction, bool scroll)
{
    if (!scroll)
    {
        Console.SetCursorPosition(0, 0);
    }
    StringBuilder output = new();
    output.Append($"Actual: {prediction.Actual.Value} ");
    output.Append(' ', 46);
    output.AppendLine($" | Predicted: {prediction.Predicted.Value}");
    Display.GetImagesAsString(output, prediction.Actual.Image, prediction.Predicted.Image);
    output.Append('=', 115);
    Console.WriteLine(output);
}

static void PrintSummary(Classifier classifier, int offset, int count, TimeSpan elapsed, int total_errors)
{
    Console.WriteLine($"Using {classifier.Name} -- Offset: {offset}   Count: {count}");
    Console.WriteLine($"Total time: {elapsed}");
    Console.WriteLine($"Total errors: {total_errors}");
}

static async Task Produce(ChannelWriter<Prediction> writer,
    Classifier classifier, Record[] validation, int threads)
{
    await Parallel.ForEachAsync(
        validation,
        new ParallelOptions() { MaxDegreeOfParallelism = threads },
        async (imageData, token) =>
        {
            var result = await classifier.Predict(new(imageData.Value, imageData.Image));
            await writer.WriteAsync(result, token);
        });

    writer.Complete();
}

static async Task Listen(ChannelReader<Prediction> reader,
    List<Prediction> log)
{
    await foreach (Prediction prediction in reader.ReadAllAsync())
    {
        DisplayImages(prediction, false);
        if (prediction.Actual.Value != prediction.Predicted.Value)
        {
            log.Add(prediction);
        }
    }
}


