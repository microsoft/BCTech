# Translations Builder

Translations Builder is an external tool created for Power BI Desktop
specifically to assist report authors and dataset authors with tasks
associated with creating translations and building multi-language
reports. **As a user**, you can install Translations Builder and use it
together with Power BI Desktop to build and test datasets and reports
that support multiple languages. **As a developer**, you can clone the
GitHub repository with the Translations Builder source code and extend
this application to meet whatever translation and localization
requirements your organization faces.

## Translations Builder Console

The *Translations Builder Console* tool is a version of Translation Builder that does not require UI. A prerequisite for running the console application is that Power BI desktop is running.

To build the binaries for the tool, open the TranslationBuilderConsole solution in Visual Studio and build.

To understand how to use the tool, run *TranslationBuilderConsole.exe /?*:

``` cmd
C:\git\BCTech\samples\PowerBi\TranslateYourApp\TranslationBuilderConsole\bin\Debug\net6.0-windows>TranslationsBuilderConsole.exe /?
Connecting to Power BI Desktop...
16000
msmdsrv
Untitled - Power BI Desktop
Port: 61024
16000
msmdsrv
Untitled - Power BI Desktop
Port: 61024
Unsupported fileFormat.
Usage: TranslationBuilderConsole [OPTIONS]+

Options:
  -o, --operation=VALUE      operation(required) [export,import]
  -f, --fileFormat=VALUE     file format(required) [csv,resx,json]
  -h, --help                 show this message and exit
      --filePath=VALUE       file to export/import
      --filePrefix=VALUE     file prefix when exporting/importing
      --directory=VALUE      directory for exporting/importing
      --cultures=VALUE       cultures to export
      --defaultCulture=VALUE default culture when importing
      --pbixFile=VALUE       target PBIX file path
-------------------
Examples:
        --operation export --fileFormat resx --directory "<path>" --filePrefix "FinanceApp" --cultures "en-US,fr-FR,da-DK"
        --operation import --fileFormat resx --directory "<path>" --filePrefix "FinanceApp" --defaultCulture "en-US"
```



## Quick Start Guide
Here are the primary links for learning how to build multi-language reports for Power BI using Translations Builder.
 - [**Guidance Document**: *Building Multi-language Reports for Power BI*](Docs/Building%20Multi-language%20Reports%20in%20Power%20BI.md)
 - [**Hands-on Lab**: *Building Multi-language Reports for Power BI*](Labs/Hands-on%20Lab%20-%20Building%20Multi-language%20Reports%20for%20Power%20BI.md)
 - [**Multi-language Report Live Demo**](https://multilanguagereportdemo.azurewebsites.net)
 - [**Installation Guide**](Docs/Installation%20Guide.md)
 - [**User Guide**](Docs/User%20Guide.md)
 - [**Developer Guide**](Docs/Developer%20Guide.md)