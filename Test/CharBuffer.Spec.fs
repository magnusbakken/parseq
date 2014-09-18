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
open System
open System.IO
open System.Reflection
open Parseq

open NUnit.Framework
open FsUnit

[<TestFixture>]
type Spec () =
(* private field accessor *)
    let bindingFlags =
        BindingFlags.SetField |||
        BindingFlags.GetField |||
        BindingFlags.Static |||
        BindingFlags.Instance |||
        BindingFlags.NonPublic |||
        BindingFlags.Public 

    let buffer (buf : CharBuffer) =
        typeof<CharBuffer>.GetField("buffer", bindingFlags).GetValue(buf) :?> char array

    let bufferPtr (buf : CharBuffer) =
        typeof<CharBuffer>.GetField("bufferPtr", bindingFlags).GetValue(buf) :?> int

    let bufferCount (buf : CharBuffer) =
        typeof<CharBuffer>.GetField("bufferCount", bindingFlags).GetValue(buf) :?> int

    let reader (buf : CharBuffer) =
        typeof<CharBuffer>.GetField("baseReader", bindingFlags).GetValue(buf) :?> TextReader

    [<TestCase>] member test.
     ``CharBuffer.Peek(k) will return CharBuffer.EOF if the CharBuffer reaches EOF`` () =
        let baseReader = TextReader.Null in
        (new CharBuffer(baseReader)).Peek()
            |> should equal (CharBuffer.EOF)

    [<TestCase>] member test.
     ``CharBuffer.Read(k) will return CharBuffer.EOF if the CharBuffer reaches EOF`` () =
        let baseReader = TextReader.Null in
        (new CharBuffer(baseReader)).Read()
            |> should equal (CharBuffer.EOF)

    [<TestCase>] member test.
     ``The buffer size must be CharBuffer.MinimumBufferSize or more`` () =
        let baseReader = TextReader.Null in
        let bufferSize = CharBuffer.MinimumBufferSize - 1 in
        buffer (new CharBuffer(baseReader, bufferSize))
            |> Array.length
            |> should equal CharBuffer.MinimumBufferSize

    [<TestCase>] member test.
     ``The buffer size must be default, If the buffer size is not specified`` () =
        let baseReader = TextReader.Null in
        buffer (new CharBuffer(baseReader))
            |> Array.length
            |> should equal CharBuffer.DefaultBufferSize

    [<TestCase>] member test.
     ``If the size of the buffer is enough, it will not be extended`` () =
        let bufferSize = 8 in
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader, bufferSize) in
        let oldBufferSize = buffer charBuffer |> Array.length in
        charBuffer.Peek(3) |> should equal ((int)'o');
        let newBufferSize = buffer charBuffer |> Array.length in
        newBufferSize |> should equal oldBufferSize
    
    [<TestCase>] member test.
     ``If the size of the buffer is not enough, it is automatically extended`` () =
        let bufferSize = 1 in
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader, bufferSize) in
        let oldBufferSize = buffer charBuffer |> Array.length in
        charBuffer.Peek(3) |> should equal ((int)'o');
        let newBufferSize = buffer charBuffer |> Array.length in
        newBufferSize |> should not' (equal oldBufferSize);
        newBufferSize |> should equal 3

    [<TestCase>] member test.
     ``CharBuffer.Peek(k) throws an ArgumentOutOfRangeException if the argument k less than 1`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader, 3) in
        (fun () -> charBuffer.Peek(-1) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>
     
    [<TestCase>] member test.
     ``CharBuffer.Peek(k) returns the character which is the k-lookahead, and it does not consume any input`` () = 
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader, 3) in
        charBuffer.Peek(1) |> should equal (charBuffer.Peek());
        charBuffer.Peek(1) |> should equal ((int)'f');
        charBuffer.Peek(2) |> should equal ((int)'o');
        charBuffer.Peek(3) |> should equal ((int)'o');
        charBuffer.Peek(4) |> should equal ((int)'b');
        charBuffer.Peek(5) |> should equal ((int)'a');
        charBuffer.Peek(6) |> should equal ((int)'r');
        charBuffer.Peek(7) |> should equal CharBuffer.EOF;
        charBuffer.Peek(8) |> should equal CharBuffer.EOF;
        (* To make sure that it does not consume any input *)
        charBuffer.Read(1) |> should equal ((int) 'f')

     [<TestCase>] member test.
      ``CharBuffer.Read(k) throws an ArgumentOutOfRangeException if the argument k less than 1`` () =
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader, 3) in
        (fun () -> charBuffer.Read(-1) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>

    [<TestCase>] member test.
     ``CharBuffer.Read(k) returns the character which is the k-lookahead, and it also consume an input`` () = 
        let baseReader = new StringReader("foobar") in
        let charBuffer = new CharBuffer(baseReader, 3) in
        charBuffer.Read(3) |> should equal ((int)'o');
        charBuffer.Read(3) |> should equal ((int)'r');
        charBuffer.Peek(1) |> should equal CharBuffer.EOF

    [<TestCase>] member test.
     ``CharBuffer.ReadLine() returns the characters which read until the EOL or EOF`` () =
        let baseReader = new StringReader("foo\r\nbar\nbaz\rqux") in
        let charBuffer = new CharBuffer(baseReader) in
        charBuffer.ReadLine() |> should equal "foo";
        charBuffer.ReadLine() |> should equal "bar";
        charBuffer.ReadLine() |> should equal "baz";
        charBuffer.ReadLine() |> should equal "qux"

    [<TestCase>] member test.
     ``CharBuffer.ReadToEnd() returns the characters which read until the EOF`` () =
        let baseReader = new StringReader("foo\r\nbar\nbaz\rqux") in
        let charBuffer = new CharBuffer(baseReader) in
        charBuffer.ReadToEnd() |> should equal "foo\r\nbar\nbaz\rqux"

    [<TestCase>] member test.
     ``CharBuffer.ReadBlock is not supported`` () =
        let baseReader = TextReader.Null in
        let charBuffer = new CharBuffer(baseReader) in
        (fun () -> charBuffer.ReadBlock([||], 0, 0) |> ignore)
            |> should throw typeof<NotSupportedException> 

    [<TestCase>] member test.
     ``CharBuffer.Read(buf, inde, count) returns the number of characters which were read`` () =
        let baseReader = new StringReader("foobarbazqux") in
        let charBuffer = new CharBuffer(baseReader, 1) in
        let index = 0 in
        let count = 3 in
        let buf = Array.create count '\u0000' in
        charBuffer.Read(buf, index, count)
            |> should equal 3;
        buf |> should equal [|'f';'o';'o'|];
        charBuffer.Read(buf, index, count)
            |> should equal 3;
        buf |> should equal [|'b';'a';'r'|]

    [<TestCase>] member test.
     ``CharBuffer.Read(buf, index, count) throws an ArgumentException if the value of (buf.Length - index) less than count`` () =
        let baseReader = new StringReader("foobarbazqux") in
        let charBuffer = new CharBuffer(baseReader, 1) in
        let index = 0 in
        let count = 3 in
        let buf = Array.create (count - 1) '\u0000' in
        (fun () -> charBuffer.Read(buf, index, count) |> ignore)
            |> should throw typeof<ArgumentException>

    [<TestCase>] member test.
     ``CharBuffer.Read(buf, index, count) throws an ArgumentOutOfRangeException if the argument index or count are negative`` () =
        let baseReader = new StringReader("foobarbazqux") in
        let charBuffer = new CharBuffer(baseReader, 1) in
        let index = -1 in
        let count = 3 in
        let buf = Array.create count '\u0000' in
        (fun () -> charBuffer.Read(buf, index, count) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>;
        let index = 0 in
        let count = -1 in
        (fun () -> charBuffer.Read(buf, index, count) |> ignore)
            |> should throw typeof<ArgumentOutOfRangeException>

    [<TestCase>] member test.
     ``CharBuffer.Dispose Test`` () =
        let baseReader = new StringReader("foobarbazqux") in
        let charBuffer = new CharBuffer(baseReader, 1) in
        charBuffer.Dispose();
        buffer charBuffer
            |> should equal null;
        reader charBuffer
            |> should equal null;