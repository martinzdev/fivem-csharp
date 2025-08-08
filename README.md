# Core for CitizenFX

This repository contains the **core**, a professional boilerplate for developing C# resources for FiveM using the CitizenFX platform. The goal of this project is to provide a solid, organized, and easily maintainable base to speed up the development of client and server scripts.

## Project Editing

To edit the project, open the `core.sln` solution in Visual Studio or your preferred editor. The structure is separated into three main projects:

- **core.Client**: Client-side scripts and logic.
- **core.Server**: Server-side scripts and logic.
- **core.Shared**: Code shared between client and server.

## Building

You can build the projects in two ways: using the `build.cmd` script or directly through your preferred IDE, such as Visual Studio or JetBrains Rider (IDEA).

### Option 1: Build via Script

1. Open the Command Prompt in the root of the repository.
2. Run the command:

   ```
   build.cmd
   ```

The script will automatically build the required projects and generate the files ready for use in FiveM.

**After building, a `bin` folder will be generated containing the compiled client and server `.dll` files.**

### Option 2: Build via IDE

You can also open the `core.sln` solution in any .NET-compatible IDE, such as Visual Studio or JetBrains Rider (IDEA), and build the projects directly there using the IDE's build commands.

**Similarly, when building via the IDE, a `bin` folder will be created with the compiled `.dll` files for both client and server.**

---

If you have any questions or need support, please refer to the official FiveM documentation or open an issue in this repository.
