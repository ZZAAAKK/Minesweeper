module Banner

open System
open System.Windows.Forms
open System.Drawing
open System.Text

open MinesweeperResourceManager

type Banner () =
    let panel = new Panel()
    let resetButton = new Button()
    let timer = new Label()
    let mutable gameTicks = 0
    let mineCounter = new Label()
    let controls = [| resetButton :> Control; timer :> Control; mineCounter :> Control |]
    let t = new Timers.Timer(1000.)
    let PadLeftInt n =
        let mutable ret = [ for c in n.ToString().ToCharArray() do yield c ]
        while ret.Length < 3 do ret <- '0' :: ret
        let sb = new StringBuilder()
        for c in ret do c |> sb.Append |> ignore
        sb.ToString()
    let ResetTimer _ =
        gameTicks <- 0
        timer.Text <- PadLeftInt <| gameTicks
        t.Stop()
        t.Start()
    do
        panel.Dock <- DockStyle.Top
        panel.Height <- 38
        panel.Font <- new Font("Seven Segment", 28f)
        resetButton.Size <- new Size(36, 36)
        resetButton.Left <- panel.Width - (resetButton.Width / 2)
        resetButton.Click.Add(ResetTimer)
        resetButton.BackgroundImage <- GetResource("Reset_Normal")
        resetButton.BackgroundImageLayout <- ImageLayout.Stretch
        timer.Width <- 72
        timer.Dock <- DockStyle.Left
        timer.TextAlign <- ContentAlignment.TopCenter
        timer.Text <- PadLeftInt <| gameTicks
        timer.BackColor <- Color.Black
        timer.ForeColor <- Color.Red
        mineCounter.Width <- 72
        mineCounter.Dock <- DockStyle.Right
        mineCounter.TextAlign <- ContentAlignment.TopCenter
        mineCounter.Text <- PadLeftInt <| 0
        mineCounter.BackColor <- Color.Black
        mineCounter.ForeColor <- Color.Red
        panel.Controls.AddRange(controls)

        t.Elapsed.Add(fun _ -> 
            timer.Invoke(
                Action(fun () ->
                    gameTicks <- gameTicks + 1
                    timer.Text <- PadLeftInt <| gameTicks
                )
            ) |> ignore
        )

        t.Start()

    member this.Panel = panel
    member this.SetResetCallback _callBack =
        resetButton.Click.Add(_callBack)
    member this.UpdateMineCounter n =
        mineCounter.Text <- PadLeftInt <| n
    member this.ResetTimerNewGame n =
        resetButton.BackgroundImage <- GetResource("Reset_Normal")
        timer.Text <- PadLeftInt <| gameTicks
        n |> this.UpdateMineCounter
        t.Start()
    member this.StopTimer () =
        t.Stop()
        gameTicks <- 0
    member this.Defeat () =
        resetButton.BackgroundImage <- GetResource("Reset_Defeat")
        this.StopTimer()
    member this.Victory () =
        resetButton.BackgroundImage <- GetResource("Reset_Victory")
        this.StopTimer()