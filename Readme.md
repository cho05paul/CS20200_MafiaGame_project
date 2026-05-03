# CS20200 CLI Mafia game (WereWolf game) Project

## Abstract

This is a term project for CS-20200: Programing Principles in KAIST by 20230722 Hanbin Cho.

This project aims to develop a semi-automatic text-based client of 'Werewolf game', which is widely known as 'Mafia game' in Korea.

This project is disigned to act as a automatic "Game-Master" of the Mafia game (WereWolf game), which assigns roles, counts votes, apply abilites, and evaluate game condition.

Built with F# and .NET 10

## Getting started



### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

  Verify with: '''dotnet --version''' (should show '''10.x.x''')

### Run


*TBA*


### Build

*TBA*

### How to Play

## Board (UI) Layout

*TBA*

---

#### Plan

Squares numbered 1~n (which user specified) in two lines (one line if n < 5)

---

### Basic Setup

To initialize the game, you should enter basic settings including the number of total players.

Here is the basic format for initialization command:

*TBA*

---

#### Plan (What GM can set)

- Number of total players
- Whether to reveal dead players' roles (Default: false)
- Whether to include 'special roles' and how many special roles should be included (Default: 0, Max: TBA)
- Time limit (Default: infinite)
- Number of Mafias(Werewolves), Doctors, Sheriffs(Seer) (Not mandatory)

---

### Taking Turns

The game starts from nighttime, repeating night and day.

For each day/night, all 'alive' players should take their turns, using their 'abilities' at night and 'voting' at day.

For each player's turn,

1. You are asked to type "Yes" and press enter, to prevent your previous player from knowing your role.
2. The current board and role is printed.
3. You are prompted: '''Your vote (1-n):''' or '''Use your ability to (1-n):''' or '''You have nothing to do. Type your own number(x):'''
4. Type an appropriate number and press Enter.
5. Hand the terminal to the next player.

---

- At the end of each night, the most voted player (voted by mafia) is murdered.
  - Doctors can vote for a player to 'heal', which prevents him from murder.
- At the end of each day, the most voted player is executed.

- Some special roles can use their 'abilities' during day or night.
  For more informations, refer to 'Roles' section.

---

### End of game

At the end of each night/day, the game automatically checks 'Game condition'.

- If there is no more Mafias or Mafian special roles alive, Civilians win.
- 

### Roles

*TBA*

---

#### Planned Roles

- Civilian
  - Civilian: No special ability. (Default role for civilian team)
  - Soldier: Can block mafia's murder once per a game. Can discover mafian special role who used its ability to him.
  - Politician: Not executed by voting.
  - GraveRobber: Get a role of player who is killed in the first night
- Mafian
  - For all mafian special roles: If they use their ability to mafia, they can send a message (<100 letters) to mafias that night.
  - Spy: Can check one player's role every night.
  - Hostess: Neutralize the ability of voting target until next night ends. (Ex. If a hostess vote to a politician, he can be executed by voting in that day)

---

## Author

Name: Hanbin Cho

Affiliation: 20230722

## Reference

*TBA*

Ideas got from **Mafia42** by *TEAM42*

