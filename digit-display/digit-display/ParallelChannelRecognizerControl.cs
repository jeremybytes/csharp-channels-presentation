using digits;
using System.Threading.Channels;

namespace DigitDisplay;

public class ParallelChannelRecognizerControl : RecognizerControl
{
    public ParallelChannelRecognizerControl(string controlTitle, double displayMultiplier) :
    base($"{controlTitle} (Parallel Channel)", displayMultiplier) { }

    protected override async Task Run(Record[] rawData, Classifier classifier)
    {
        var channel = Channel.CreateUnbounded<Prediction>();

        var listener = Listen(channel.Reader);
        var producer = Produce(channel.Writer, rawData, classifier);

        await producer;
        await listener;
    }

    private async Task Listen(ChannelReader<Prediction> reader)
    {
        await foreach (Prediction result in reader.ReadAllAsync())
        {
            CreateUIElements(result, DigitsBox);
        }
    }

    private async Task Produce(ChannelWriter<Prediction> writer, Record[] rawData,
        Classifier classifier)
    {
        await Parallel.ForEachAsync(
            rawData, 
            new ParallelOptions() { MaxDegreeOfParallelism = 6 },
            async (imageData, token) =>
            {
                var result = await classifier.Predict(imageData);
                await writer.WriteAsync(result, token);
            });

        writer.Complete();
    }

}

