
#load "Scripts/load-project-debug.fsx"

open SimpleWebCam
open System.Drawing
open BitmapConverting


let wc = new WebCam()

let bm = wc.GetNBitMaps(1).Result

let pixels = ((bm |> Seq.head).Height * (bm |> Seq.head).Width) |> float

#time
    

//let rgb = bm |> Seq.map BitmapConverting.ToRGB |> Seq.head


#r "../packages/FSharp.Charting.0.90.13/lib/net40/FSharp.Charting.dll"

open FSharp.Charting

let getHHistogramData' bitmap =
    bitmap
    |> ToRGB
    |> Array.map RGBToHSV
    |> Array.map (fun hsv -> hsv.h |> int)
    |> Array.sort
    |> Array.countBy (fun h -> (h/10)*10)
    |> Array.map (fun (a, b) -> (a, (b |> float) / pixels ))



#time
let getHHistogramData bitmap =
    let hsvArray = 
        bitmap
        |> ToRGB
        |> Array.map RGBToHSV

    [for i in [0.0f..1.0f..35.0f] -> (i*10.f, (hsvArray |> Array.filter(fun hsv -> (i/36.f) <= hsv.h / 360.f && hsv.h / 360.f < ((i+1.f)/36.f)) |> Array.length |> float)/ pixels)]

#time
let firsth = 
    bm
    |> Seq.head
    |> getHHistogramData
#time

#time
let RGBToH (rgb:rgb) =
    let r, g, b = (rgb.r |> float) / 255., (rgb.g |> float) / 255., (rgb.b |> float) / 255.

    let maxv = max (max r g) b
    let minv = min (min r g) b

    if maxv = minv then
        0.0
    else
        match maxv with
        | m when r=m -> (g-b)/(maxv-minv)
        | m when g=m -> 2.0 + (b-r)/(maxv-minv)
        | m when b=m -> 4.0 + (r-g)/(maxv-minv)
        | _ -> failwith "error in code"
        / 6.0
        
let getHHistogramData' bitmap =
        let hArray =
            bitmap
            |> ToRGB
            |> Array.map RGBToH

        [for i in [0.0..1.0..35.0] -> (i*10., (hArray |> Array.filter(fun h -> (i/36.) <= h && h < ((i+1.)/36.)) |> Array.length |> float)/ pixels)]

#time
let firsth' = 
    bm
    |> Seq.head
    |> getHHistogramData'
#time



firsth |> List.sumBy snd
#time

let reference = [(0.0f, 0.03952148438); (10.0f, 0.04311523438); (20.0f, 0.05429361979);(30.0f, 0.0570703125); (40.0f, 0.09747721354); (50.0f, 0.04459635417);(60.0f, 0.1116731771); (70.0f, 0.02562825521); (80.0f, 0.2239908854);(90.0f, 0.139781901); (100.0f, 0.007701822917); (110.0f, 0.00095703125);(120.0f, 0.004583333333); (130.0f, 0.001682942708);(140.0f, 0.002194010417); (150.0f, 0.001826171875);(160.0f, 0.001067708333); (170.0f, 0.0026953125); (180.0f, 0.005052083333);(190.0f, 0.004436848958); (200.0f, 0.009625651042); (210.0f, 0.01236653646);(220.0f, 0.02409830729); (230.0f, 0.007513020833); (240.0f, 0.02057942708);(250.0f, 0.01242838542); (260.0f, 0.01584635417); (270.0f, 0.0130859375);(280.0f, 0.01491536458); (290.0f, 0.0001953125); (300.0f, 0.0);(310.0f, 0.0); (320.0f, 0.0); (330.0f, 0.0); (340.0f, 0.0); (350.0f, 0.0)]



let calculateDotProduct list1 list2 =
    list1 |> List.map2 (fun (a,b) (c,d) -> b*d ) list2 |> List.sum

let dotproduct = calculateDotProduct firsth reference
    





(ProjectHelpers.createBitmapViewer (bm.[0])).Show()
(Chart.Column firsth).ShowChart() |> ignore
(Chart.Column firsth').ShowChart() |> ignore
//(Chart.Column firsth').ShowChart() |> ignore



(*
1)	Quantize the histogram (instead of lots of points, make twenty colums and draw them in the appropriate color). Each column corresponds to a range of H values, for example column one corresponds to 0<=h<1/20, column two corresponds to 1/20<=h<2/20, etc.

2)	Take a reference picture of an object with bright colors. Make a histogram of this reference picture.

3)	Normalize the histograms (the sum of all columns must be one, and the columns have floating point, not integer values)

4)	Compute the dot product of the reference histogram with that of the picture. If the value is high enough (determine this experimentally) then the object is in the picture, otherwise it is not.

*)