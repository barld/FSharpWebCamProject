
#load "Scripts/load-project-debug.fsx"

open SimpleWebCam
open System.Drawing
open BitmapConverting


let wc = new WebCam()

let bm = wc.GetNBitMaps(1).Result

#time
    

//let rgb = bm |> Seq.map BitmapConverting.ToRGB |> Seq.head


#r "../packages/FSharp.Charting.0.90.13/lib/net40/FSharp.Charting.dll"

open FSharp.Charting

let getHHistogramData bitmap =
    bitmap
    |> ToRGB
    |> Array.map RGBToHSV
    |> Array.map (fun hsv -> hsv.h |> int)
    |> Array.sort
    |> Array.countBy (fun h -> h/10)


let firsth = 
    bm
    |> Seq.head
    |> getHHistogramData
#time





(ProjectHelpers.createBitmapViewer (bm.[0])).Show()
(Chart.Column firsth).ShowChart() |> ignore



(*
1)	Quantize the histogram (instead of lots of points, make twenty colums and draw them in the appropriate color). Each column corresponds to a range of H values, for example column one corresponds to 0<=h<1/20, column two corresponds to 1/20<=h<2/20, etc.

2)	Take a reference picture of an object with bright colors. Make a histogram of this reference picture.

3)	Normalize the histograms (the sum of all columns must be one, and the columns have floating point, not integer values)

4)	Compute the dot product of the reference histogram with that of the picture. If the value is high enough (determine this experimentally) then the object is in the picture, otherwise it is not.

*)