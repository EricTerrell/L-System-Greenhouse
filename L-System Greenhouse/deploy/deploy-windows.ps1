Push-Location

c:
cd "\Users\erict\Documents\software development\L-System Greenhouse\L-System Greenhouse"

Remove-Item "C:\Users\erict\Documents\software development\L-System Greenhouse\L-System Greenhouse\bin\Release\net9.0\win-x64" -Recurse -Force

dotnet publish -c Release --os win --self-contained -f net9.0

Pop-Location