# Business Platform Solution Template Framework for Power BI

This repository contains the resources and tools required for contributing to the Business Platform Solution Template (BPST) Framework. The BPST Framework is the driving mechanism behind Power BI solution templates.

If you have any questions about building your own solution template, you can find ways to contact us at the link above. There is also a direct Contact Us link available at the bottom right area of any solution template.

## Preparing your Development Environment

Visual Studio 2017 is the required IDE. If you elect to use a different IDE most build and debug features will be unavailable.

While installing Visual Studio 2017, the following features are required:

1. Windows
    * Universal Windows Platform Development
    * .NET desktop development
    * Desktop development with C++
2. Web & Cloud
    * ASP.NET and web development
    * Azure development
    * Node.js development
    * Data storage and processing
3. Other Toolsets
    * .NET Core 1.0 cross-platform development
    * Gulp
4. GitHub Extension for Visual Studio (found under Individual components > Uncategorized)

While Visual Studio 2017 installs, Fork the BPST Framework repository. The simplest way to Fork is to use the Fork button at the top right of this page:

https://github.com/Microsoft/BusinessPlatformApps

Keep in mind that the dev branch acts as the master branch of this repository. If you are unfamiliar with the GitHub branching system, simply make all changes to your forked copy of the dev branch.

## Node Installation

1. Install [NodeJS >= 6.X](https://nodejs.org/en/)
    * Confirm a successful installation by running `node -v` in a command prompt
    * Also confirm npm availability by running `npm -v`
2. Install [Git](https://git-scm.com/downloads)
    * Confirm a successful installation by running `git --version` in a command prompt
3. In a fresh command prompt, install gulp by running `npm install gulp -g`
4. Install typings by running `npm install typings -g`
5. In a fresh command prompt, navigate to the directory where the repository cloned
6. Navigate down to Source/Site/Microsoft.Deployment.Site.Web
7. Run `npm install`

## .NET Core Installation

The solution requires .NET Core 1.0.4, for which the installer is no longer easily available online

1. Download the latest version of [.NET Core SDK >= 2.X](https://www.microsoft.com/net/download/windows)
2. Download the [.NET Core 1.0.4 SDK Binaries](https://www.microsoft.com/net/download/dotnet-core/sdk-1.0.4)
3. Extract the downloaded .zip
4. From inside the .zip, copy shared\Microsoft.NETCore.App\\* to C:\Program Files\dotnet\Microsoft.NETCore.App\ (or wherever you installed the latest .NET Core)
5. From inside the .zip, copy sdk\\* to C:\Program Files\dotnet\sdk\ (or wherever you installed the latest .NET Core)
6. From inside the .zip, copy host\fxr\\* to C:\Program Files\dotnet\host\fxr\ (or wherever you installed the latest .NET Core)

## Debugging Locally

1. Open Visual Studio 2017
2. Open Team Explorer
3. Open the Connect Tab
4. Sign in to GitHub
5. Clone your Fork of the BPST Framework
6. Open the Home tab
7. Open the Microsoft.Deployment solution
8. Wait for the .NET Core packages to restore (this only needs to happen the first time you open the solution)
9. Build the solution
10. Debug Microsoft.Deployment.Site.Service
11. While it is still running, Debug Microsoft.Deployment.Site.Web
12. A local instance of the site will now be running (close the browser to stop debugging)

For subsequent debugging sessions, the following steps are recommended:

1. Clean the solution (this will run a gulp script to clean up the website files)
2. Rebuild the solution
3. Repeat steps 10-12 from above

## Contributing to BPST

All available solution templates can be found in the Apps folder. To access an app, the name is passed as a query parameter to the url of the website. For example, in the following url:

https://bpsolutiontemplates.com/?name=Microsoft-TwitterTemplate

The name of the template Microsoft-TwitterTemplate corresponds to the Microsoft-TwitterTemplate folder inside the Apps directory. If you are a Microsoft Partner, please add your Apps to the Partner directory instead of the Released directory.

### init.json

To function properly, all newly added Apps require an init.json file. The minimum requirement for the init.json is to have the following structure:

```json
{
    "Install": {
        "Pages": [
            {
                "displayname": "",
                "name": ""
            }
        ],
        "Actions": [
            {
                "displayname": "",
                "name": ""
            }
        ]
    }
}
```

#### Pages

Pages is a list of objects corresponding to the pages to be displayed for the solution template. These are intended to be navigated through in the order set here.

Pages are intended for providing information about the solution template and also for gathering and validating user credentials and customizations for use during deployment.

At minimum each page requires two properties, a displayname and a name.

A displayname is a string that appears in the tab list on the left of a solution template page to describe the intent of the currently open page.

A name is a string that corresponds to the file path of the .html file for your page. This path assumes you have a Web folder located inside your App directory and is relative to it (the Web folder should be at the same level as the init.json file).

You can also make use of the existing pages. Here is an example of the syntax:

$SiteCommon$/pages-gallery/sql.html

For advanced usage, explore the existing init.json files. The typical advanced use is to create a new property that will be directly passed in as a property of the TypeScript file for the page. Contact us if you would like to learn about different advanced uses.

#### Actions

Actions is a list of objects corresponding to the service actions to be executed by the following page:

$SiteCommon$/pages-gallery/progress.html

All Microsoft solution templates have a progress page at the end of the Pages list.

Similar to Pages, all actions require a displayname and a name property.

The displayname property is a string that corresponds to the text that gets displayed while the action is executing.

The name corresponds to the name of the .cs file located in the Actions directory containing the code to execute the action prefixed by Microsoft-. For example:

Microsoft-DeploySQLScripts

This will call the DeploySQLScripts action. See some of the existing actions to figure out how to structure your custom ones. Currently there is no direct support to have a custom prefix, but contact us if you would like one.

#### HTML, TypeScript, and Aurelia

Each .html file has a .ts file with the same name that it corresponds to. Each page extends ViewModelBase, which provides it with several important pieces of functionality. The most important are the following:

* onLoaded
* onNavigatingNext

The onLoaded function is called when the page opens. In particular, you will want to set this.isValidated = true at some point to allow the Next button for a page to become enabled and allow a user to progress.

The simplest way to call an action is to use the following syntax:

await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions');

If the action returns a serialized JSON response, you can set a variable equal to the ActionResponse:

let subscriptions: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions');

ActionResponse is a model that exists both for the TypeScript website and in the service. Responses typically appear in the Body.value property. For advanced usage, see some of the .ts files in the pages-gallery or contact us.

For data binding between .html and .ts, the Aurelia framework is used. For more information about how to use Aurelia, see their documentation here:

http://aurelia.io/

If calling an action fails, the page will automatically invalidate itself and display the error returned by the action. For customizing how this works, check out some of the actions and also check out the error-service.ts file. You can also contact us.

The onNavigatingNext function lets you execute code after the user clicks Next. The function should return true if all execution was successful. In particular, you can call any number of actions and if some of them fail, the user will be prevented from successfully navigating next and instead kept on the current page in an error state forcing them to validate their input.

### Checking in

Once your changes are ready, open a new pull request targeting the dev branch. Upon review, we will get your changes merged in and deployed to production during our next release cycle.
