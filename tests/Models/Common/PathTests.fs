module MediaManagerTests.Models.Common.PathTests

open Expecto
open MediaManager.Models.Common.Path

[<Tests>]
let pathTests =
    let validAbsolutePaths = [
        @"C:\Users\"
        @"D:\Documents\"
        @"f:\Documents\Folder.With.Dot\" ]
    let invalidAbsolutePaths = [
        null
        ""
        "  "
        @"Cx:\Users\"
        @"C:\Users\ \"
        @"D:\Documents"
        @"D:\Documents\\T\"
        @"D:\Docu*ents\"
        @".\Documents"
        @"f:\Documents\Folder.With.Dot\file.exe" ]
    let validRelativePaths = [
        @"\Users\"
        @"\Documents\"
        @"\Documents\Folder.With.Dot\" ]
    let invalidRelativePaths = [
        ""
        @".\Users\"
        ]
    testList "Path tests" [ 
        testTheory "createAbsolute with valid paths" validAbsolutePaths <| fun x ->
            let result = createAbsolute x
            Expect.isOk result $"Path: '{x}' should be valid"
        testTheory "createAbsolute with invalid paths" invalidAbsolutePaths <| fun x ->
            let result = createAbsolute x
            Expect.isError result $"Path: '{x}' should be invalid"
        testTheory "createAbsolute with valid relative paths" validRelativePaths <| fun x ->
            let result = createAbsolute x
            Expect.isError result $"Path: '{x}' should be invalid"
            
        testTheory "createRelative with valid paths" validRelativePaths <| fun x ->
            let result = createRelative x
            Expect.isOk result $"Path: '{x}' should be valid"
        testTheory "createRelative with invalid paths" invalidRelativePaths <| fun x ->
            let result = createAbsolute x
            Expect.isError result $"Path: '{x}' should be invalid"
        testTheory "createRelative with valid absolute paths" validAbsolutePaths <| fun x ->
            let result = createRelative x
            Expect.isError result $"Path: '{x}' should be invalid"
        
    ]