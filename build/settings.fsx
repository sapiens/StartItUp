#r "tools/FAKE/tools/FakeLib.dll"
open Fake

let projName="StartItUp"
let projDir= "..\src" @@ projName
let testDir="..\src" @@ "Tests\Tests.csproj"

let additionalPack=[]
//let additionalPack=[|".SqlServer";".Sqlite"|]





