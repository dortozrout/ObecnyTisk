@echo off
rem C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -nologo -r:System.Net.Http.dll  *.cs HtmlParser\*.cs Form\*.cs
set exeName=Labels_202501.exe
set currentDisc=%cd:~0,3%
IF EXIST "%currentDisc%Roslyn-4.1.0\csc.exe" (
    %currentDisc%Roslyn-4.1.0\csc.exe -nologo -r:System.Net.Http.dll *.cs .\Form\*.cs -out:%exeName%
) ELSE (
    echo Compiller not found!
)
pause
