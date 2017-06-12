module R4nd0mApps.XTestPlatform.CecilUtils.NavigationDataProviderTests

open global.Xunit
open FsUnit.Xunit
open R4nd0mApps.XTestPlatform.CecilUtils
open System.IO

let provider = typeof<NavigationDataProvider> |> Path.getAssemblyPath |> NavigationDataProvider 

[<InlineData("", "EmptyMethod_OneLine", "NavigationTestData.cs", 10)>]
[<InlineData("", "EmptyMethod_TwoLines", "NavigationTestData.cs", 13)>]
[<InlineData("", "EmptyMethod_ThreeLines", "NavigationTestData.cs", 17)>]
[<InlineData("", "EmptyMethod_LotsOfLines", "NavigationTestData.cs", 21)>]
[<InlineData("", "SimpleMethod_Void_NoArgs", "NavigationTestData.cs", 27)>]
[<InlineData("", "SimpleMethod_Void_OneArg", "NavigationTestData.cs", 33)>]
[<InlineData("", "SimpleMethod_Void_TwoArgs", "NavigationTestData.cs", 39)>]
[<InlineData("", "SimpleMethod_ReturnsInt_NoArgs", "NavigationTestData.cs", 45)>]
[<InlineData("", "SimpleMethod_ReturnsString_OneArg", "NavigationTestData.cs", 51)>]
[<InlineData("", "GenericMethod_ReturnsString_OneArg", "NavigationTestData.cs", 56)>]
[<InlineData("", "AsyncMethod_Void", "NavigationTestData.cs", 61)>]
[<InlineData("", "AsyncMethod_Task", "NavigationTestData.cs", 68)>]
[<InlineData("", "AsyncMethod_ReturnsInt", "NavigationTestData.cs", 75)>]
[<InlineData("/NestedClass", "SimpleMethod_Void_NoArgs", "NavigationTestData.cs", 84)>]
[<InlineData("/ParameterizedFixture", "SimpleMethod_ReturnsString_OneArg", "NavigationTestData.cs", 102)>]
[<InlineData("/GenericFixture`2", "Matches", "NavigationTestData.cs", 117)>]
[<InlineData("/GenericFixture`2/DoublyNested", "WriteBoth", "NavigationTestData.cs", 133)>]
[<InlineData("/GenericFixture`2/DoublyNested`1", "WriteAllThree", "NavigationTestData.cs", 152)>]
[<InlineData("/DerivedClass", "EmptyMethod_ThreeLines", "NavigationTestData.cs", 161)>]
[<InlineData("+NestedClass", "SimpleMethod_Void_NoArgs", "NavigationTestData.cs", 84)>]
[<InlineData("+GenericFixture`2+DoublyNested", "WriteBoth", "NavigationTestData.cs", 133)>]
[<InlineData("+GenericFixture`2+DoublyNested`1", "WriteAllThree", "NavigationTestData.cs", 152)>]
[<InlineData("/DerivedFromExternalAbstractClass", "EmptyMethod_ThreeLines", "NavigationTestData2.cs", 6)>]
[<InlineData("/DerivedFromExternalConcreteClass", "EmptyMethod_ThreeLines", "NavigationTestData2.cs", 13)>]
[<Theory>]
let ``Verify NavigationData Within Assembly``(suffix, methodName, fileName, lineNumber) =
    let className = "R4nd0mApps.XTestPlatform.CecilUtils.Tests.NavigationTestData" + suffix
    let nd = provider.GetNavigationData(className, methodName)
    nd.FilePath |> Path.GetFileName |> should equal fileName
    nd.LineNumber |> should equal lineNumber
