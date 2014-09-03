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
    public interface IReply<TToken, TResult>
        : IEither<String, TResult>
    {
        IStream<TToken> RestStream { get; }

        new T Case<T>(Func<String, T> failure, Func<TResult, T> success);
    }

    public static partial class Reply
    {
        public static IReply<TToken, T> Success<TToken, T>(IStream<TToken> restStream, T value)
        {
            return new Reply.SuccessImpl<TToken, T>(restStream, value);
        }

        public static IReply<TToken, T> Failure<TToken, T>(IStream<TToken> restStream, String errorMessage)
        {
            return new Reply.FailureImpl<TToken, T>(restStream, errorMessage);
        }
    }

    public static partial class Reply
    {
        class SuccessImpl<TToken, TResult>
            : IReply<TToken, TResult>
        {
            public SuccessImpl(IStream<TToken> restStream, TResult value)
            {
                if (restStream == null)
                    throw new ArgumentNullException("restStream");

                this.RestStream = restStream;
                this.Value = value;
            }

            public IStream<TToken> RestStream
            {
                get;
                private set;
            }

            public TResult Value
            {
                get;
                private set;
            }

            T IReply<TToken, TResult>.Case<T>(Func<string, T> failure, Func<TResult, T> success)
            {
                if (failure == null)
                    throw new ArgumentNullException("failure");
                if (success == null)
                    throw new ArgumentNullException("success");

                return success(this.Value);
            }

            T IEither<string, TResult>.Case<T>(Func<string, T> left, Func<TResult, T> right)
            {
                if (left == null)
                    throw new ArgumentNullException("left");
                if (right == null)
                    throw new ArgumentNullException("right");

                return right(this.Value);
            }
        }

        class FailureImpl<TToken, TResult>
            : IReply<TToken, TResult>
        {
            public FailureImpl(IStream<TToken> restStream, String errorMessage)
            {
                if (restStream == null)
                    throw new ArgumentNullException("restStream");
                if (errorMessage == null)
                    throw new ArgumentNullException("errorMessage");

                this.RestStream = restStream;
                this.ErrorMessage = errorMessage;
            }

            public IStream<TToken> RestStream
            {
                get;
                private set;
            }

            public String ErrorMessage
            {
                get;
                private set;
            }

            T IReply<TToken, TResult>.Case<T>(Func<string, T> failure, Func<TResult, T> success)
            {
                if (failure == null)
                    throw new ArgumentNullException("failure");
                if (success == null)
                    throw new ArgumentNullException("success");

                return failure(this.ErrorMessage);
            }

            T IEither<string, TResult>.Case<T>(Func<string, T> left, Func<TResult, T> right)
            {
                if (left == null)
                    throw new ArgumentNullException("left");
                if (right == null)
                    throw new ArgumentNullException("right");

                return left(this.ErrorMessage);
            }
        }
    }
}
