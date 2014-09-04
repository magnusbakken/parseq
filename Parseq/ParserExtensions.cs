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
    public static class Parser
    {
        public static IReply<TToken, T> Run<TToken, T>(
            this Parser<TToken, T> parser,
                 IStream<TToken> stream)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (stream == null)
                throw new ArgumentNullException("stream");
            try
            {
                return parser(stream);
            }
            catch (FailFastException<TToken> e)
            {
                return Reply.Failure<TToken, T>(e.RestStream, e.Message);
            }
        }

        public static Parser<TToken, T> Return<TToken, T>(T value)
        {
            return stream => Reply.Success<TToken, T>(stream, value);
        }

        public static Parser<TToken, T> Fail<TToken, T>(String errorMessage)
        {
            if (errorMessage == null)
                throw new ArgumentNullException("errorMessage");

            return stream => Reply.Failure<TToken, T>(stream, errorMessage);
        }

        public static Parser<TToken, T> FailFast<TToken, T>(String errorMessage)
        {
            return stream =>
                { throw new FailFastException<TToken>(stream, errorMessage); };
        }

        internal class FailFastException<TToken>
            : Exception
        {
            public FailFastException(IStream<TToken> restStream, String errorMessage)
                : base(errorMessage)
            {
                if (restStream == null)
                    throw new ArgumentNullException("restStream");

                this.RestStream = restStream;
            }

            public IStream<TToken> RestStream
            {
                get;
                private set;
            }
        }
    }

    public static class ParserExtensions
    {
        public static Parser<TToken, T> Where<TToken, T>(
            this Parser<TToken, T> parser,
                 Func<T, Boolean> predicate)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return stream => parser(stream).Where(predicate, () => "Filtered by the `Where` operator")
                .Case(
                    left: _ => Reply.Failure<TToken, T>(stream, _),
                    right: _ => Reply.Success<TToken, T>(stream, _));
        }

        public static Parser<TToken, T1> Select<TToken, T0, T1>(
            this Parser<TToken, T0> parser,
                 Func<T0, T1> selector)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return stream => parser(stream).Case(
                success: (restStream, value) =>
                    Reply.Success<TToken, T1>(restStream, selector(value)),
                failure: Reply.Failure<TToken, T1>);
        }

        public static Parser<TToken, T1> SelectMany<TToken, T0, T1>(
            this Parser<TToken, T0> parser,
                 Func<T0, Parser<TToken, T1>> selector)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return stream => parser(stream).Case(
                success: (restStream, value) =>
                    selector(value)(restStream),
                failure: Reply.Failure<TToken, T1>);
        }

        public static Parser<TToken, T2> SelectMany<TToken, T0, T1, T2>(
            this Parser<TToken, T0> parser,
                 Func<T0, Parser<TToken, T1>> selector,
                 Func<T0, T1, T2> projector)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (projector == null)
                throw new ArgumentNullException("projector");

            return parser.SelectMany(_0 => selector(_0).Select(_1 => projector(_0, _1)));
        }

        public static Parser<TToken, T> DoWhenSuccess<TToken, T>(
            this Parser<TToken, T> parser,
                 Action<T> action)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (action == null)
                throw new ArgumentNullException("action");

            return stream => parser(stream).Case(
                success: (restStream, value) =>
                    { action(value); return Reply.Success<TToken, T>(restStream, value); },
                failure: Reply.Failure<TToken, T>);
        }

        public static Parser<TToken, T> DoWhenFailure<TToken, T>(
            this Parser<TToken, T> parser,
                 Action<String> action)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (action == null)
                throw new ArgumentNullException("action");

            return stream => parser(stream).Case(
                failure: (restStream, errorMessage) =>
                { action(errorMessage); return Reply.Failure<TToken, T>(restStream, errorMessage); },
                success: Reply.Success<TToken, T>);
        }
    }
}
