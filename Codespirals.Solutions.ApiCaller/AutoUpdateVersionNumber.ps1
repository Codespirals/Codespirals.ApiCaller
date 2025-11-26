#Add the following line to your post build events in your csproj file
#PowerShell -ExecutionPolicy Bypass -NoProfile -File AutoUpdateVersionNumber.ps1

$file = Get-ChildItem -Path .\ -Filter *.csproj
$path = $file.Name

$xml = [xml](Get-Content $path)

#The selection of the -First PropertyGroup was necessary here because certain settings can add additional property groups with the same name - however Version should always be in the first
$currentVersion = [version]($xml.Project.PropertyGroup | Select-Object -First 1).Version

$build = [int]$currentVersion.Build
$year = Get-Date -Format "yy"
$dayOfYear = (Get-Date).DayOfYear
$yearAndDay = "{0:d2}{1:d3}" -f [int]$year, [int]$dayOfYear
($xml.Project.PropertyGroup | Select-Object -First 1).Version = "{0}.{1}.{2}.{3}" -f $currentVersion.Major, $currentVersion.Minor, [int]($build + 1), $yearAndDay

$xml.Save($path)