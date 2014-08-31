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
module Test.Delayed

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec() =
    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(value).Value returns value`` () =
        let value = 42 in
        Parseq.Delayed.Create(value).Value
            |> should equal value

    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(valueFactory).Value returns valueFactory()`` () =
        let valueFactory = Func<int>(fun () -> 42) in
        Parseq.Delayed.Create<int>(valueFactory).Value
            |> should equal (valueFactory.Invoke())

    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(value).HasValue always returns True`` () =
        let value = 42 in
        Parseq.Delayed.Create(value).HasValue
            |> should be True

    [<TestMethod>] member test.
     ``Parseq.Delayed.Create(valueFactory).HasValue returns whether the value has been created`` () =
        let valueFactory = Func<int>(fun () -> 42) in
        let delayed = Parseq.Delayed.Create<int>(valueFactory) in
        delayed.HasValue |> should be False;
        delayed.Value |> ignore;
        delayed.HasValue |> should be True

    [<TestMethod>] member test.
     ``If the instance of Parseq.Delayed<T> interface treated as IOption<T>, the one's HasValue method always return true`` () =
        let valueFactory = Func<int>(fun () -> 42) in
        (Parseq.Delayed.Create<int>(valueFactory) :> IOption<int>).HasValue
            |> should be True
