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
module Test.ReplyExtensions

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec () =
    [<TestMethod>] member test.
     ``Parseq.Reply.Success(restStream, value).Case(faliure, success) returns success(restStream, value)`` () =
        let restStream = new Parseq.CharStream(new System.IO.StringReader("don't care")) in
        let value = 42 in
        let failure = Func<IStream<char>, string, (IStream<char> * int)>(fun restStream _ -> Tuple.Create(restStream, -1)) in
        let success = Func<IStream<char>, int, (IStream<char> * int)>(fun restStream value -> Tuple.Create(restStream, value)) in
        Parseq.Reply.Success(restStream, value).Case(failure, success)
            |> fun (restStream', value') ->
                restStream'
                    |> should equal restStream;
                value'
                    |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Reply.Failure(restStream, errorMessage).Case(faliure, success) returns failure(restStream, errorMessage)`` () =
        let restStream = new Parseq.CharStream(new System.IO.StringReader("don't care")) in
        let errorMessage = "error message" in
        let failure = Func<IStream<char>, string, (IStream<char> * string)>(fun restStream errorMessage -> Tuple.Create(restStream, errorMessage)) in
        let success = Func<IStream<char>, int, (IStream<char> * string)>(fun restStream value -> Tuple.Create(restStream, "don't care")) in
        Parseq.Reply.Failure(restStream, errorMessage).Case(failure, success)
            |> fun (restStream', errorMessage') ->
                restStream'
                    |> should equal restStream;
                errorMessage'
                    |> should equal errorMessage
