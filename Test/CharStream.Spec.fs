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

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.IO
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec () =
    [<TestMethod>] member test.
     ``When the internal buffer reaches the EOF, Parseq.CharStream.Current should be None`` () =
        let baseReader = StringReader.Null in
        let charBuffer = new Parseq.CharBuffer(baseReader) in
        let charStream = new Parseq.CharStream(charBuffer) in
        charBuffer.Peek() |> should equal CharBuffer.EOF;
        charStream.Current.HasValue
            |> should be False

    [<TestMethod>] member test.
     ``When the internal buffer.Peek() returns c, Parseq.CharStream.Current should be Some((char)c)`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new Parseq.CharBuffer(baseReader) in
        let charStream = new Parseq.CharStream(charBuffer) in
        let c = charBuffer.Peek() in
        charStream.Current.HasValue
            |> should be True;
        charStream.Current.Value
            |> should equal ((char)c)

    [<TestMethod>] member test.
     ``The instance of Parseq.Stream stream, stream.Consume() always returns same value`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new Parseq.CharBuffer(baseReader) in
        let charStream = new Parseq.CharStream(charBuffer) in
        charStream.Consume()
            |> should equal (charStream.Consume());
        let charStream = charStream.Consume() in
        charStream.Consume()
            |> should equal (charStream.Consume())

    [<TestMethod>] member test.
     ``When the internal buffer.Peek() returns CR-LF, do new-line processing`` () =
        let baseReader = new StringReader(" \r\n") in
        let charBuffer = new Parseq.CharBuffer(baseReader) in
        let charStream = new Parseq.CharStream(charBuffer) in
        let initPosition = new Position(1, 1, 0) in
        charStream.Current.Value |> should equal ' ';
        charStream.CurrentPosition |> should equal initPosition;
        (* \r *)
        let charStream = charStream.Consume() in
        let position = new Position(initPosition.Line, initPosition.Column + 1, initPosition.Index + 1) in
        charStream.Current.Value |> should equal '\r';
        charStream.CurrentPosition |> should equal position;
        (* \n *)
        let charStream = charStream.Consume() in
        let position = new Position(position.Line + 1, 1, position.Index + 1) in
        charStream.Current.Value |> should equal '\n';
        charStream.CurrentPosition |> should equal position;

     [<TestMethod>] member test.
      ``When the internal buffer.Peek() returns CR, do new-line processing`` () =
        let baseReader = new StringReader(" \r") in
        let charBuffer = new Parseq.CharBuffer(baseReader) in
        let charStream = new Parseq.CharStream(charBuffer) in
        let initPosition = new Position(1, 1, 0) in
        charStream.Current.Value |> should equal ' ';
        charStream.CurrentPosition |> should equal initPosition;
        (* \r (\n ∉ LookAhead(1)) *)
        let charStream = charStream.Consume() in
        let position = new Position(initPosition.Line + 1, 1, initPosition.Index + 1) in
        charStream.Current.Value |> should equal '\r';
        charStream.CurrentPosition |> should equal position;

    [<TestMethod>] member test.
     ``When the internal buffer.Peek() returns LF, do new-line processing`` () =
        let baseReader = new StringReader(" \n") in
        let charBuffer = new Parseq.CharBuffer(baseReader) in
        let charStream = new Parseq.CharStream(charBuffer) in
        let initPosition = new Position(1, 1, 0) in
        charStream.Current.Value |> should equal ' ';
        charStream.CurrentPosition |> should equal initPosition;
        (* \n *)
        let charStream = charStream.Consume() in
        let position = new Position(initPosition.Line + 1, 1, initPosition.Index + 1) in
        charStream.Current.Value |> should equal '\n';
        charStream.CurrentPosition |> should equal position;
