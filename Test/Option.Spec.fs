﻿(*
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
module Test.Option

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec () =
    
    [<TestMethod>] member test.
     ``Parseq.Option.Some(value).HasValue must be True``() =
        let ``don't care`` = 42 in
        Option.Some(``don't care``).HasValue |> should be True

    [<TestMethod>] member test.
     ``Parseq.Option.Some(value).Value returns value``() =
        let value = 42 in
        Option.Some(value).Value |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Option.None().HasValue must be False``() =
        Option.None().HasValue |> should be False

    [<TestMethod>] member test.
     ``Parseq.Option.None().Value throws InvalidOperationException``() =
        (fun () -> Option.None().Value |> ignore)
            |> should throw typeof<InvalidOperationException>
