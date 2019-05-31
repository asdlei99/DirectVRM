echo off
echo ------------------------------------------------------------
echo DirectVRM の NuGet パッケージ (.nupkg) 作成バッチ
echo 事前に x64/Release  のビルドを完了しておくこと。
echo また、NuGetパッケージの属性（バージョンなど）が変わったら、
echo DirectVRM/DirectVRM.nuspec を修正すること。
echo ------------------------------------------------------------

nuget pack ..\DirectVRM\DirectVRM.csproj -IncludeReferencedProjects -properties Configuration=Release;Platform=x64 -OutputDirectory nuget_packages
pause
