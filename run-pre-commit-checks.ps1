param(
    [switch]$Reset
)

$modulesIsln = "Modules/Intent.Modules.Importers.isln"
$testsIsln = "Tests/Intent.Modules.Importers.Tests.isln"

if ($Reset) {
    ./PipelineScripts/run-pre-commit-checks.ps1 -ModulesIsln $modulesIsln -TestsIsln $testsIsln -Reset
    exit 0
}

./PipelineScripts/run-pre-commit-checks.ps1 -ModulesIsln $modulesIsln -TestsIsln $testsIsln
exit 0
