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

### Use the Github Actions to test your development

The [Test, Build and Deploy to GitHub Pages 😎](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/main.yml) workflow allows you, as its name suggest, to test, build and deploy the code on the targeted branch on Github Pages.
Because of the 2000 minutes of free runners availability [(learn more here)](https://docs.github.com/en/billing/managing-billing-for-github-actions/about-billing-for-github-actions), this workflow is not triggered automatically on pull requests or push.
You need to trigger it manually [here](https://github.com/Mythic-North-Games/FrostallSaga/actions/workflows/main.yml) and choose the branch you want to trigger on.
You must do it when your pull request has been approved and is ready to merge, to avoid introducing regressions that could have been detected by the tests or the builds.

> Triggering the workflow will, if the tests and the WebGL build succeed, deploy the built game to Github Pages. Use it to showcase the latest version of your game or to functionally test a specific branch.
