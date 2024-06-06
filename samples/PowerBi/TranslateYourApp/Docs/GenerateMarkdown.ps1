# pandoc -s --extract-media ./images/TranslationsBuilder "ReadMe.docx" -t gfm -o ./../ReadMe.md

# (Get-Content -path ./../ReadMe.md -Raw) -replace './images/TranslationsBuilder','./Docs/images/TranslationsBuilder' | Set-Content -Path ./../ReadMe.md

# pandoc -s --extract-media ./images/BuildingMultiLanguageReportsInPowerBI "Building Multi-language Reports in Power BI.docx" -t gfm -o "Building Multi-language Reports in Power BI.md"


# pandoc -s --extract-media ./images/ObtainingKeyForAzureTranslatorService "Obtaining a Key for the Azure Translator Service.docx" -t gfm -o "Obtaining a Key for the Azure Translator Service.md"


# pandoc -s --extract-media ./images/HandsOnLabExercises "Hands-On Lab Exercises.docx" -t gfm -o "Hands-On Lab Exercises.md"

pandoc -s --extract-media ./images/UserGuide "User Guide.docx" -t gfm -o "User Guide.md"

# pandoc -s --extract-media ./images/DeveloperGuide "Developer Guide.docx" -t gfm -o "Developer Guide.md"
