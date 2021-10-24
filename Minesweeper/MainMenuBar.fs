module MainMenuBar

open System.Windows.Forms
open System.Drawing

open Difficulty
open BoardSizes
open CustomGameForm

[<AllowNullLiteral>]
type MainMenuBar (_callBackStandard : Difficulty * Point -> unit, _callBackCustom : int * Point -> unit, _owner, _mineCount : int, _size : Point) =
    let menu = new MainMenu()
    let file = new MenuItem("File")
    let newGame = new MenuItem("New Game")
    let help = new MenuItem("Help")
    let beginner = new MenuItem("Beginner")
    let intermediate = new MenuItem("Intermediate")
    let expert = new MenuItem("Expert")
    let custom = new MenuItem("Custom...")
    let exit = new MenuItem("Exit")
    let about = new MenuItem("About")
    let mutable customGameForm : CustomGameForm = null
    let CreateNewCustomGameForm () =
            customGameForm <- new CustomGameForm(_owner, _mineCount, _size, _callBackCustom)
    do
        beginner.Click.Add(fun _ -> _callBackStandard(Difficulty.Beginner, BoardSizes.Beginner))
        intermediate.Click.Add(fun _ -> _callBackStandard(Difficulty.Intermediate, BoardSizes.Intermediate))
        expert.Click.Add(fun _ -> _callBackStandard(Difficulty.Expert, BoardSizes.Expert))
        custom.Click.Add(fun _ -> CreateNewCustomGameForm())
        exit.Click.Add(fun _ -> Application.Exit())
        about.Click.Add(fun _ -> MessageBox.Show("Created by Swift, purely using the F# language.\n\nView the code here:\nhttps://github.com/ZZAAAKK/Minesweeper\n\nRaise issues here:\nhttps://github.com/ZZAAAKK/Minesweeper/issues", "Minesweeper About", MessageBoxButtons.OK) |> ignore)
        newGame.MenuItems.AddRange <| [| beginner; intermediate; expert; custom; |]
        file.MenuItems.AddRange <| [| newGame; new MenuItem("-"); exit|]
        help.MenuItems.AddRange <| [| about |]
        menu.MenuItems.AddRange <| [| file; help |]

    member this.Menu = menu