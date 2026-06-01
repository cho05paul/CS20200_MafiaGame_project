module Game

open System
open DataTypes
open Engine

let clearScreen () =
    for _ in 1..50 do printfn ""

let printBoard (players: Player list) =
    printfn "\n========== Players =========="
    let printIds = players |> List.map (fun p -> if not p.IsAlive then "X" else string p.Id)
    printfn "[ %s ]" (String.concat " | " printIds)
    printfn "=============================\n"

let getValidTarget (players: Player list) prompt =
    let aliveIds = players |> List.filter (fun p -> p.IsAlive) |> List.map (fun p -> p.Id)
    let rec loop () =
        printf "%s" prompt
        match Int32.TryParse(Console.ReadLine()) with
        | true, n when List.contains n aliveIds -> n
        | _ ->
            printfn "Invalid target. Please enter the ID of an alive player."
            loop ()
    loop ()

let maskTurn playerId =
    let rec loop () =
        printfn "\nPlayer %d, type 'Yes' and press Enter to begin your turn (this ensures secrecy):" playerId
        let input = Option.ofObj (Console.ReadLine()) |> Option.defaultValue ""
        if input.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) then ()
        else loop ()
    loop ()
    clearScreen ()

let runNightTurn (player: Player) (state: GameState) =
    maskTurn player.Id
    printfn "Night %d" state.TurnNumber
    printBoard state.Players
    printfn "Your Secret Role: %A\n" player.Role
    printfn "Your Number: %d\n" player.Id

    if player.Temptation = true then
        printfn "You are tempted by hostess! You cannot use any ability today."
        printf "Type 'Yes' to skip: "
        Console.ReadLine() |> ignore
        state
    else
        match player.Role with
        | Civilian | Soldier | GraveRobber | Politician | Hostess ->
            printfn "You have no special abilities tonight."
            printf "Type 'Yes' to skip: "
            Console.ReadLine() |> ignore
            state
        | Doctor ->
            let target = getValidTarget state.Players "Select a player to heal (1-n): "
            { state with DoctorHeal = Some target }
        | Sheriff ->
            let target = getValidTarget state.Players "Select a player to investigate (1-n): "
            let targetRole = (state.Players |> List.find (fun p -> p.Id = target)).Role
            let isMafia = if targetRole = Mafia then "a Mafia" else "NOT a Mafia"
            printfn "\n[Investigation Result] Player %d is %s." target isMafia
            printfn "Press Enter to end your turn."
            Console.ReadLine() |> ignore
            state
        | Mafia ->
            let target = getValidTarget state.Players "Select a player to murder (1-n): "
            { state with MafiaVotes = target :: state.MafiaVotes }
        | Spy ->
            let target = getValidTarget state.Players "Select a player to investigate (1-n): "
            let targetRole = (state.Players |> List.find (fun p -> p.Id = target)).Role
            printfn "\n[Investigation Result] Player %d is %s." target (string targetRole)
            printfn "Press Enter to end your turn."
            Console.ReadLine() |> ignore
            state


let runDayTurn (player: Player) (state: GameState) =
    maskTurn player.Id
    printfn "Night %d" state.TurnNumber
    printBoard state.Players
    printfn "Your Role: %A\n" player.Role
    printfn "Your Number: %d\n" player.Id
    let target = getValidTarget state.Players "Select a player to vote for execution (1-n): "
    let updatedPlayers =
        if player.Role = Hostess then
            state.Players |> List.map (fun p -> if p.Id = target then { p with Temptation = true } else p)
        else
            state.Players
    { state with Votes = Map.add player.Id target state.Votes; Players = updatedPlayers }

let initGame playerCount mafias doctors sheriffs civSpRoles mafSpRoles turnLimit =
    let rnd = Random()

    let civSpPool = [Soldier; Politician; GraveRobber] |> List.sortBy (fun _ -> rnd.Next())
    let mafSpPool = [Spy; Hostess] |> List.sortBy (fun _ -> rnd.Next())

    let actualCivSp = civSpPool |> List.take (min civSpRoles civSpPool.Length)
    let actualMafSp = mafSpPool |> List.take (min mafSpRoles mafSpPool.Length)
    
    let roles =
        List.init mafias (fun _ -> Mafia) @
        List.init doctors (fun _ -> Doctor) @
        List.init sheriffs (fun _ -> Sheriff) @
        actualCivSp @
        actualMafSp

    let civilians = playerCount - roles.Length
    let allRoles = roles @ List.init civilians (fun _ -> Civilian)

    let shuffledRoles = allRoles |> List.sortBy (fun _ -> rnd.Next())

    let players =
        shuffledRoles
        |> List.mapi (fun i r -> { Id = i + 1; Role = r; IsAlive = true; UsedAbility = false; Temptation = false })

    { Players = players; CurrentPhase = Night; TurnNumber = 1; MafiaVotes = []; DoctorHeal = None; Votes = Map.empty }

let rec gameLoop (state: GameState, turnLimit: int option) =
    match checkWinCondition (state.Players, state.TurnNumber, turnLimit) with
    | MafiaWins ->
        printfn "\n======================="
        printfn "      MAFIA WINS!      "
        printfn "======================="
    | CivilianWins ->
        printfn "\n======================="
        printfn "    CIVILIANS WIN!     "
        printfn "======================="
    | Ongoing ->
        let alivePlayers = state.Players |> List.filter (fun p -> p.IsAlive)

        if state.CurrentPhase = Night then
            printfn "\n>>> Night %d begins..." state.TurnNumber
            let finalState =
                alivePlayers |> List.fold (fun st p -> 
                    clearScreen()
                    runNightTurn p st
                ) state

            clearScreen ()
            let outcome, nextState = evaluateNight finalState
            
            // Handle new NightResult Types
            match outcome with
            | Murdered id -> printfn "The sun rises... Player %d was found murdered." id
            | Healed id -> printfn "The sun rises... The Doctor saved Player %d tonight!" id
            | BlockedBySoldier id -> printfn "The sun rises... Soldier %d bravely blocked the Mafia's attack!" id
            | Peaceful -> printfn "The sun rises... It was a peaceful night."
            
            printfn "\nPress Enter to begin daytime discussions."
            Console.ReadLine() |> ignore
            gameLoop (nextState, turnLimit)
        else
            printfn "\n>>> Day %d begins..." state.TurnNumber
            let finalState =
                alivePlayers |> List.fold (fun st p -> 
                    clearScreen()
                    runDayTurn p st
                ) state

            clearScreen ()
            let result, nextState = evaluateDay finalState
            
            match result with
            | Executed id -> 
                printfn "The votes are in... Player %d was executed by the town." id
            | Conduct id -> 
                printfn "The votes are in... But Player %d is a Politician! His 'Way of Conduct' saved himself from execution." id
            | NoExecution -> 
                printfn "The votes are tied... Nobody was executed today."
            
            printfn "\nPress Enter to begin the night phase."
            Console.ReadLine() |> ignore
            gameLoop (nextState, turnLimit)

let GameSetup () = 
    printfn "Game Master Initialization\n"

    printf "Enter total number of players: "
    let playerCount = Int32.Parse(Console.ReadLine())
    
    printf "Enter number of Mafias: "
    let mafias = Int32.Parse(Console.ReadLine())
    
    printf "Enter number of Doctors: "
    let doctors = Int32.Parse(Console.ReadLine())
    
    printf "Enter number of Sheriffs: "
    let sheriffs = Int32.Parse(Console.ReadLine())

    printf "Enter number of civilian special roles(Max. 3): "
    let civSpRoles = Int32.Parse(Console.ReadLine())

    printf "Enter number of mafian special roles(Max. 2): "
    let mafSpRoles = Int32.Parse(Console.ReadLine())

    if playerCount < mafias + doctors + sheriffs + civSpRoles + mafSpRoles then
        printfn "Invalid number of player set."
    else
        printf "Enter turn limit: "
        let turnLimit = Int32.Parse(Console.ReadLine())

        let initialState = initGame playerCount mafias doctors sheriffs civSpRoles mafSpRoles (Some turnLimit)
        
        printfn "\nSetup complete. Hand terminal to Player 1."
        gameLoop (initialState, Some turnLimit)
    0
