using digits;

namespace DigitDisplay;

public class ParallelTaskRecognizerControl : RecognizerControl
{
    public ParallelTaskRecognizerControl(string controlTitle, double displayMultiplier) :
        base($"{controlTitle} (Parallel Task)", displayMultiplier)
    { }

    protected override Task Run(Record[] rawData, Classifier classifier)
    {
        var allTasks = new List<Task>();
        foreach (var imageData in rawData)
        {
            var task = classifier.Predict(imageData);
            var continuation = task.ContinueWith(t =>
                {
                    var result = t.Result;
                    CreateUIElements(result, DigitsBox);
                },
                TaskScheduler.FromCurrentSynchronizationContext()
            );
            allTasks.Add(continuation);
        }
        return Task.WhenAll(allTasks);
    }
}
