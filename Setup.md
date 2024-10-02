# Instructions for installation (ill redo this)

## Prerequisites

Before you start Make sure you have Visual Studio installed with the following components.

### Step 1: Install Visual Studio

Download and install [Visual Studio](https://visualstudio.microsoft.com/downloads/) if you have not already installed it.

### Step 2: Select workload

Once Visual Studio is installed, open the Visual Studio installer, then under the **Workload** tab, select:

- **ASP.NET and web development**
- **.NET Desktop Development**

### Step 3: Install each component

Switch to tab **Individual components** and make sure you have:

- **.NET 7.0 Runtime**

Choose above and install!

## Run the project

Once you have installed all the necessary components:

1. Double-click or run the `build.bat` file.

This will create a project for you. And you're all set!

## More Info
### Launch in Https (if https causes issues then create a issue)
- First go to the built location
- Then go to FortBackend/Resources/Config.json 
- Change HTTPS from false to true
- After, go to [FortBackend/resources/certificates](https://github.com/zinx28/FortBackend/tree/main/FortBackend/Resources/Certificates)
- Remove .temp from the file then replace it with your .pfx (If you have cert and the key just look up how to make it a pfx)