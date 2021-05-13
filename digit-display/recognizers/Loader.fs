module Loader

open System.IO

let trainingReader path fileOffset recordCount = 
    let data = File.ReadAllLines path
    Array.concat [|data.[1..fileOffset]; data.[fileOffset+recordCount+1..]|]

let validationReader path fileOffset recordCount =
    let data = File.ReadAllLines path
    data.[(1+fileOffset)..(fileOffset+recordCount)]
