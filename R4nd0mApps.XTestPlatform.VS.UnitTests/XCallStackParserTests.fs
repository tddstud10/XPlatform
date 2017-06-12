module R4nd0mApps.XTestPlatform.Implementation.VS.XCallStackParserTests

open global.Xunit
open R4nd0mApps.XTestPlatform.Api
open FsUnit.Xunit

let ``Test Data - Call stack parser tests`` : obj array seq = 
    [| (null, null)
       ("", null)
       ("at XNS.XC.XM()", [| XUnparsedFrame "at XNS.XC.XM()" |])
       ("at NS.C.M() in d:\\s\\p\\f.cs:line 15", [| XParsedFrame("NS.C.M()", @"d:\s\p\f.cs", 15) |])
       ("    at NS.C.M(T p) in d:\\s1\\p2\\f3.cs:line 1000\n    Anything that cannot be parsed as stackframe\r\n    at YNS.YC.YM(YT yp, XT xp) in d:\\s5\\p6\\f7.cpp:line 5000", 
        [| XParsedFrame("NS.C.M(T p)", @"d:\s1\p2\f3.cs", 1000)
           XUnparsedFrame "Anything that cannot be parsed as stackframe"
           XParsedFrame("YNS.YC.YM(YT yp, XT xp)", @"d:\s5\p6\f7.cpp", 5000) |])
       
       (// Mono stack frames
        "at A.B.C.D[a] (a a, a.a`1[T] m) [0x00041] in <566549b8b0d60cb9a7450383b8496556>:0", 
        [| XUnparsedFrame "at A.B.C.D[a] (a a, a.a`1[T] m) [0x00041] in <566549b8b0d60cb9a7450383b8496556>:0" |])
       
       ("    at R.X.I..Call stack parser tests[a] (s cs, a p) [0x0006f] in /a/b/c/d.fs:30", 
        [| XParsedFrame("R.X.I..Call stack parser tests[a] (s cs, a p)", "/a/b/c/d.fs", 30) |])
       ("at R.X.I.XX.Call stack parser tests[a] (a b, a b) [0x0006f] in /a/b/c/d.fs:30\n    at (wrapper managed-to-native) S.R.M:I (a,object,object[],e&)", 
        [| XParsedFrame("R.X.I.XX.Call stack parser tests[a] (a b, a b)", "/a/b/c/d.fs", 30)
           XUnparsedFrame "at (wrapper managed-to-native) S.R.M:I (a,object,object[],e&)" |]) |]
    |> Seq.map (fun (a, b) -> 
           [| box a
              box b |])

[<Theory>]
[<MemberData("Test Data - Call stack parser tests")>]
let ``Call stack parser tests`` (callStack, parsedCallStack) = 
    callStack
    |> XCallStackParser.parse
    |> should equal parsedCallStack
