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
module Test.Position
open System
open Parseq

open NUnit.Framework
open FsUnit

[<TestFixture>]
type Spec () =
    [<TestCase>] member test.
     ``Two instances of Position are equal if the value of Line and Column and Index properties are the same`` () =
        let instance0 = new Position(1, 1, 0) in
        let instance1 = new Position(1, 1, 0) in
        instance0.Line |> should equal instance1.Line;
        instance0.Column |> should equal instance1.Column;
        instance0.Index |> should equal instance1.Index;
        instance0.Equals(instance1)
            |> should be True

    [<TestCase>] member test.
     ``Throws an ArgumentOutOfRangeException if the constructor's argument index less than 0`` () =
        let ``don't care`` = 42 in
        (fun () -> new Position(``don't care``, ``don't care``, -1) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>

    [<TestCase>] member test.
     ``Throws an ArgumentOutOfRangeException if the constructor's argument line less than 1`` () =
        let ``don't care`` = 42 in
        (fun () -> new Position(0, ``don't care``, ``don't care``) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>

    [<TestCase>] member test.
     ``Throws an ArgumentOutOfRangeException if the constructor's argument column less than 1`` () =
        let ``don't care`` = 42 in
        (fun () -> new Position(``don't care``, 0, ``don't care``) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>