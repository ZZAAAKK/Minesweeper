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
open Banner
open MinesweeperResourceManager
open MainMenuBar

type Interface () =
    let form = new Form()
    let mutable board = new Board()
    let mutable difficulty = Difficulty.Beginner
    let mutable mineCount = 0
    let mutable size = BoardSizes.Beginner
    let mutable region : ValuedCell list = []
    let banner = new Banner()
    
    let GetCell _col _row =
        query { 
            for cell in board.Cells do 
            where (cell.Column = _col && cell.Row = _row) 
            exactlyOneOrDefault 
        }

    let rec GetRegion (input : ValuedCell) = 
        region <- input :: region
        for i = -1 to 1 do
            for j = -1 to 1 do
                match GetCell (input.Column + i) (input.Row + j) with
                    | :? ValuedCell as v ->
                        if not (region |> List.contains(v)) then
                            if v.Value = 0 then v |> GetRegion
                            region <- v :: region
                    | _ -> ()

    let Reset (cell_Callback : MouseEventArgs * Label * CellState * int * int -> unit) =
        board.DropCells()
        board.Panel.Dispose()
        board <- new Board()
        form.Controls.Add(board.Panel)
        board.init(difficulty, cell_Callback, mineCount, size)
        form.ClientSize <- new Size(board.Panel.Size.Width, board.Panel.Size.Height + 38)
        banner.ResetTimerNewGame <| mineCount

    let GetNearestNeighbours (c : ValuedCell) =
        [| for i = -1 to 1 do
            for j = -1 to 1 do
                if not (i = 0 && j = 0) then
                    let neighbour = (query {
                        for cell in board.Cells do
                        where (cell.Column = c.Column + i && cell.Row = c.Row + j && cell.CellState = Unopened)
                        exactlyOneOrDefault
                        })
                    if not (neighbour |> isNull) then
                        yield neighbour |]

    let ResolveNeighbours (nearestNeighbours : seq<BaseCell>) : bool =
        let mutable foundMine = false
        for n in nearestNeighbours do
            if n.CellState = Unopened then
                match n with
                | :? ValuedCell as v ->
                    if v.Value = 0 then
                        v |> GetRegion
                        for cell in region do cell.Reveal() 
                        region <- []
                    else v.Reveal()
                | :? MineCell as m ->
                    m.Detonate()
                    foundMine <- true
                | _ -> ()
        foundMine

    let FlaggedMines () =
        query {
            for c in board.Cells do
            where (c.CellState = Flag)
            count
        }

    let RemainingCells () =
        query {
            for c in board.Cells do
            where (c.CellState = Unopened || c.CellState = Flag)
            count
        }

    let Cell_OnClick (e : MouseEventArgs, _label : Label, _cellState : CellState, _col : int, _row : int) =
        match e.Button with
        | MouseButtons.Left -> 
            if _cellState = Unopened || _cellState = Opened then 
                match GetCell _col _row with
                | :? ValuedCell as c -> 
                    if c.Value = 0 then
                        c.Reveal()
                        c |> GetRegion
                        for cell in region do cell.Reveal() 
                        region <- []
                    elif c.Value > 0 && c.CellState = Unopened then
                        c.Reveal()
                    elif c.Value > 0 && c.CellState = Opened then
                        if c |> GetNearestNeighbours |> ResolveNeighbours then
                            banner.Defeat()
                            board.Panel.Enabled <- false
                    if RemainingCells() = mineCount then 
                        banner.Victory()
                        for cell in board.Cells do 
                            match cell with
                            | :? MineCell as c -> c.AddFlag()
                            | _ -> ()
                        banner.UpdateMineCounter(mineCount - FlaggedMines())
                | :? MineCell as c ->
                    banner.Defeat()
                    for cell in board.Cells do 
                        match cell with
                        | :? MineCell as c -> c.Reveal()
                        | _ -> ()
                    c.Detonate()
                    board.Panel.Enabled <- false
                | _ -> ()
        | MouseButtons.Right ->
            let cell = GetCell _col _row
            cell.UpdateCellState <| match cell.CellState with
                                    | Question_Mark -> Unopened
                                    | Flag -> Question_Mark
                                    | Unopened -> Flag
                                    | Opened -> Opened
            _label.BackgroundImage <- GetResource($"{cell.CellState}")
            banner.UpdateMineCounter(mineCount - FlaggedMines())
        | _ -> ignore()

    let CreateNewStandardGame(_difficulty, _size) = 
        difficulty <- _difficulty
        size <- _size
        mineCount <- int<Difficulty> difficulty
        Reset(Cell_OnClick)
        
    let CreateNewCustomGame(_mineCount, _size) = 
        mineCount <- _mineCount
        size <- _size
        Reset(Cell_OnClick)
       
    let mutable menu = null

    do
        if mineCount = 0 then mineCount <- int<Difficulty> difficulty
        menu <- new MainMenuBar(CreateNewStandardGame, CreateNewCustomGame, form, mineCount, size)
        banner.UpdateMineCounter(mineCount)
        form.Controls.AddRange <| [| board.Panel :> Control; banner.Panel :> Control |]
        board.init(difficulty, Cell_OnClick, mineCount, size)
        banner.SetResetCallback((fun _ -> Reset(Cell_OnClick)))
        form.ClientSize <- new Size(board.Panel.Size.Width, board.Panel.Size.Height + 38)
        form.FormBorderStyle <- FormBorderStyle.FixedSingle
        form.Text <- "Minesweeper"
        form.Menu <- menu.Menu
        form.Icon <- GetIconResource("Icon")

    member this.Form = form
    member this.NewGame() = Reset(Cell_OnClick)