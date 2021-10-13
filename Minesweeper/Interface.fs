module Interface

open System
open System.Windows.Forms
open System.Drawing

open Board
open Difficulty
open MineState
open ValuedCellState
open MineCell
open ValuedCell
open BaseCell
open BoardSizes
open CellState
open MinesweeperResourceManager

type Interface () =
    let form = new Form()
    let mutable board = new Board()
    let mutable difficulty = Difficulty.Beginner
    let mutable mineCount = 0
    let mutable size = BoardSizes.Beginner
    let mutable region : ValuedCell list = []
    let rec GetRegion (input : ValuedCell) = 
        region <- input :: region
        for i = -1 to 1 do
            for j = -1 to 1 do
                match (query {
                    for cell in board.Cells do
                    where (cell.Column = input.Column + i && cell.Row = input.Row + j)
                    exactlyOneOrDefault
                    }) with
                    | :? ValuedCell as v ->
                        if not (region |> List.contains(v)) then
                            if v.Value = 0 then v |> GetRegion
                            region <- v :: region
                    | _ -> ()
    let Reset (minecell_Callback : MouseEventArgs * Label * CellState -> unit, valuedCell_Callback : MouseEventArgs * Label * int * CellState * int * int -> unit) =
        for cell in board.Cells do cell.Label.Dispose()
        board.Panel.Dispose()
        board <- new Board()
        form.Controls.Add(board.Panel)
        board.init(difficulty, minecell_Callback, valuedCell_Callback, mineCount, size)
        form.ClientSize <- board.Panel.Size
    let rec ValuedCell_OnClick (e : MouseEventArgs, _label : Label, _value : int, _cellState : CellState, _col : int, _row : int) =
        match e.Button with
        | MouseButtons.Left -> 
            if _cellState = CellState.Unopened then 
                match (query { 
                    for cell in board.Cells do 
                    where (cell.Column = _col && cell.Row = _row) 
                    exactlyOneOrDefault 
                }) with
                | :? ValuedCell as c -> 
                    if c.Value = 0 then
                        c.Reveal()
                        c |> GetRegion
                        for cell in region do cell.Reveal() 
                        region <- []
                    elif c.Value > 0 && c.ValuedCellState = Covered then
                        c.Reveal()
                    elif c.Value > 0 && c.ValuedCellState = Uncovered then
                        let mutable nearestNeighbours : BaseCell list = []
                        let mutable foundMine = false
                        for i = -1 to 1 do
                            for j = -1 to 1 do
                                if not (i = 0 && j = 0) then
                                    nearestNeighbours <- (query {
                                        for cell in board.Cells do
                                        where (cell.Column = c.Column + i && cell.Row = c.Row + j)
                                        exactlyOneOrDefault
                                        }) :: nearestNeighbours
                        for n in nearestNeighbours do
                            if n.CellState = Unopened then
                                match n with
                                | :? ValuedCell as v ->
                                    if v.Value = 0 then
                                        c |> GetRegion
                                        for cell in region do cell.Reveal() 
                                        region <- []
                                    else v.Reveal()
                                | :? MineCell as m ->
                                    m.Detonate()                                        
                                    foundMine <- true
                                | _ -> ()
                        if foundMine then
                            let rec MineCell_OnClick (e : MouseEventArgs, _label : Label, _cellState : CellState) =
                                match e.Button with
                                | MouseButtons.Left -> 
                                    for cell in board.Cells do 
                                        match cell with
                                        | :? MineCell as c -> c.Reveal()
                                        | _ -> ()
                                    _label.BackgroundImage <- GetResource("Mine_Exploded")
                                    board.Panel.Enabled <- false
                                    match MessageBox.Show("You lose!\n\nPlay again?", "Game Over", MessageBoxButtons.YesNo) with
                                    | DialogResult.Yes -> Reset(MineCell_OnClick, ValuedCell_OnClick)                
                                    | _ -> Application.Exit()
                                | _ -> ignore()
                            board.Panel.Enabled <- false
                            match MessageBox.Show("You lose!\n\nPlay again?", "Game Over", MessageBoxButtons.YesNo) with
                            | DialogResult.Yes -> Reset(MineCell_OnClick, ValuedCell_OnClick)                
                            | _ -> Application.Exit()
                | _ -> ()
        | _ -> ignore()
    let rec MineCell_OnClick (e : MouseEventArgs, _label : Label, _cellState : CellState) =
        match e.Button with
        | MouseButtons.Left -> 
            for cell in board.Cells do 
                match cell with
                | :? MineCell as c -> c.Reveal()
                | _ -> ()
            _label.BackgroundImage <- GetResource("Mine_Exploded")
            board.Panel.Enabled <- false
            match MessageBox.Show("You lose!\n\nPlay again?", "Game Over", MessageBoxButtons.YesNo) with
            | DialogResult.Yes -> Reset(MineCell_OnClick, ValuedCell_OnClick)                
            | _ -> Application.Exit()
        | _ -> ignore()
    do
        form.Controls.Add(board.Panel)
        board.init(difficulty, MineCell_OnClick, ValuedCell_OnClick, mineCount, size)
        form.ClientSize <- board.Panel.Size
        form.FormBorderStyle <- FormBorderStyle.FixedSingle
        form.Text <- "Minesweeper"

    member this.Form = form