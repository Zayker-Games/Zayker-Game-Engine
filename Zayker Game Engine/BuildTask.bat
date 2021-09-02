ECHO ON

echo Starting Build Task

cd ..\

rem delete old folders
del /f /q /s "Build"

rem Copy to debug folder
robocopy "Zayker Game Engine\Core" "Build\Debug\netcoreapp3.1\Engine\Core" /E /IS /it
robocopy "Zayker Game Engine\Modules" "Build\Debug\netcoreapp3.1\Engine\Modules" /E /IS /it
rem Copy to publish folder
robocopy "Zayker Game Engine\Core" "Build\Publish\Engine\Core" /E /IS /it
robocopy "Zayker Game Engine\Modules" "Build\Publish\Engine\Modules" /E /IS /it

@exit 0