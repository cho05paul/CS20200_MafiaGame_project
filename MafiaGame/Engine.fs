module Engine

open System
open DataTypes

let checkWinCondition (players: Player list, turn: int, turnLimit: int option) =
    let alive = players |> List.filter (fun p -> p.IsAlive)
    let aliveMafias = alive |> List.filter (fun p -> p.Role = Mafia || p.Role = Spy || p.Role = Hostess) |> List.length
    let aliveCivilians = alive.Length - aliveMafias

    if aliveMafias = 0 then CivilianWins
    elif aliveMafias >= aliveCivilians then MafiaWins
    elif turnLimit < Some turn then MafiaWins
    else Ongoing

let evaluateNight (state: GameState) =
    // Determine the result of the Mafia's murder
    let result =
        match state.MafiaTarget with
        | Some target ->
            if state.DoctorHeal = Some target then 
                Healed target
            else
                let targetPlayer = state.Players |> List.find (fun p -> p.Id = target)
                // Soldier block check: Active if they are a Soldier, haven't used the ability, and aren't tempted.
                if targetPlayer.Role = Soldier && not targetPlayer.UsedAbility && not targetPlayer.Temptation then
                    BlockedBySoldier target
                else
                    Murdered target
        | None -> Peaceful

    // Update player statuses (deaths, abilities, and clearing temptation)
    let updatePlayer p =
        match result with
        | Murdered id when id = p.Id -> { p with IsAlive = false; Temptation = false }
        | BlockedBySoldier _ when p.Role = Soldier && p.Id = Option.get state.MafiaTarget -> 
            { p with UsedAbility = true; Temptation = false }
        | _ -> { p with Temptation = false } // Hostess temptation expires at the end of the night

    let updatedPlayers = List.map updatePlayer state.Players

    // GraveRobber Logic: On Night 1, if someone is murdered, take their role.
    let finalPlayers =
        if state.TurnNumber = 1 then
            match result with
            | Murdered id ->
                let deadRole = (state.Players |> List.find (fun p -> p.Id = id)).Role
                updatedPlayers |> List.map (fun p -> 
                    if p.Role = GraveRobber && not p.Temptation then { p with Role = deadRole } 
                    else p)
            | _ -> updatedPlayers
        else updatedPlayers

    let nextState = { state with Players = finalPlayers; CurrentPhase = Day; MafiaTarget = None; DoctorHeal = None }
    result, nextState

let evaluateDay (state: GameState) =
    let voteCounts =
        state.Votes
        |> Map.toSeq
        |> Seq.map snd // extract the targets
        |> Seq.countBy id
        |> Seq.sortByDescending snd
        |> Seq.toList

    let targetIdOpt =
        match voteCounts with
        | (target, count) :: (_, count2) :: _ when count = count2 -> None // Tie, nobody is executed
        | (target, _) :: _ -> Some target
        | [] -> None

    let result =
        match targetIdOpt with
        | Some id ->
            let targetPlayer = state.Players |> List.find (fun p -> p.Id = id)
            // If the target is a Politician and wasn't tempted by the Hostess, they use 'Way of Conduct'
            if targetPlayer.Role = Politician && not targetPlayer.Temptation then
                Conduct id
            else
                Executed id
        | None ->
            NoExecution
    
    let updatePlayer p =
        match result with
        | Executed id when id = p.Id -> { p with IsAlive = false }
        | _ -> p
    
    let updatedPlayers = List.map updatePlayer state.Players

    let nextState = { state with Players = updatedPlayers; CurrentPhase = Night; TurnNumber = state.TurnNumber + 1; Votes = Map.empty }
    result, nextState