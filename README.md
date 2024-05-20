# FrostFall-Saga

The latest development version of the game can be played here: <https://mythic-north-games.gitlab.io/games/frostfall-saga/>

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

### Use the Github workflows to test your development

Because of the 2000 minutes of free runners availability, only the [Only tests 🧪](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/test.yml) workflow is triggered automatically on PR and pushes.
If you want to test the builds, you need to manually trigger one of these two jobs:
- [Only build for Windows 🏗️](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/build-windows.yml) if you don't want to deploy your version to GitHub pages
- [Build WebGL and Deploy to GitHub Pages 🚀](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/build-webgl-and-deploy.yml) if you want to deploy your version to GitHub Pages.

> ⚠️ Only one version of Github Pages can live at a given time. Deploying to Github Pages will **override** the existing deployed version.
