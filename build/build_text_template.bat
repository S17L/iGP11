set ProgramFiles=%1
set ProgramFiles=%ProgramFiles:"=%
set ProjectDir=%2
set ProjectDir=%ProjectDir:"=%
set FileName=%3
set FileName=%FileName:"=%
"%ProgramFiles%\Common Files\Microsoft Shared\TextTemplating\12.0\TextTransform.exe" "%ProjectDir%%FileName%.tt" -out "%ProjectDir%%FileName%.cpp"