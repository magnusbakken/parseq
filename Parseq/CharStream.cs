/*
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
using System.IO;

namespace Parseq
{
    public class CharStream
        : IStream<Char>
        , IDisposable
    {
        private IDelayed<CharStream> restStream;
        private IDisposable buffer;

        public CharStream(TextReader reader, Position position)
            : this(new CharBuffer(reader), position)
        {

        }

        public CharStream(TextReader reader)
            : this(reader, new Position(1, 1, 0))
        {

        }

        public CharStream(CharBuffer buffer, Position position)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            this.CurrentPosition = position;
            this.Current = Option.Try(valueFactory: () =>
                (CharBuffer.EOF == buffer.Peek())
                    ? Option.None<Char>()
                    : Option.Some<Char>((Char)buffer.Read()))
                    .SelectMany(_ => _);

            this.restStream = Delayed.Create(valueFactory: () =>
                (buffer.Peek(0) == '\n' || (buffer.Peek() == '\r' && buffer.Peek(1) != '\n'))
                    ? new CharStream(buffer, new Position(position.Line + 1, 1, position.Index + 1))
                    : new CharStream(buffer, new Position(position.Line, position.Column + 1, position.Index + 1)));
            this.buffer = buffer;
        }

        public IOption<Char> Current
        {
            get;
            private set;
        }

        public Position CurrentPosition
        {
            get;
            private set;
        }

        public IStream<Char> Consume()
        {
            return this.restStream.Force();
        }

        public virtual void Dispose()
        {
            if (this.buffer != null)
            {
                this.buffer.Dispose();
                this.buffer = null;
            }
        }
    }
}
