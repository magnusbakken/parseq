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
module Test.DelayedExtensions

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec() =
    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(value).Select(selector).Value returns selector(value)`` () =
        let value = 42 in
        Parseq.Delayed.Create(value).Select(fun x -> x * 2).Value
            |> should equal (value * 2)

    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(valueFactory).Select(selector).Value returns selector(valueFactory())`` () =
        let valueFactory = Func<int>(fun () -> 42) in
        Parseq.Delayed.Create<int>(valueFactory).Select(fun x -> x * 2).Value
            |> should equal (valueFactory.Invoke() * 2)

    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(value).SelectMany(selector) returns selector(value)`` () =
        let value = 42 in
        Parseq.Delayed.Create(value).SelectMany(fun x -> 
            Parseq.Delayed.Create<int>(fun () -> x * 2)).Value
            |> should equal (value * 2)

    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(valueFactory).SelectMany(selector) returns selector(valueFactory())`` () =
        let valueFactory = Func<int>(fun () -> 42) in
        Parseq.Delayed.Create<int>(valueFactory).SelectMany(fun x ->
            Parseq.Delayed.Create<int>(fun () -> x * 2)).Value
            |> should equal (valueFactory.Invoke() * 2)

    [<TestMethod>] member test.
     ``Query-Expression Test`` () =
        Parseq.Delayed.Create<int>(fun () -> 2).SelectMany(fun x ->
            Parseq.Delayed.Create<int>(3).SelectMany(fun y ->
                Parseq.Delayed.Create<int>(fun () -> 5).Select(fun z ->
                    x * y * z))).Value
            |> should equal (2 * 3 * 5)
