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
module Test.OptionExtensions

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec () =

    [<TestMethod>] member test.
     ``Parseq.Option.Some(value).Get() returns value`` () =
        let value = 42 in
        Parseq.Option.Some(value).Get()
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Option.None.Get() throws InvalidOperationException`` () =
        (fun () -> Parseq.Option.None().Get() |> ignore)
            |> should throw typeof<InvalidOperationException>

    [<TestMethod>] member test.
     ``Parseq.Option.Some(value).Select(selector).Get() returns selector(value)`` () =
        let value = 42 in
        Parseq.Option.Some(value).Select(fun x -> x * 2)
            |> Parseq.Option.Get
            |> should equal (value * 2)

    [<TestMethod>] member test.
     ``Parseq.Option.None().Select(selector).Get() throws InvalidOperationException`` () =
        (fun () -> Parseq.Option.None().Select(fun x -> x * 2).Get() |> ignore)
            |> should throw typeof<InvalidOperationException>

    [<TestMethod>] member test.
     ``Parseq.Option.Some(value).SelectMany(selector).Get() returns selector(value).Get()`` () =
        let value = 42 in
        Parseq.Option.Some(value).SelectMany(fun x -> Parseq.Option.Some(x * 2))
            |> Parseq.Option.Get
            |> should equal (value * 2)

    [<TestMethod>] member test.
     ``Parseq.Option.None().SelectMany(selector).Get throws InvalidOperationException`` () =
        (fun () -> Parseq.Option.None().SelectMany(fun x -> Parseq.Option.Some(x * 2)).Get() |> ignore)
            |> should throw typeof<InvalidOperationException>

    [<TestMethod>] member test.
      ``Query-Expression Test`` () =
        Parseq.Option.Some(2).SelectMany(fun x ->
            Parseq.Option.Some(3).Select(fun y ->
               x * y))
        |> Parseq.Option.Get
        |> should equal (2 * 3)