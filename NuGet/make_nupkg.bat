echo off
echo ------------------------------------------------------------
echo DirectVRM �� NuGet �p�b�P�[�W (.nupkg) �쐬�o�b�`
echo ���O�� x64/Release  �̃r���h���������Ă������ƁB
echo �܂��ANuGet�p�b�P�[�W�̑����i�o�[�W�����Ȃǁj���ς������A
echo DirectVRM/DirectVRM.nuspec ���C�����邱�ƁB
echo ------------------------------------------------------------

nuget pack ..\DirectVRM\DirectVRM.csproj -IncludeReferencedProjects -properties Configuration=Release;Platform=x64 -OutputDirectory nuget_packages
pause
