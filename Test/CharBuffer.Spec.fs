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
module Test.CharBuffer

open Microsoft.VisualStudio.TestTools.UnitTesting
open System
open System.IO
open System.Linq
open Parseq

open FsUnit.MsTest

[<TestClass>]
type Spec () =

    let buffer obj =
        (new PrivateObject(obj)).GetField("buffer") :?> char array

    let bufferPtr obj =
        (new PrivateObject(obj)).GetField("bufferPtr") :?> int

    let bufferCount obj =
        (new PrivateObject(obj)).GetField("bufferCount") :?> int

    [<TestMethod>] member test.
     ``The buffer size must be Parseq.CharBuffer.MinimumBufferSize and more`` () =
        let k = CharBuffer.MinimumBufferSize - 1 in
        let baseReader = new StringReader("foobar") in
        buffer (new Parseq.CharBuffer(baseReader, k))
            |> Array.length
            |> should equal (Parseq.CharBuffer.MinimumBufferSize)

    [<TestMethod>] member test.
     ``The buffer size must be default, If the buffer size is not specified`` () =
        let baseReader = new StringReader("foobar") in
        buffer (new Parseq.CharBuffer(baseReader))
            |> Array.length
            |> should equal (Parseq.CharBuffer.DefaultBufferSize)

    [<TestMethod>] member test.
     ``If the size of the buffer is enough, it will not be extended`` () =
        let enoughBufferSize = 8 in
        let baseReader = new StringReader("foobar") in
        let charBuffer = new Parseq.CharBuffer(baseReader, enoughBufferSize) in
        let oldBufferSize = buffer charBuffer |> Array.length in
        charBuffer.Peek(3) |> should equal ((int)'b');
        let newBufferSize = buffer charBuffer |> Array.length in
        newBufferSize |> should equal oldBufferSize

    [<TestMethod>] member test.
     ``If the size of the buffer is not enough, it is automatically extended`` () =
        let notEnoughBufferSize = 1 in
        let baseReader = new StringReader("foobar") in
        let charBuffer = new Parseq.CharBuffer(baseReader, notEnoughBufferSize) in
        let oldBufferSize = buffer charBuffer |> Array.length in
        charBuffer.Peek(3) |> should equal ((int)'b');
        let newBufferSize = buffer charBuffer |> Array.length in
        newBufferSize |> should not' (equal oldBufferSize);
        newBufferSize |> should equal 4

    [<TestMethod>] member test.
     ``When the buffer reaches the EOF, buffer.Peek() returns Parseq.CharBuffer.EOF`` () =
        let baseReader = StringReader.Null in
        (new Parseq.CharBuffer(baseReader)).Peek()
            |> should equal CharBuffer.EOF

    [<TestMethod>] member test.
     ``When the buffer reaches the EOF, buffer.Read() returns Parseq.CharBuffer.EOF`` () =
        let baseReader = StringReader.Null in
        (new Parseq.CharBuffer(baseReader)).Read()
            |> should equal CharBuffer.EOF

    [<TestMethod>] member test.
      ``Lookahead Test`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new Parseq.CharBuffer(baseReader, 3) in
        charBuffer.Peek(0) |> should equal (charBuffer.Peek());
        charBuffer.Peek(0) |> should equal ((int)'f');
        charBuffer.Peek(1) |> should equal ((int)'o');
        charBuffer.Peek(2) |> should equal ((int)'o');
        charBuffer.Peek(3) |> should equal ((int)'b');
        charBuffer.Peek(4) |> should equal ((int)'a');
        charBuffer.Peek(5) |> should equal ((int)'r');
        charBuffer.Peek(6) |> should equal Parseq.CharBuffer.EOF;
        charBuffer.Peek(7) |> should equal Parseq.CharBuffer.EOF

    [<TestMethod>] member test.
      ``Parseq.CharBuffer.Peek(k) always equals Parseq.CharBuffer.Read(k)`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new Parseq.CharBuffer(baseReader, 3) in
        charBuffer.Peek(0) |> should equal (charBuffer.Read(0));
        charBuffer.Peek(1) |> should equal (charBuffer.Read(1));
        charBuffer.Peek(3) |> should equal (charBuffer.Read(3));