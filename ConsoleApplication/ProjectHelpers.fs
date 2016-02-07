

module ProjectHelpers 
    open System.Windows.Forms

    let createBitmapViewer bitmap =
        let form = new Form()
        form.Name <- "bitmap vieuwer"
        let pb = new PictureBox()
        form.Controls.Add(pb)
        pb.Image <- bitmap
        pb.Dock <- DockStyle.Fill
        pb.SizeMode <- PictureBoxSizeMode.StretchImage
        pb.BringToFront()
        pb.Update()
        form
