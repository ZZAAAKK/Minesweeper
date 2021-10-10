module BoardSizes

open System.Drawing

type BoardSizes = 
    static member beginner = new Point(10, 10)
    static member intermediate = new Point(15, 13)
    static member expert = new Point(30, 16)