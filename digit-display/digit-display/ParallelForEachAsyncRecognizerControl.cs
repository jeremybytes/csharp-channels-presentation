using digits;

namespace DigitDisplay
{
    public class ParallelForEachAsyncRecognizerControl : RecognizerControl
    {
        public ParallelForEachAsyncRecognizerControl(string controlTitle, double displayMultiplier) :
            base($"{controlTitle} (Parallel ForEachAsync)", displayMultiplier)
        { }

        protected override async Task Run(Record[] rawData, Classifier classifier)
        {
            await Parallel.ForEachAsync(
                rawData,
                new ParallelOptions() { MaxDegreeOfParallelism = 10 },
                async (imageData, _) =>
                {
                    var result = await classifier.Predict(imageData);
                    Dispatcher.Invoke(() => CreateUIElements(result, DigitsBox));
                });
        }
    }
}
