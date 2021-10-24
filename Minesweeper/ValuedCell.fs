module ValuedCell

open System
open System.Windows.Forms
open System.Drawing

open BaseCell
open CellState
open ValuedCellState
open MinesweeperResourceManager

type ValuedCell (_value : int, _col : int, _row : int, _callBack : MouseEventArgs * Label * CellState * int * int -> unit) =
    inherit BaseCell(_col, _row)
    let mutable value = _value
    let mutable cellState = Unopened
    let label = new Label()
    do
        let image = GetResource($"{cellState}")
        label.Size <- new Size(image.Size.Height / 2, image.Size.Width / 2)
        label.BackgroundImageLayout <- ImageLayout.Stretch
        label.BackgroundImage <- image
        label.Left <- (image.Width / 2) * _col
        label.Top <- (image.Height / 2) * _row
        label.MouseClick.Add(fun e -> _callBack(e, label, cellState, _col, _row))
    
    member this.Value = value
    member this.Reveal() = 
        if cellState = Unopened then
            cellState <- Opened
            label.BackgroundImage <- GetResource($"Minesweeper_{value}")
    member this.IncrementValue() = value <- value + 1
    override this.CellState = cellState
    override this.Label = label
    override this.UpdateCellState _cellState = cellState <- _cellState