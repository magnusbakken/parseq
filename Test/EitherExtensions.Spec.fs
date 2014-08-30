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
module Test.EitherExtensions

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest
open NHamcrest
open NHamcrest.Core

[<TestClass>]
type Spec () =
    [<TestMethod>] member test.
     ``Parseq.Either.Left(exception).Throws throws exception`` () =
        let ``exception`` = new Exception() in
        (fun () -> Either.Left(``exception``).Throw() |> ignore)
            |> should throw typeof<Exception>

    [<TestMethod>] member test.
     ``Parseq.Either.Right(value).Throws returns value`` () =
        let value = 42 in
        Either.Right(value).Throw()
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Either.Left(value).GetL returns value`` () =
        let value = 42 in
        Either.Left(value).GetL()
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Either.Left(value).GetR throws InvalidOperationException`` () =
        let value = 42 in
        (fun () -> Either.Left(value).GetR() |> ignore)
            |> should throw typeof<InvalidOperationException>

    [<TestMethod>] member test.
     ``Parseq.Either.Right(value).GetR returns value`` () =
        let value = 42 in
        Either.Right(value).GetR()
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Either.Right(value).GetL throws InvalidOperationException`` () =
        let value = 42 in
        (fun () -> Either.Right(value).GetL() |> ignore)
            |> should throw typeof<InvalidOperationException>

    [<TestMethod>] member test.
     ``Parseq.Either.Right(value).Select(selector) returns Either.Right(selector(value))`` () =
        let value = 42 in
        Either.Right<int, int>(value).Select(fun x -> x * 2)
            |> Either.GetR
            |> should equal (value * 2)

    [<TestMethod>] member test.
     ``Parseq.Either.Left(value).Select(selector) returns Either.Left(value)`` () =
        let value = 42 in
        Either.Left(value).Select(fun x -> x * 2)
            |> Either.GetL
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Either.Right(value).SelectMany(selector) returns selector(value)`` () =
        let value = 42 in
        Either.Right<int, int>(value).SelectMany(fun x -> Either.Right(x * 2))
            |> Either.GetR
            |> should equal (value * 2)

    [<TestMethod>] member test.
     ``Parseq.Either.Left(value).SelectMany(selector) returns Either.Left(value)`` () =
        let value = 42 in
        Either.Left(value).Select(fun x -> Either.Right(x * 2))
            |> Either.GetL
            |> should equal value

    [<TestMethod>] member test.
     ``Query-Expression Test`` () =
        Either.Right(2).SelectMany(fun x ->
            Either.Right(3).Select(fun y ->
                x * y
            )
        )
        |> Either.GetR
        |> should equal (2 * 3)
