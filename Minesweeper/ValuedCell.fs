﻿module ValuedCell

open System
open System.Windows.Forms
open System.Drawing

open BaseCell
open CellState
open ValuedCellState
open MinesweeperResourceManager

type ValuedCell (_value : int, _col : int, _row : int, _callBack : MouseEventArgs * Label * int * CellState * int * int -> unit) =
    inherit BaseCell(_col, _row)
    let mutable value = _value
    let mutable valuedCellState = Covered
    let mutable cellState = Unopened
    let label = new Label()
    let Label_OnClick (e : MouseEventArgs) =
        match e.Button with
        | MouseButtons.Left -> 
            if valuedCellState = Covered then
                valuedCellState <- Uncovered
        | MouseButtons.Right ->
            if not (valuedCellState = Uncovered) then
                match cellState with
                | Question_Mark -> 
                    cellState <- Unopened
                | Flag -> 
                    cellState <- Question_Mark
                | Unopened -> 
                    cellState <- Flag
                label.BackgroundImage <- GetResource($"{cellState}")
        | _ -> ()
    do
        let image = GetResource($"{cellState}")
        label.Size <- new Size(image.Size.Height / 2, image.Size.Width / 2)
        label.BackgroundImageLayout <- ImageLayout.Stretch
        label.BackgroundImage <- image
        label.Left <- (image.Width / 2) * _col
        label.Top <- (image.Height / 2) * _row
        label.MouseClick.Add(fun e -> _callBack(e, label, value, cellState, _col, _row))
        label.MouseClick.Add(Label_OnClick)
    
    member this.Value = value
    member this.Reveal() = 
        valuedCellState <- Uncovered
        label.BackgroundImage <- GetResource($"Minesweeper_{value}")
    member this.IncrementValue() = value <- value + 1
    member this.ValuedCellState = valuedCellState
    override this.CellState = cellState
    override this.Label = label