# PugSharp



[![PugSharp_test_and_build](https://github.com/Lan2Play/PugSharp/actions/workflows/test_and_build.yml/badge.svg)](https://github.com/Lan2Play/PugSharp/actions/workflows/test_and_build.yml)
[![PugSharp_website_build](https://github.com/Lan2Play/PugSharp/actions/workflows/website_build.yml/badge.svg)](https://github.com/Lan2Play/PugSharp/actions/workflows/website_build.yml)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Lan2Play_PugSharp&metric=bugs)](https://sonarcloud.io/summary/new_code?id=Lan2Play_PugSharp)
<!-- [![Translation status](https://translate.lan2play.de/widgets/netevent-client/-/netevent-client/svg-badge.svg)](https://translate.lan2play.de/engage/netevent-client/) -->

Pugsharp is a PUG System Plugin for CS2 based on the awsome [CounterStrikeSharp by roflmuffin](https://github.com/roflmuffin/CounterStrikeSharp). Its intended purpose is to be used with our fork of [eventula](https://github.com/Lan2Play/eventula-manager), but ofc can be used in a different environment as well.


> **Warning**
> This Plugin is in a very early state of development and is not fully working right now! We keep you updated on our discord below, if you are interested in using it.

You can find the full documentation on [pugsharp.lan2play.de](https://pugsharp.lan2play.de) .


If you want to help developing or translating, join our discord:


[![Discord](https://discordapp.com/api/guilds/748086853449810013/widget.png?style=banner3)](https://discord.gg/zF5C9WPWFq)

## Working features

- [x] Configuration via http(s) json (example below) 
- [ ] api reporting to a http(s) server
- [x] automatic team assignment
- [x] map and starting team vote
- [x] automatic pause if player disconnects
- [x] pause / unpause feature
- [ ] demo recording 
- [ ] demo upload
- [ ] hltv support
- [ ] i18n


## Usage

> **Warning**
> Don't use this in production right now!
>

If you want to know how to use PugSharp, hop over to our [Documentation](https://pugsharp.lan2play.de) .

## Commands

- `!ready` Mark the player as ready
- `!pause` Pause the match in the next freezetime
- `!unpause` Unpause the match. To continue the match, both teams have to !unpause.

### Admin/Rcon Commands

- `!ps_loadconfig <url>` Load a [MatchConfig](#MatchConfig) to initialize a match 
- `!ps_dumpmatch` Dumps the current matchstate and config to console

## Configuration

### MatchConfig

| Field                    | Description                                                                                  |
|--------------------------|----------------------------------------------------------------------------------------------|
| maplist                  | List of availbale maps for the map vote                                                      |
| team1                    | [Team](TODO Link) Description                                                                |
| team2                    | [Team](TODO Link) Description                                                                |
| matchid                  | Unique Identifier for the match                                                              |
| num_maps                 | Number of Maps to be played. This should be an odd number to be able to determine an winner. |
| players_per_team         | Maximum possible number of players per team.                                                 |
| min_players_to_ready     | Number of players per team, that have to be ready to start the game.                         |
| eventula_apistats_url    | Url where the Game State have to be send.                                                    |
| eventula_demo_upload_url | Url to upload the game demo to [Eventula](https://github.com/Lan2Play/eventula-manager)      |

**Example Config**
```json
{
    "maplist": [
        "de_vertigo",
        "de_dust2",
        "de_inferno",
        "de_mirage",
        "de_nuke",
        "de_overpass",
        "de_ancient"
    ],
    "team1": {
        "name": "hallo",
        "tag": "hallo",
        "flag": "DE",
        "players": {
            "12345678901234567": "Apfelwurm",
            "12345678901234568": "strange name"
        }
    },
    "team2": {
        "name": "asd",
        "tag": "asd",
        "flag": "DE",
        "players": {
            "12345678901234569": "BOT R00st3r",
            "76561198064576360": "heatwave"
        }
    },
    "matchid": "40",
    "num_maps": 1,
    "players_per_team": 2,
    "min_players_to_ready": 2,
    "eventula_apistats_url": "https:\/\/dev.lan2play.de\/api\/matchmaking\/40\/",
    "eventula_demo_upload_url": "https:\/\/dev.lan2play.de\/api\/matchmaking\/40\/demo"
}
```


### ServerConfig

| Field                    | Description                                                                                  |
|--------------------------|----------------------------------------------------------------------------------------------|
| admins                   | List of admins with the steamId and a Name                                                   |

**Example Config**
```json
{
    "admins": {
        "12345678901234569": "BOT R00st3r",
        "12345678901234567": "Apfelwurm"
    }
}
```




<!-- 
## Tanslation

[![Translation status](https://translate.lan2play.de/widgets/eventula-manager/-/multi-auto.svg)](https://translate.lan2play.de/engage/eventula-manager/) -->


