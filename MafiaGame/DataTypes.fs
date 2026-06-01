module DataTypes

open System

type Role =
    | Civilian
    | Doctor
    | Sheriff
    | Soldier
    | Politician
    | GraveRobber
    | Mafia
    | Spy
    | Hostess

type Phase =
    | Night
    | Day

type Player = {
    Id: int
    Role: Role
    IsAlive: bool
    UsedAbility: bool
    Temptation: bool
}

type GameState = {
    Players: Player list
    CurrentPhase: Phase
    TurnNumber: int
    MafiaVotes: int list
    DoctorHeal: int option
    Votes: Map<int, int> // VoterId -> TargetId
    SpyMessage: string list
    HostessMessage: string list
}

type NightResult =
    | Murdered of int
    | Healed of int
    | BlockedBySoldier of int
    | Peaceful

type DayResult = 
    | Executed of int
    | Conduct of int
    | NoExecution

type GameResult =
    | CivilianWins
    | MafiaWins
    | Ongoing