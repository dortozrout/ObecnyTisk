@echo off
rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe -nologo -r:System.Net.Http.dll *.cs .\Form\*.cs
E:\Roslyn-4.1.0\csc.exe -nologo -out:Labels_202409.exe *.cs .\Form\*.cs
pause
