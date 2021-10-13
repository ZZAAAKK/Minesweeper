module BoardSizes

open System.Drawing

type BoardSizes = 
    static member Beginner = new Point(10, 10)
    static member Intermediate = new Point(15, 13)
    static member Expert = new Point(30, 16)