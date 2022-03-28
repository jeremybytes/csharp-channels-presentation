using digits;

namespace DigitDisplay;

public class NonParallelRecognizerControl : RecognizerControl
{
    public NonParallelRecognizerControl(string controlTitle, double displayMultiplier) :
        base($"{controlTitle} (Non-Parallel)", displayMultiplier)
    { }

    protected override async Task Run(Record[] rawData, Classifier classifier)
    {
        foreach (var imageData in rawData)
        {
            var result = await classifier.Predict(imageData);
            CreateUIElements(result, DigitsBox);
        }
    }
}
