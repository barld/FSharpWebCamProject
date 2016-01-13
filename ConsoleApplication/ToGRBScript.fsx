
#load "Scripts/load-references-debug.fsx"

open SimpleWebCam
open System.Drawing

type rgb = {r:byte;g:byte;b:byte}
type hsv = {h:float;s:float;v:float}


let wc = new WebCam()

let bmp = wc.GetNBitMaps(1).Result |> Seq.head


let rect = new Rectangle(0,0,bmp.Width,bmp.Height)
#time
let bmpData = bmp.LockBits(rect, Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
#time

// Get the address of the first line.
let ptr = bmpData.Scan0;
    
#time
// Declare an array to hold the bytes of the bitmap.
let bytes = (abs bmpData.Stride) * bmp.Height;
let rgbValues : byte array = Array.create bytes 0uy
#time

#time
// Copy the RGB values into the array.
System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
#time

#time
bmp.UnlockBits(bmpData)
#time

#time
let rtw1 =
    rgbValues 
    |> Array.mapi (fun i v -> i/3,v) 
    |> Array.groupBy fst 
    |> Array.map snd
    |> Array.map (fun bg -> {b=snd bg.[0]; g = (snd bg.[1]); r = (snd bg.[2])})// b,g,r
#time


#time
let rtw = Array.create (bmp.Width * bmp.Height) {r=0uy;g=0uy;b=0uy}

for i in 0..(bmp.Width * bmp.Height)-1 do
    rtw.[i] <- {r=rgbValues.[i*4+1];g=rgbValues.[i*4+2];b=rgbValues.[i*4+3] }   
#time

rtw1 = rtw

rtw.Length
rtw1.Length