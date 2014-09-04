(*
 * Copyright (C) 2012 - 2014 Takahisa Watanabe <linerlock@outlook.com> All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 *)
module Test.Parser

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec () =
    [<TestMethod>] member test.
     ``Parseq.Parser.Return(value).Run(stream).Case(success, failure) returns success(value)`` () =
        let stream = new CharStream(new System.IO.StringReader("foobar")) in
        let value = 42 in
        let failure = Func<string, int>(fun _ -> -1) in
        let success = Func<int, int>(fun x -> x) in
        Parseq.Parser.Return(value).Run(stream).Case(failure, success)
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Parser.Fail(errorMessage).Run(stream).Case(success, failure) returns failure(errorMessage)`` () =
        let stream = new CharStream(new System.IO.StringReader("foobar")) in
        let errorMessage = "error message" in
        let failure = Func<string, string>(fun x -> x) in
        let success = Func<int, string>(fun _ -> "don't care") in
        Parseq.Parser.Fail(errorMessage).Run(stream).Case(failure, success)
            |> should equal errorMessage

    [<TestMethod>] member test.
     ``Parseq.Parser.FailFast(errorMessage).Run(stream).Case(success, failure) returns failure(errorMessage)`` () =
        let stream = new CharStream(new System.IO.StringReader("foobar")) in
        let errorMessage = "error message" in
        let failure = Func<string, string>(fun x -> x) in
        let success = Func<int, string>(fun _ -> "don't care") in
        Parseq.Parser.FailFast(errorMessage).Run(stream).Case(failure, success)
            |> should equal errorMessage

    [<TestMethod>] member test.
     ``Throw FailFastException when you run the Parseq.Parser.FailFast parser from the other than Parseq.Parser.Run method`` () =
        let stream = new CharStream(new System.IO.StringReader("foobar")) in
        let errorMessage = "error message" in
        (fun () -> Parseq.Parser.FailFast(errorMessage).Invoke(stream) |> ignore)
            |> should throw typeof<Parseq.Parser.FailFastException<char>>