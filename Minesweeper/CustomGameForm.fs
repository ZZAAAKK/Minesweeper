module CustomGameForm

open System.Windows.Forms
open System.Drawing

[<AllowNullLiteral>]
type CustomGameForm (_owner : Form, _mineCount : int, _size : Point, _submitCallBack : int * Point -> unit) =
    let form = new Form()
    let mineCountLabel = new Label()
    let mineCountSpinner = new NumericUpDown()
    let heightLabel = new Label()
    let heightSpinner = new NumericUpDown()
    let widthLabel = new Label()
    let widthSpinner = new NumericUpDown()
    let submitButton = new Button()
    let cancelButton = new Button()
    let mutable newMineCount = _mineCount
    let mutable newHeight = _size.X
    let mutable newWidth = _size.Y
    let newSize() = new Point(newHeight, newWidth)
    let UpdateMineCount() =
        newMineCount <- (int)mineCountSpinner.Value
        mineCountSpinner.Text <- $"{newMineCount}"
    let ManualUpdateMineCount() =
        match System.Int32.TryParse mineCountSpinner.Text with
        | true,int -> 
            newMineCount <- int
            mineCountSpinner.Text <- $"{newMineCount}"
        | _ -> 
            mineCountSpinner.Text <- $"{newMineCount}"
    let UpdateHeight() =
        newHeight <- (int)heightSpinner.Value
        heightSpinner.Text <- $"{newHeight}"
    let ManualUpdateHeight() =
        match System.Int32.TryParse heightSpinner.Text with
        | true,int -> 
            newHeight <- int
        | _ -> 
            heightSpinner.Text <- $"{newHeight}"
    let UpdateWidth() =
        newWidth <- (int)widthSpinner.Value
        widthSpinner.Text <- $"{newWidth}"
    let ManualUpdateWidth() =
        match System.Int32.TryParse widthSpinner.Text with
        | true,int -> 
            newWidth <- int
        | _ -> 
            widthSpinner.Text <- $"{newWidth}"
    do
        newMineCount <- _mineCount
        newHeight <- _size.X
        newWidth <- _size.Y
        form.Owner <- _owner
        form.ClientSize <- new Size(250, 105)
        form.FormBorderStyle <- FormBorderStyle.FixedDialog
        form.StartPosition <- FormStartPosition.CenterParent
        form.Text <- "Create Custom Game"
        mineCountLabel.Text <- "Number of mines:"
        mineCountLabel.AutoSize <- true
        mineCountLabel.Location <- new Point(10, 10)
        mineCountSpinner.Minimum <- 1.0m
        mineCountSpinner.Maximum <- 99.0m
        mineCountSpinner.Increment <- 1.0m
        mineCountSpinner.DecimalPlaces <- 0
        mineCountSpinner.Value <- (decimal)newMineCount
        mineCountSpinner.ValueChanged.Add(fun _ -> UpdateMineCount())
        mineCountSpinner.TextChanged.Add(fun _ -> ManualUpdateMineCount())
        mineCountSpinner.Location <- new Point(120, 10)
        heightLabel.Text <- "Height:"
        heightLabel.AutoSize <- true
        heightLabel.Location <- new Point(10, 30)
        heightSpinner.Minimum <- 8.0m
        heightSpinner.Maximum <- 30.0m
        heightSpinner.Increment <- 1.0m
        heightSpinner.DecimalPlaces <- 0
        heightSpinner.Value <- (decimal)newHeight
        heightSpinner.ValueChanged.Add(fun _ -> UpdateHeight())
        heightSpinner.TextChanged.Add(fun _ -> ManualUpdateHeight())
        heightSpinner.Location <- new Point(120, 30)
        widthLabel.Text <- "Width:"
        widthLabel.AutoSize <- true
        widthLabel.Location <- new Point(10, 50)
        widthSpinner.Minimum <- 8.0m
        widthSpinner.Maximum <- 30.0m
        widthSpinner.Increment <- 1.0m
        widthSpinner.DecimalPlaces <- 0
        widthSpinner.Value <- (decimal)newWidth
        widthSpinner.ValueChanged.Add(fun _ -> UpdateWidth())
        widthSpinner.TextChanged.Add(fun _ -> ManualUpdateWidth())
        widthSpinner.Location <- new Point(120, 50)
        submitButton.Text <- "Submit"
        submitButton.Location <- new Point(10, 75)
        submitButton.Size <- new Size(100, 25)
        submitButton.Click.Add(fun _ -> 
            _submitCallBack(newMineCount, newSize())
            form.Dispose())
        cancelButton.Text <- "Cancel"
        cancelButton.Location <- new Point(140, 75)
        cancelButton.Size <- new Size(100, 25)
        cancelButton.Click.Add(fun _ -> form.Dispose())
        form.Controls.AddRange <| [| mineCountLabel; mineCountSpinner; heightLabel; heightSpinner; widthLabel; widthSpinner; submitButton; cancelButton |]
        form.Show()
        

    member this.Form = form