module ValuedCell

open System
open System.Windows.Forms
open System.Drawing

open BaseCell
open MineCell
open ValuedCellState
open ValuedCellImages

type ValuedCell (_value : int, _col : int, _row : int, _callBack : EventArgs * Label * int -> unit) =
    inherit BaseCell(CellState.Valued, _col, _row)
    let mutable value = _value
    let mutable state = ValuedCellState.Covered
    let label = new Label()
    let Label_OnClick (e : EventArgs) =
        match state with
        | ValuedCellState.Covered -> 
                state <- ValuedCellState.Uncovered
                label.BackgroundImage <- Image.FromFile(ValuedCellImages.[value])
        | ValuedCellState.Uncovered -> ignore()
    do
        let image = Image.FromFile("C:\\Users\\zakth\\source\\repos\\Minesweeper\\Minesweeper\\Resources\\Unopened.png")
        label.Size <- new Size(image.Size.Height / 2, image.Size.Width / 2)
        label.BackgroundImageLayout <- ImageLayout.Stretch
        label.BackgroundImage <- image
        label.Left <- (image.Width / 2) * _col
        label.Top <- (image.Height / 2) * _row
        label.Click.Add(fun e -> _callBack(e, label, value))
    
    member this.Value = value
    member this.State with get() = state and set(value) = state <- value
    member this.IncrementValue() = value <- value + 1
    override this.Label = label