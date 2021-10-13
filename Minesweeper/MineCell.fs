module MineCell

open System
open System.Windows.Forms
open System.Drawing

open BaseCell
open MineState
open CellState
open MinesweeperResourceManager

type MineCell (_col : int, _row : int, _callBack : MouseEventArgs * Label * CellState -> unit) =
    inherit BaseCell(_col, _row)
    let mutable mineState = Revealed
    let mutable cellState = Unopened
    let label = new Label()
    let Label_OnClick (e : MouseEventArgs) =
        match e.Button with
        | MouseButtons.Left -> 
            mineState <- Exploded
        | MouseButtons.Right ->
            if not (mineState = Exploded) then
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
        label.MouseClick.Add(fun e -> _callBack(e, label, cellState))
        label.MouseClick.Add(Label_OnClick)

    member this.Reveal() = 
        mineState <- Missed
        label.BackgroundImage <- GetResource($"Mine_{mineState}")
    member this.Detonate() =
        mineState <- Exploded
        label.BackgroundImage <- GetResource($"Mine_{mineState}")
    override this.CellState = cellState
    override this.Label = label