$DocsDirectory = Convert-Path $PSScriptRoot
$GithubDirectory = Join-Path $PSScriptRoot ../.github

'README', 'USAGE', 'CONTRIBUTING' | ForEach-Object {
    $source = "${DocsDirectory}/${_}.adoc"
    $docbook = "${DocsDirectory}/${_}.xml"
    $target = "${GithubDirectory}/${_}.md"
    & asciidoctor.bat -b docbook $source -o $docbook
    & pandoc.exe -f docbook -t markdown_strict $docbook -o $target
    Remove-Item $docbook
}