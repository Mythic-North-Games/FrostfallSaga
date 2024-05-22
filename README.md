# FrostFall-Saga
<img src="https://mythic-north-games.github.io/FrostfallSaga/coverage/Report/badge_shieldsio_linecoverage_orange.svg">

The latest development version of the game can be played here: <https://mythic-north-games.github.io/FrostfallSaga/>

## Setup project

### Requirements

- [.NET](https://dotnet.microsoft.com/en-us/download) version 7 or more
- [Unity](https://unity.com/download) version `2022.3.26f1`
- [NodeJS](https://nodejs.org/dist/v21.4.0/) version `21.4.0`

### Installation

1. Make sure you have all the requirements correctly installed on your system
2. Clone or pull this repository
3. Install and setup commit linter and pre commit by running `npm install`.

> Now, every time you want to commit, a formatter will automatically format the staged `.cs` files for you before commiting, then a commit linter applying the [conventional commit](https://www.conventionalcommits.org/en/v1.0.0/) convention will prevent you from writing bad formatted commits.

## Contributing

### Conventions to read and follow

#### Architecture

- [SOLID](https://www.digitalocean.com/community/conceptual-articles/s-o-l-i-d-the-first-five-principles-of-object-oriented-design)
- [KISS](https://medium.com/@symflower/programming-principle-kiss-keep-it-simple-stupid-c428784acb71)

#### Naming and coding conventions

- [Microsoft C# naming conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
- [Microsoft C# coding conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Unity naming and coding conventions](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity)
- [Unity scene structure conventions](https://github.com/justinwasilenko/Unity-Style-Guide?tab=readme-ov-file#29-scene-structure)
- [Unity resources naming conventions](https://github.com/justinwasilenko/Unity-Style-Guide?tab=readme-ov-file#4-asset-naming-conventions)

#### Personal conventions document

- [Architecture, code and development Guidelines](https://frostfall-saga.atlassian.net/wiki/spaces/FUSYB/pages/23298453/Architecture+code+and+development+Guidelines)

### Use the Github workflows to test your development

Because of the 2000 minutes of free runners availability, only the [Only tests 🧪](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/test.yml) workflow is triggered automatically on PR and pushes.
If you want to test the builds, you need to manually trigger one of these two jobs:
- [Only build for Windows 🏗️](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/build-windows.yml) if you don't want to deploy your version to GitHub pages
- [Build WebGL and Deploy to GitHub Pages 🚀](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/build-webgl-and-deploy.yml) if you want to deploy your version to GitHub Pages.

> ⚠️ Only one version of Github Pages can live at a given time. Deploying to Github Pages will **override** the existing deployed version.

## Miscellaneous

- [Latest full test coverage report](https://mythic-north-games.github.io/FrostfallSaga/coverage/Report/)
