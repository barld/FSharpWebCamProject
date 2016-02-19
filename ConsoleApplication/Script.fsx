
#load "Scripts/load-project-debug.fsx"

open SimpleWebCam
open System.Drawing
open BitmapConverting


let wc = new WebCam()

let bm = wc.GetNBitMaps(1).Result.[0]


#r "../packages/FSharp.Charting.0.90.13/lib/net40/FSharp.Charting.dll"

open FSharp.Charting

type rgb = {r:byte;g:byte;b:byte;a:byte}
type hsv = {h:float32;s:float32;v:float32}

let ToRGB (bmp : System.Drawing.Bitmap) =
    match bmp.PixelFormat with
    | Imaging.PixelFormat.Format32bppArgb ->
        let rect = new Rectangle(0,0,bmp.Width,bmp.Height)
        let bmpData = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);

        // Get the address of the first line.
        let ptr = bmpData.Scan0;
    
        // Declare an array to hold the bytes of the bitmap.
        let bytes = (abs bmpData.Stride) * bmp.Height;
        let rgbValues : byte array = Array.create bytes 0uy

        // Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        bmp.UnlockBits(bmpData)


        let rtw = Array.create (bmp.Width * bmp.Height) {r=0uy;g=0uy;b=0uy;a=0uy}

        for i in 0..(bmp.Width * bmp.Height)-1 do
            rtw.[i] <- {r=rgbValues.[i*4+2];g=rgbValues.[i*4+1];b=rgbValues.[i*4];a=rgbValues.[i*4+3]}   

        rtw
    | _ -> failwith "not suported format"

let RGBToH (rgb:rgb) =
    let r, g, b = (rgb.r |> float32) / 255.f, (rgb.g |> float32) / 255.f, (rgb.b |> float32) / 255.f

    let maxv = max (max r g) b
    let minv = min (min r g) b

    if maxv = minv then
        0.0f
    else
        match maxv with
        | m when r=m -> (g-b)/(maxv-minv)
        | m when g=m -> 2.0f + (b-r)/(maxv-minv)
        | m when b=m -> 4.0f + (r-g)/(maxv-minv)
        | _ -> failwith "error in code"
        / 6.0f |> abs

let groupByI projection (array: 't array ) =
    array
    |> Array.mapi (fun i item -> i,item)
    |> Array.groupBy projection
    |> Array.map (fun (a,b) -> a, b |> Array.map snd) 

let splitInParts (width,wParts) (height,hParts) array =
    let w = width / wParts
    let h = height / hParts 
    array
    |> groupByI (fun (index, item) -> ((index%width)/w, (index/width)/h))

type HHistrogram = 
    {data:float list; x:int; y:int}

let getHHistogramData (rgb: rgb[]) =
    let pixels = rgb |> Array.length |> float
    printfn "%A" pixels
    let hArray =
        rgb
        |> Array.map RGBToH
    
    [for i in [0.0f..1.0f..19.0f] -> (hArray |> Array.filter(fun h -> (i/20.f) <= h && h < ((i+1.f)/20.f)) |> Array.length |> float) / pixels];

#time
let firsth  = 
    bm
    |> ToRGB
    |> splitInParts (bm.Width, 16) (bm.Height, 16) 
    |> Array.map (fun ((x,y), pixels) -> {x=x;y=y;data= getHHistogramData pixels})
#time

let calculateDotProduct list1 list2 =
    list1 |> List.map2 (fun a b -> a * b ) list2 |> List.sum

let yellowCardReference = [0.002083333333; 0.0; 0.0; 0.5058854167; 0.4874479167; 0.004583333333; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0]

let redCardReference = [0.9999153646; 8.463541667e-05; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0; 0.0]


let isYellow (histogrm:HHistrogram) =
    0.5 < calculateDotProduct yellowCardReference histogrm.data

let isRed (histogrm:HHistrogram) =
    0.6 < calculateDotProduct redCardReference histogrm.data

let markRedSectors (bmp:Bitmap) histograms wParts hParts =
    let redsectores = histograms |> Array.filter (fun histo -> isRed histo)
    match bmp.PixelFormat with
    | Imaging.PixelFormat.Format32bppArgb ->
        let rect = new Rectangle(0,0,bmp.Width,bmp.Height)
        let bmpData = bmp.LockBits(rect, Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

        // Get the address of the first line.
        let ptr = bmpData.Scan0;
    
        // Declare an array to hold the bytes of the bitmap.
        let bytes = (abs bmpData.Stride) * bmp.Height;
        let rgbValues : byte array = Array.create bytes 0uy

        // Copy the RGB values into the array.
        System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        let w = bmp.Width / wParts
        let h = bmp.Height / hParts 

        for histo in redsectores do   
            for x in [0..w-1] do
                for y in [0..h-1] do
                    do rgbValues.[(y*bmp.Width + h*bmp.Width*histo.y + w * histo.x + x) * 4 + 1] <- 255uy

        // Copy the RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            

        bmp.UnlockBits(bmpData)
        () 
    | _ -> failwith "not suported format"

#time
markRedSectors bm firsth 16 16
#time


(ProjectHelpers.createBitmapViewer (bm)).Show()
//(Chart.Column redCardReference).ShowChart() |> ignore

//firsth' |> Array.iter (fun histo -> (Chart.Column histo.data).ShowChart() |> ignore)

//(Chart.Column firsth').ShowChart() |> ignore



(*
1)	Quantize the histogram (instead of lots of points, make twenty colums and draw them in the appropriate color). Each column corresponds to a range of H values, for example column one corresponds to 0<=h<1/20, column two corresponds to 1/20<=h<2/20, etc.

2)	Take a reference picture of an object with bright colors. Make a histogram of this reference picture.

3)	Normalize the histograms (the sum of all columns must be one, and the columns have floating point, not integer values)

4)	Compute the dot product of the reference histogram with that of the picture. If the value is high enough (determine this experimentally) then the object is in the picture, otherwise it is not.

*)
