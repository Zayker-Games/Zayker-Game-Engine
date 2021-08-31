ECHO ON

cd ..\

rem Copy to debug folder
robocopy "Zayker Game Engine\Core" "Build\Debug\netcoreapp3.1\Engine\Core" /E
robocopy "Zayker Game Engine\Modules" "Build\Debug\netcoreapp3.1\Engine\Modules" /E
rem Copy to publish folder
robocopy "Zayker Game Engine\Core" "Build\Publish\Engine\Core" /E
robocopy "Zayker Game Engine\Modules" "Build\Publish\Engine\Modules" /E

@exit 0