namespace digits;

public class K5EuclideanClassifier : Classifier
{
    public K5EuclideanClassifier(Record[] training_data)
        : base("Euclidean Classifier", training_data)
    {
    }

    private List<(int, Record)> GetTopFive(List<(int, Record)> input)
    {
        return input.OrderBy(x => x.Item1).Take(5).ToList();
    }

    private Record GetBest(List<(int, Record)> input)
    {
        var records = input.Select(x => x.Item2);
        var groups = input.GroupBy(r => r.Item2.Value);
        if (groups.Count() == 1)
        {
            return input.First().Item2;
        }
        var sums = groups.OrderByDescending(g => g.Count());
        var bestMatches = sums.Select(g => g.Key).First();
        var best = input.First(m => m.Item2.Value == bestMatches);
        return best.Item2;
    }

    public override Task<Prediction> Predict(Record input)
    {
        return Task.Run(() =>
        {
            List<(int, Record)> bests = new()
            {
                (int.MaxValue, new(0, Array.Empty<int>()))
            };

            int[] inputImage = input.Image;
            foreach (Record candidate in TrainingData)
            {
                var bestsMax = bests.Max(x => x.Item1);
                int total = 0;
                int[] candidateImage = candidate.Image;
                for (int i = 0; i < 784; i++)
                {
                    int diff = inputImage[i] - candidateImage[i];
                    total += (diff * diff);
                }
                if (total < bestsMax)
                {
                    bests.Add((total, candidate));
                    bests = GetTopFive(bests);
                    bestsMax = bests.Max(x => x.Item1);
                }
            }

            var best = GetBest(bests);

            return new Prediction(input, best);
        });
    }
}
