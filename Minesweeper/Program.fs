open System
open System.Windows.Forms

open Interface

let game = new Interface()

[<STAThread>]
[<EntryPoint>]
let main argv =
    Application.Run(game.Form)
    0