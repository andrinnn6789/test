net stop "IAG.InstallClient.HostWindows.exe" > InstallClientUpdate.log

del /F /S /Q %1
xcopy "%~dp0*.*" "%1\*.*" /Y

if NOT [%2]==[] (
	xcopy "%~dp0Settings\*.*" "%2\*.*" /Y
)

net start "IAG.InstallClient.HostWindows.exe" > InstallClientUpdate.log

rd /S /Q "%~dp0" >> InstallClientUpdate.log