module BaseCell

open System.Windows.Forms

open CellState

[<AllowNullLiteral>]
[<AbstractClass>]
type BaseCell (_col : int, _row : int) = 
    let col = _col
    let row = _row
    let label = new Label()
    let mutable cellState = CellState.Unopened

    member this.Column = col
    member this.Row = row
    abstract Label : Label with get
    abstract CellState : CellState with get
    abstract member UpdateCellState : CellState -> unit