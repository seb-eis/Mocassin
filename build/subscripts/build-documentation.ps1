param (
    [Parameter(Mandatory=$true)]
    [ValidateNotNull()]
    [object]
    $Settings
)

try {
    Write-Host "Sourcing helper functions ..."
    . "./subscripts/helper-functions.ps1"
}
catch {
    throw "Failed to source helper functions."
}

if (-not (Test-Command pandoc)) {
    throw "Cannot build documentation. The 'pandoc' command is not defined."
}

$pathToKatex = $Settings.Source.KatexDirectory
$pathToGuidePagesSource = $Settings.Source.GuidePagesDirectory
$pathToGuidePagesDeploy = $Settings.Deploy.RootDirectory, $Settings.Deploy.RelativeGuidePagesPath -join "/"
$pathToStylesDeploy = $pathToGuidePagesDeploy, "styles" -join "/"
$pathToFiguresSource = $pathToGuidePagesSource, "figures/png" -join "/"
$pathToFiguresDeploy = $pathToGuidePagesDeploy, "figures/png" -join "/"
$pandocCss = $Settings.Source.StylesDirectory, $Settings.Deploy.PandocCss -join "/"
$deployCss = "styles/style.css"

New-Item $pathToGuidePagesDeploy -ItemType Directory
New-Item $pathToStylesDeploy -ItemType Directory

if (-not (Test-Katex $pathToKatex)) {
    throw "Katex is missing at $pathToKatex. Cannot build html documentation."
}

foreach ($mdFile in (Get-ChildItem $pathToGuidePagesSource\* -Include *.md)) {
    $outFile = $pathToGuidePagesDeploy + "/" + $mdFile.BaseName + ".html"
    pandoc -s --katex="$pathToKatex/" -f markdown -t html -o $outFile $mdFile.FullName --lua-filter=./subscripts/md-to-html-link.lua --css=$deployCss
}

Copy-Item -Recurse $pathToFiguresSource -Destination $pathToFiguresDeploy
Copy-Item -Recurse $pathToKatex -Destination $pathToGuidePagesDeploy
Copy-Item $pandocCss -Destination $pathToStylesDeploy/style.css
