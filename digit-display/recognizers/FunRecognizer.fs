module Recognizers

open System

type Observation = { Label:string; Pixels:int[] }
type Prediction = { Prediction:string; Pixels:int[] }

let toObservation (csvData:string) =
    let columns = csvData.Split(',')
    let label = columns.[0]
    let pixels = columns.[1..] |> Array.map int
    { Label = label; Pixels = pixels }

let trainingData rawTrain =
    rawTrain |> Array.map toObservation

type Distance = int[] * int[] * int -> int

let manhattanDistance (pixels1,pixels2:int[],target) =
    let mutable total = 0
    let len = pixels1 |> Array.length
    for i in 0 .. (len - 1) do
        total <- total + abs (pixels1.[i] - pixels2.[i])
    total

let euclideanDistance (pixels1,pixels2:int[],target) =
    let mutable total = 0
    let len = pixels1 |> Array.length
    let diff i = pixels1.[i] - pixels2.[i]
    for i in 0 .. (len - 1) do
        total <- total + diff i * diff i
    total

let train (trainingset:Observation[]) (dist:Distance) =
    let classify (pixels:int[]) =
        trainingset
        |> Array.minBy (fun x -> dist (x.Pixels, pixels, Int32.MaxValue))
    classify

let classifier (rawTrain:string[]) = train (trainingData rawTrain)

let manhattanClassifier (rawTrain:string[]) = train (trainingData rawTrain) manhattanDistance
let euclideanClassifier (rawTrain:string[]) = train (trainingData rawTrain) euclideanDistance

let predict (pixels:int[]) classifier =
    classifier pixels
