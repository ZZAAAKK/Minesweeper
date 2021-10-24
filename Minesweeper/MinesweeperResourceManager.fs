module MinesweeperResourceManager

open System.Drawing
open System.Resources
open System.Reflection

let GetResource(resourceName : string) : Image =
    ResourceManager("Minesweeper.Resources.Resource", Assembly.GetCallingAssembly()).GetObject(resourceName) :?> Image

let GetIconResource(resourceName : string) =
    ResourceManager("Minesweeper.Resources.Resource", Assembly.GetCallingAssembly()).GetObject(resourceName) :?> Icon