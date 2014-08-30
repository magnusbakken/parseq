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
    public interface IEither<TL, TR>
    {
        T Case<T>(Func<TL, T> left, Func<TR, T> right);
    }

    public static partial class Either
    {
        public static IEither<TL, TR> Left<TL, TR>(TL value)
        {
            return new Either.LeftImpl<TL, TR>(value);
        }

        public static IEither<TL, TR> Right<TL, TR>(TR value)
        {
            return new Either.RightImpl<TL, TR>(value);
        }
    }

    public static partial class Either
    {
        class LeftImpl<TL, TR>
            : IEither<TL, TR>
        {
            public LeftImpl(TL value)
            {
                this.Value = value;
            }

            public TL Value
            {
                get;
                private set;
            }

            public T Case<T>(Func<TL, T> left, Func<TR, T> right)
            {
                return left(this.Value);
            }
        }

        class RightImpl<TL, TR>
            : IEither<TL, TR>
        {
            public RightImpl(TR value)
            {
                this.Value = value;
            }

            public TR Value
            {
                get;
                private set;
            }

            public T Case<T>(Func<TL, T> left, Func<TR, T> right)
            {
                return right(this.Value);
            }
        }
    }
}
