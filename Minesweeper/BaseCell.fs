module BaseCell

open System.Windows.Forms

open CellState

[<AbstractClass>]
type BaseCell (_state : CellState, _col : int, _row : int) = 
    let state = _state
    let col = _col
    let row = _row
    let label = new Label()

    member this.Column = col
    member this.Row = row
    abstract Label : Label with get