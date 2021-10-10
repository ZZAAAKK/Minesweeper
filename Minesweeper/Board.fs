module Board

open System
open System.Drawing
open System.Windows.Forms

open Difficulty
open BoardSizes
open BaseCell
open MineState
open ValuedCellState
open MineCell
open ValuedCell

type Board (?_difficulty : Difficulty, ?_mineCellCallBack : EventArgs * Label -> unit, ?_valuedCellCallBack : EventArgs * Label * int -> unit, ?_mineCount : int, ?_size : Point) =
    let panel = new Panel()
    let cells : BaseCell list = []
    member this.init(_difficulty : Difficulty, _mineCellCallBack : EventArgs * Label -> unit, _valuedCellCallBack : EventArgs * Label * int -> unit, _mineCount : int, _size : Point) =
        let mineCount = match _mineCount with
                        | n when n > 0 -> n
                        | _ -> int<Difficulty> _difficulty
        let size = match _size with
                        | s when not (s.IsEmpty) -> s
                        | _ -> match _difficulty with
                                    | Difficulty.Beginner -> BoardSizes.beginner
                                    | Difficulty.Intermediate -> BoardSizes.intermediate
                                    | Difficulty.Expert -> BoardSizes.expert
                                    | _ -> new Point()
        let rand = new Random()
        let cells : BaseCell[] = [| for x = 0 to size.X - 1 do
                                        for y = 0 to size.Y - 1 do
                                            yield new ValuedCell(0, x, y, _valuedCellCallBack) :> BaseCell |]

        panel.Size <- new Size(size.X * 38, size.Y * 38)
        let mutable mineCells : BaseCell list = []
        while mineCells.Length < mineCount do
            let rec GetRandomCell () =
                let index = rand.Next(0, cells.Length)
                if not (mineCells |> List.contains(cells.[index])) then
                    cells.[index] <- new MineCell(cells.[index].Column, cells.[index].Row, _mineCellCallBack) :> BaseCell
                    mineCells <- cells.[index] :: mineCells
                    for i = -1 to 1 do
                        for j = -1 to 1 do
                            match (query {
                                for cell in cells do
                                where (cell.Column = cells.[index].Column + i && cell.Row = cells.[index].Row + j)
                                exactlyOneOrDefault
                                }) with
                            | :? ValuedCell as v -> v.IncrementValue()
                            | _ -> ignore()
                else
                    GetRandomCell()
            GetRandomCell()
            
        panel.Controls.AddRange <| [| for cell in cells do yield cell.Label |]

    member this.Cells = cells
    member this.Panel = panel

    new () =
        Board(Difficulty.Beginner)

    new (_mineCellCallBack : EventArgs * Label -> unit, _valuedCellCallBack : EventArgs * Label * int -> unit) = 
        Board(Difficulty.Beginner, _mineCellCallBack, _valuedCellCallBack)

    new (_mineCellCallBack : EventArgs * Label -> unit, _valuedCellCallBack : EventArgs * Label * int -> unit, _mineCount : int, _size : Point) =
        Board(Difficulty.Beginner, _mineCellCallBack, _valuedCellCallBack, _mineCount, _size)