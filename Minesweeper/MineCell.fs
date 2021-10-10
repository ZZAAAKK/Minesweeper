module MineCell

open System
open System.Windows.Forms
open System.Drawing

open BaseCell
open MineState

type MineCell (_col : int, _row : int, _callBack : EventArgs * Label -> unit) =
    inherit BaseCell(CellState.Mine, _col, _row)
    let mutable state = MineState.Covered
    let label = new Label()
    let Label_OnClick (e : EventArgs) =
        match state with
        | x when x = MineState.Covered -> 
            state <- MineState.Exploded
            label.BackgroundImage <- Image.FromFile(state)
            label.Parent.Parent.Text <- "Hi"
        | _ -> ignore()
        state <- MineState.Exploded
    do
        let image = Image.FromFile(MineState.Covered)
        label.Size <- new Size(image.Size.Height / 2, image.Size.Width / 2)
        label.BackgroundImageLayout <- ImageLayout.Stretch
        label.BackgroundImage <- image
        label.Left <- (image.Width / 2) * _col
        label.Top <- (image.Height / 2) * _row
        label.Click.Add(fun e -> _callBack(e, label))

    member this.State with get() = state and set(value) = state <- value
    override this.Label = label