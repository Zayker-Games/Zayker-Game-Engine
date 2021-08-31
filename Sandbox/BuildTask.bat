rem Copy to debug folder
robocopy Engine Build\Debug\netcoreapp3.1\Engine /E /xf *.cs
rem Copy to publish folder
robocopy Engine Build\Publish\Engine /E /xf *.cs

@exit 0