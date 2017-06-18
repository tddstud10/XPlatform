module R4nd0mApps.XTestPlatform.Implementation.VS.XTestErrorInfoParser

open global.Xunit
open R4nd0mApps.XTestPlatform.Api
open FsUnit.Xunit

let ``Test Data - Call stack parser tests`` : obj array seq = 
    [| (null, [||])
       ("", [||])
       ("at XNS.XC.XM()", [| XErrorUnparsedFrame "at XNS.XC.XM()" |])
       ("at NS.C.M() in d:\\s\\p\\f.cs:line 15", [| XErrorParsedFrame("NS.C.M()", @"d:\s\p\f.cs", 15) |])
       ("    at NS.C.M(T p) in d:\\s1\\p2\\f3.cs:line 1000\n    Anything that cannot be parsed as stackframe\r\n    at YNS.YC.YM(YT yp, XT xp) in d:\\s5\\p6\\f7.cpp:line 5000", 
        [| XErrorParsedFrame("NS.C.M(T p)", @"d:\s1\p2\f3.cs", 1000)
           XErrorUnparsedFrame "Anything that cannot be parsed as stackframe"
           XErrorParsedFrame("YNS.YC.YM(YT yp, XT xp)", @"d:\s5\p6\f7.cpp", 5000) |])
       
       (// Mono stack frames
        "at A.B.C.D[a] (a a, a.a`1[T] m) [0x00041] in <566549b8b0d60cb9a7450383b8496556>:0", 
        [| XErrorUnparsedFrame "at A.B.C.D[a] (a a, a.a`1[T] m) [0x00041] in <566549b8b0d60cb9a7450383b8496556>:0" |])
       
       ("    at R.X.I..Call stack parser tests[a] (s cs, a p) [0x0006f] in /a/b/c/d.fs:30", 
        [| XErrorParsedFrame("R.X.I..Call stack parser tests[a] (s cs, a p)", "/a/b/c/d.fs", 30) |])
       ("at R.X.I.XX.Call stack parser tests[a] (a b, a b) [0x0006f] in /a/b/c/d.fs:30\n    at (wrapper managed-to-native) S.R.M:I (a,object,object[],e&)", 
        [| XErrorParsedFrame("R.X.I.XX.Call stack parser tests[a] (a b, a b)", "/a/b/c/d.fs", 30)
           XErrorUnparsedFrame "at (wrapper managed-to-native) S.R.M:I (a,object,object[],e&)" |]) |]
    |> Seq.map (fun (a, b) -> 
           [| box a
              box b |])

[<Theory>]
[<MemberData("Test Data - Call stack parser tests")>]
let ``Call stack parser tests`` (callStack, parsedCallStack) = 
    callStack
    |> XTestErrorInfoParser.parseStackTrace
    |> should equal (parsedCallStack |> XErrorStackTrace)

[<Theory>]
[<InlineData(null, "")>]
[<InlineData("", "")>]
[<InlineData("anything", "anything")>]
let ``Error message parser tests`` (msg, expected) = 
    msg
    |> XTestErrorInfoParser.parseMessage
    |> should equal (expected |> XErrorMessage)
