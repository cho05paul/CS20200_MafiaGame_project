# CS20200 CLI Mafia game (WereWolf game) Project

## Abstract

This is a term project for **CS-20200: Programming Principles** at KAIST by 20230722 Hanbin Cho.

The project aims to develop a semi-automatic text-based "Game-Master" (GM) for the Werewolf game, widely known as "Mafia" in Korea. The GM automates role assignment, vote counting, ability application, and win-condition evaluation.

**This project does not include chat service. It is meant to be designed as a GM, which enables secret voting, fair win condition evaluation, etc. Players are encouraged to discuss freely in ‘daytime’ by external method.**

Built with **F#** and **.NET 10**.

---

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Verify installation: `dotnet --version` (Expected: `10.x.x`)

### Run

Windows: 

run.bat


Unix / macOS: 

chmod +x run.sh


./run.sh


Or directly

dotnet run

### Build

dotnet build

### How to Play

## Board (UI) Layout

The game board displays player statuses in a grid format:

- Players are numbered 1 to n, which represents the turn order.
- Layout: Single line of squares.
- The UI updates dynamically after each night/day to reflect deaths and abilities.

---

#### Plan

Squares numbered 1~n (which user specified) in one line

---

### Basic Setup

To initialize the game, you should enter basic settings including the number of total players.

Here is the basic format for initialization command:

=== CS20200 CLI Mafia Game ===
Press Enter to start.

Game Master Initialization

Enter total number of players: 8
Enter number of Mafias: 2
Enter number of Doctors: 1
Enter number of Sheriffs: 1
Enter number of civilian special roles(Max. 3): 3
Enter number of mafian special roles(Max. 2): 1
Enter turn limit: 7

---

### Taking Turns

The game starts from nighttime, repeating night and day.

For each day/night, all 'alive' players should take their turns, using their 'abilities' at night and 'voting' at day.

For each player's turn,

1. You are asked to type "Yes" and press enter, to prevent your previous player from knowing your role.
2. The current turn, board, your role and number is printed.
3. You are prompted: "Your vote (1-n):" or "Select a player to use ability (1-n):" or "You have no special abilities tonight. Type 'Yes' to skip:"
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

- If there is no more Mafia members alive, Civilian win.
- If the number of remaining Mafia members equals or exceeds the number of remaining Civilian members, Mafia win.

### Roles

- Civilian
  - Civilian: No special ability. (Default role for civilian members)
  - Doctor: Heal a player every night to prevent him from murdered by mafias. The heal is only noticed when a player's life is saved.
  - Sheriff: Investigate a player every night to find out whether that player is a mafia or not. A sheriff cannot find out mafian special roles, like spy or hostess.
  - Soldier: Can block mafia's murder once per a game. Can discover mafian special role who used its ability to him.
  - Politician: Not executed by voting.
  - GraveRobber: Get a role of player who is killed in the first night
- Mafian
  - For all mafian special roles: If they use their ability to mafia, they can send a message (<100 letters) to mafias that night. (Sent to mafias the next day)
  - Spy: Can check one player's role every night.
  - Hostess: Neutralize the ability of voting target until next night ends. (Ex. If a hostess vote to a politician, he can be executed by voting in that day) Mafia teammates are not tempted by hostess.

---

## Requirement Changes
Because if mafian special role's order is prior to mafias, the message cannot be handed to mafias. Therefore, the message by spy(written at night) is shown next day(voting phase), and the message by hostess(written at voting phase) is shown next night.


---

## Author

Name: Hanbin Cho

Affiliation: 20230722

## Use of Large Language Models (LLM)

What I used the LLM for: Grammatical/content revision for README.md and Requirements document (May be added if LLM is used for further development)

## Reference

Ideas got from **Mafia42** by *TEAM42*

