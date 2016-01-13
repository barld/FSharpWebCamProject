
#load "Scripts/load-references-debug.fsx"

open SimpleWebCam
open System.Drawing

type rgb = {r:byte;g:byte;b:byte;a:byte}
type hsv = {h:float;s:float;v:float}


let wc = new WebCam()

let bm = wc.GetNBitMaps(1).Result

#time

let ToRGB (bmp : System.Drawing.Bitmap) =
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
        rtw.[i] <- {r=rgbValues.[i*4];g=rgbValues.[i*4+1];b=rgbValues.[i*4+2];a=rgbValues.[i*4+3]}   

    rtw
    

//https://nl.wikipedia.org/wiki/HSV_(kleurruimte)#Omzetting_van_RGB_naar_HSV
let RGBToHSV (rgb:rgb) =
    let getFloat32 (b:byte) =
        if b = 0uy then
            0.0
        else
            255.0/(b|>float)
    let r, g, b = getFloat32 rgb.r, getFloat32 rgb.g, getFloat32 rgb.b

    let maxv = max (max r g) b
    let minv = min (min r g) b

    if maxv = minv then
        {h=0.0;s=0.0;v=maxv}
    else
        let s = (maxv-minv)/maxv

        let h = 
            60.0 *
            match maxv with
            | m when r=m -> (g-b)/(maxv-minv)
            | m when g=m -> 2.0 + (b-r)/(maxv-minv)
            | m when b=m -> 4.0 + (r-g)/(maxv-minv)
            | _ -> failwith "error in code"

        {h=(h |> abs);s=s;v=maxv}


let rgb = bm |> Seq.map ToRGB

let hsv = rgb |> Seq.map (fun rgbl -> rgbl |> Array.map RGBToHSV)


// On Mac OSX use FSharp.Charting.Gtk.fsx
//#load "packages/FSharp.Charting/FSharp.Charting.fsx"

#r "../packages/FSharp.Charting.0.90.13/lib/net40/FSharp.Charting.dll"

open FSharp.Charting



let firsth = 
    hsv
    |> Seq.head
    |> Array.map (fun hsv -> hsv.h |> int)
    |> Array.sort
    |> Array.countBy (fun h -> h)
#time



#time
(Chart.FastLine firsth).ShowChart() |> ignore
#time

