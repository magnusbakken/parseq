﻿/*
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
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parseq
{
    internal class CharBuffer
        : System.IO.TextReader
    {
        public const Int32 EOF = -1;
        public const Int32 DefaultBufferSize = 8;
        public const Int32 MinimumBufferSize = 1;

        private System.IO.TextReader baseReader;
        private Char[] buffer;
        private Int32 bufferPtr;
        private Int32 bufferCount;

        public CharBuffer(System.IO.TextReader baseReader, Int32 bufferSize)
        {
            if (baseReader == null)
                throw new ArgumentNullException("baseReader");

            this.baseReader = baseReader;
            this.buffer = new Char[Math.Max(bufferSize, CharBuffer.MinimumBufferSize)];
            this.bufferPtr = 0;
            this.bufferCount = 0;
        }

        public CharBuffer(System.IO.TextReader baseReader)
            : this(baseReader, DefaultBufferSize)
        {

        }

        void LookAhead(Int32 k)
        {
            if (k <= 0) throw new ArgumentOutOfRangeException("k");

            if (this.bufferCount - this.bufferPtr >= k)
                return;

            var sourceBuffer = this.buffer;
            var destinationBuffer = (k > this.buffer.Length)
                ? new Char[k]
                : this.buffer;

            Array.Copy(sourceBuffer, this.bufferPtr, destinationBuffer, 0, sourceBuffer.Length - this.bufferPtr);
            this.bufferCount -= this.bufferPtr;
            this.bufferCount += this.baseReader.Read(destinationBuffer, this.bufferCount, destinationBuffer.Length - this.bufferCount);
            this.bufferPtr = 0;
            this.buffer = destinationBuffer;
        }

        public Int32 Peek(Int32 k)
        {
            if (k <= 0) throw new ArgumentOutOfRangeException("k");

            this.LookAhead(k);
            return (this.bufferPtr + (k - 1) >= this.bufferCount)
                ? CharBuffer.EOF
                : (Int32)this.buffer[this.bufferPtr + (k - 1)];
        }

        public Int32 Read(Int32 k)
        {
            if (k <= 0) throw new ArgumentOutOfRangeException("k");

            var c = this.Peek(k);
            if (c != CharBuffer.EOF) { this.bufferPtr += k; }

            return c;
        }

        public override Int32 Read(Char[] buffer, Int32 index, Int32 count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            this.LookAhead(count);
            Array.Copy(this.buffer, this.bufferPtr, buffer, index, count = Math.Min(count, this.bufferCount - this.bufferPtr));
            this.bufferPtr += count;
            return count;
        }

        public override Int32 ReadBlock(Char[] buffer, Int32 index, Int32 count)
        {
            throw new NotSupportedException();
        }

        public override Int32 Peek()
        {
            return this.Peek(1);
        }

        public override Int32 Read()
        {
            return this.Read(1);
        }

        public override String ReadLine()
        {
            var builder = new StringBuilder();
            while (true)
            {
                switch (this.Peek(1))
                {
                    case CharBuffer.EOF:
                    case '\n':
                        this.Read(1);
                        goto exit;
                    case '\r':
                        if (this.Peek(2) == '\n') this.Read(1);
                        this.Read(1);
                        goto exit;
                    default:
                        builder.Append((Char)this.Read(1));
                        break;
                }
            }
        exit:
            return builder.ToString();
        }

        public override String ReadToEnd()
        {
            var builder = new StringBuilder();
            while (this.Peek() != CharBuffer.EOF)
                builder.Append((Char)this.Read());
            return builder.ToString();
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing && this.baseReader != null)
            {
                this.baseReader.Dispose();
                this.baseReader = null;
                this.buffer = null;
            }

            base.Dispose(disposing);
        }
    }
}
