open System
open Game

[<EntryPoint>]
let main _ =
    printfn "=== CS20200 CLI Mafia Game ==="
    printfn "Press Enter to start."
    Console.ReadLine () |> ignore
    let rec infiniteloop () =
        GameSetup () |> ignore
        printfn("Press Enter to start a new game.")
        Console.ReadLine () |> ignore
        clearScreen ()
        infiniteloop ()
    infiniteloop ()