# CSSThemeHelper
A tool to help with the Metronic / CSS theming of ASP.NET Zero.

### About

CSSThemeHelper serves as an alternative to the Metronic tool provided by ASP.NET Zero on their private GitHub repo. See [https://github.com/aspnetzero/metronic](https://github.com/aspnetzero/metronic) 

These two tools do very similar things. Both generate new colors based on a single primary color and the Saturation and Lightness information (HSL color model) from the default theme colors.

This tool could still use improvement:
* The CSS files that I reference don't seem to contain every color that is defined in ASP.NET Zero (the number of colors between ASP.NET Zero's Metronic tool and my tool don't match)
* This tool DOES NOT change the images assocaiated with the Tenant's login screen or any other images associated with the theme.

### How it works

* The tool uses Regex to add all of the colors from each of the files defined in the `DefaultThemeFiles` array into a HashSet. I used HashSet to remove duplicates. 
* The tool uses Regex to find all of the colors from each of the files defined in the `AlternativeThemeFiles` and _removes_ them from the HashSet. 
* Now the HashSet of colors contains _ONLY_ the colors that are _NOT_ a part of a standard / common theme between your chosen themes. Theoretically, the only colors in this set are the accent colors from the Purple, Blue, Pink, and Green (etc) themes.
* The tool goes through each color in the HashSet, creates a CSS variable, and replaces the color in each CSS file with the variable reference. This is designed to make using the "Custom CSS" function of ASP.NET Zero easier. 
* The `metronic-customize.css` file contains the `:root` block that defines the colors for the rest of the CSS files. 
* Running the application will produce a `tenant.css` file at your specified location. This is the file that should be customized and distributed to Tenants for use in the "Custom CSS" function of ASP.NET Zero.
    * By default, the `tenant.css` file contains the information for your (the Host's) branding.
        * If you ran `UpdateThemeFiles()` with `relativeColors = true`, then customizing the colors for a Tenant is as easy as setting the `--branding-color-hue` variable to the Hue (0-360) of the Tenant's branding.
        * If you ran `UpdateThemeFiles()` with `relativeColors = false`, then you'll need to individually customize each HEX color.

### Getting Started

1. Clone this repo
2. Open the solution
   * You'll notice that there is a compile error in Program.cs. You'll fix this below. 
3. Create a file called `FilePaths.cs` in the root of the project (same directory as `Program.cs`).
4. Add and modify the following code
   * Notice the `**original**` markings. *Since this tool writes to your project directory, I chose to create a back up of the original CSS files.* The `**original**` folder is also where I told the tool to find the Default theme's files. 
```csharp
namespace CSSThemeHelper
{
    public static class FilePaths
    {
        // A file that should be distributed to tenants. 
        // Tenants should upload this file using the "Custom CSS" function of ASP.NET Zero after modifying the "--branding-color-hue" variable 
        public static readonly string DemoTenantFile = @"C:\Users\yourName\Desktop\tenant.css";

        // Default theme CSS files
        public static readonly string[] DefaultThemeFiles = new string[]
        {
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\**original**\metronic-customize.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\**original**\metronic-customize-angular.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\**original**\primeng.datatable.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\**original**\primeng.datatable-rtl.css",
            @"C:\path\to\your\project\angular\src\assets\metronic\assets\demo\default\base\**original**\style.bundle.css",
            @"C:\path\to\your\project\angular\src\assets\metronic\assets\demo\default\base\**original**\style.bundle.rtl.css"
        };

        // An alternative theme for comparison
        public static readonly string[] AlternativeThemeFiles = new string[]
        {
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\pink\metronic-customize.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\pink\metronic-customize-angular.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\pink\primeng.datatable.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\pink\primeng.datatable-rtl.css",
            @"C:\path\to\your\project\angular\src\assets\metronic\assets\demo\pink\base\style.bundle.css",
            @"C:\path\to\your\project\angular\src\assets\metronic\assets\demo\pink\base\style.bundle.rtl.css"
        };

        public static readonly string[] OutputThemeFiles = new string[]
        {
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\metronic-customize.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\metronic-customize-angular.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\primeng.datatable.css",
            @"C:\path\to\your\project\angular\src\assets\common\styles\themes\default\primeng.datatable-rtl.css",
            @"C:\path\to\your\project\angular\src\assets\metronic\assets\demo\default\base\style.bundle.css",
            @"C:\path\to\your\project\angular\src\assets\metronic\assets\demo\default\base\style.bundle.rtl.css"
        };
    }
}
```
5. Open `Program.cs` and modify the `PrimaryColor` and `DefaultPrimaryColor` to suit your needs.
   * `PrimaryColor` should reflect your primary branding color
   * `DefaultPrimaryColor` should reflect the primary color from the theme you defined in `DefaultThemeFiles`. See [https://github.com/aspnetzero/metronic/blob/master/MetronicThemeChanger/MetronicThemeChanger/Themes.cs](https://github.com/aspnetzero/metronic/blob/master/MetronicThemeChanger/MetronicThemeChanger/Themes.cs) for a list of the primary colors for the default themes.
6. Debug the application using Visual Studio and your files will be generated and output to the locations you defined. 