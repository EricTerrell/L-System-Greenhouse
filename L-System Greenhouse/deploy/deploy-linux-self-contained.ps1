Push-Location

c:
cd "\Users\erict\Documents\software development\L-System Greenhouse\L-System Greenhouse"

Remove-Item "C:\Users\erict\Documents\software development\L-System Greenhouse\L-System Greenhouse\bin\Release\net9.0\linux-x64" -Recurse -Force

dotnet publish -c Release --os linux --self-contained true -f net9.0

copy "Assets\app_icon.png" ".\bin\Release\net9.0\linux-x64\L-System Greenhouse.png"

Pop-Location