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

namespace Parseq
{
    public struct Position
        : IEquatable<Position>
        , IComparable<Position>
    {
        public Position(Int32 line, Int32 column, Int32 index) : this()
        {
            if (line < 1)
                throw new ArgumentOutOfRangeException("line");
            if (column < 1)
                throw new ArgumentOutOfRangeException("column");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            this.Line = line;
            this.Column = column;
            this.Index = index;
        }

        public Int32 Line
        {
            get;
            private set;
        }

        public Int32 Column
        {
            get;
            private set;
        }

        public Int32 Index
        {
            get;
            private set;
        }

        public Int32 CompareTo(Position other)
        {
            return this.Index.CompareTo(other.Index);
        }

        public Boolean Equals(Position other)
        {
            return other.Column == this.Column
                && other.Line == this.Line
                && other.Index == this.Index;
        }

        public override Int32 GetHashCode()
        {
            return unchecked(
                this.Column.GetHashCode() * 171 ^
                this.Line.GetHashCode() * 218 ^
                this.Index.GetHashCode()
            );
        }

        public override Boolean Equals(Object obj)
        {
            return obj is Position && this.Equals((Position)obj);
        }

        public override String ToString()
        {
            return this.Line + ":" + this.Column;
        }

        public static Boolean operator >(Position x, Position y)
        {
            return x.Index > y.Index;
        }

        public static Boolean operator >=(Position x, Position y)
        {
            return x.Index >= y.Index;
        }

        public static Boolean operator <(Position x, Position y)
        {
            return x.Index < y.Index;
        }

        public static Boolean operator <=(Position x, Position y)
        {
            return x.Index <= y.Index;
        }

        public static Boolean operator ==(Position x, Position y)
        {
            return x.Equals(y);
        }

        public static Boolean operator !=(Position x, Position y)
        {
            return !(x.Equals(y));
        }
    }
}
