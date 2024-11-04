open Expecto
open MediaManagerTests.Models.Common.PathTests
[<EntryPoint>]
let main argv =
    runTestsWithCLIArgs [] argv pathTests
    // runTestsWithCLIArgs [] argv (
    //     testList "All tests" [
    //         pathTests
    //     ]
    // )
