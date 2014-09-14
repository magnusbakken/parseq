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
module Test.CharStream
open System
open System.IO
open System.Reflection
open Parseq

open NUnit.Framework
open FsUnit

type Spec () =
(* private field accessor *)
    let bindingFlags =
        BindingFlags.SetField |||
        BindingFlags.GetField |||
        BindingFlags.Static |||
        BindingFlags.Instance |||
        BindingFlags.NonPublic |||
        BindingFlags.Public

    let buffer (stream : CharStream) =
        typeof<CharStream>.GetField("buffer", bindingFlags).GetValue(stream) :?> IDisposable

    let (|Some|None|) (option : IOption<'a>) =
        if option.HasValue then 
            Some(option.Value)
        else
            None

    [<TestCase>] member test.
     ``CharStream.Current returns None if the internal buffer reaches EOF`` () =
        let baseReader = TextReader.Null in
        let charBuffer = new CharBuffer(baseReader) in
        let charStream = new CharStream(charBuffer) in
        match charStream.Current with
        | Some _ -> Assert.Fail()
        | None   -> Assert.Pass()

    [<TestCase>] member test.
     ``CharStream.Current returns Some(c) if the character to be read next from the internal buffer is c`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader) in
        let charStream = new CharStream(charBuffer) in
        let c = "foobar".[0] in
        match charStream.Current with
        | Some c' -> c' |> should equal c
        | None    -> Assert.Fail()
        
    [<TestCase>] member test.
     ``CharStream.Consume() returns same value even if it were called many times`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader) in
        let charStream = new CharStream(charBuffer) in
        charStream.Consume()
            |> should equal (charStream.Consume())

    [<TestCase>] member test.
     ``CharStream.Dispose Test`` () =
        let baseReader = new StringReader("foobarbazqux") in
        let charBuffer = new CharBuffer(baseReader, 1) in
        let charStream = new CharStream(charBuffer) in
        charStream.Dispose();
        buffer charStream
            |> should equal null;