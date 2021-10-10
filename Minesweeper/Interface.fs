module Interface

open System
open System.Windows.Forms
open System.Drawing

open Board
open Difficulty
open MineState
open ValuedCellState
open ValuedCellImages
open MineCell
open ValuedCell
open BoardSizes

type Interface () =
    let form = new Form()
    let mutable board = new Board()
    let mutable difficulty = Difficulty.Beginner
    let mutable mineCount = 0
    let mutable size = BoardSizes.beginner
    let ValuedCell_OnClick (e : EventArgs, _label : Label, _value : int) =
        _label.BackgroundImage <- Image.FromFile(ValuedCellImages.[_value])
    let rec MineCell_OnClick (e : EventArgs, _label : Label) =
        _label.BackgroundImage <- Image.FromFile(MineState.Exploded)
        match MessageBox.Show("You lose!\n\nPlay again?", "Game Over", MessageBoxButtons.YesNo) with
        | DialogResult.Yes -> 
            board.Panel.Dispose()
            board <- new Board()
            form.Controls.Add(board.Panel)
            board.init(difficulty, MineCell_OnClick, ValuedCell_OnClick, mineCount, size)
            form.ClientSize <- board.Panel.Size
        | _ -> ()
    do
        form.Controls.Add(board.Panel)
        board.init(difficulty, MineCell_OnClick, ValuedCell_OnClick, mineCount, size)
        form.ClientSize <- board.Panel.Size

    member this.Form = form