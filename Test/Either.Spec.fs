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
module Test.Either

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest
open NHamcrest
open NHamcrest.Core

let success = CustomMatcher<obj>("success", fun _ -> true)
let failure = CustomMatcher<obj>("failure", fun _ -> false)


[<TestClass>]
type Spec () =
    [<TestMethod>] member test.
     ``Parseq.Either.Left(value).Case(l, r) returns l(value)`` () =
        let value = 42 in
        Parseq.Either.Left<int, int>(value).Case(
            System.Func<int, int>(fun x -> x * 2),
            System.Func<int, int>(fun x -> x / 2))
            |> should equal (value * 2)


    [<TestMethod>] member test.
     ``Parseq.Either.Right(value).Case(l, r) returns r(value)`` () =
        let value = 42 in
        Parseq.Either.Right<int, int>(value).Case(
            System.Func<int, int>(fun x -> x * 2),
            System.Func<int, int>(fun x -> x / 2))
            |> should equal (value / 2)


        