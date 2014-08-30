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
    public static partial class Either
    {
        public static T Throw<TException, T>(this IEither<TException, T> either)
            where TException : Exception
        {
            if (either == null)
                throw new ArgumentNullException("either");

            return either.Case(ex => { throw ex; }, _ => _);
        }

        public static TL GetL<TR, TL>(this IEither<TL, TR> either)
        {
            if (either == null)
                throw new ArgumentNullException("either");

            return either.Case(_ => _, _ => { throw new InvalidOperationException(); });
        }

        public static TR GetR<TR, TL>(this IEither<TL, TR> either)
        {
            if (either == null)
                throw new ArgumentNullException("either");

            return either.Case(_ => { throw new InvalidOperationException(); }, _ => _);
        }
    }

    public static class EitherExtensions
    {
        public static IEither<TException, T> Where<TException, T>(
            this IEither<TException, T> either,
                 Func<T, Boolean> predicate,
                 Func<TException> leftValueFactory)
        {
            if (either == null)
                throw new ArgumentNullException("either");
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (leftValueFactory == null)
                throw new ArgumentNullException("leftValueFactory");

            return either.Case(
                left: _ => Either.Left<TException, T>(_),
                right: _ => predicate(_)
                    ? Either.Right<TException, T>(_)
                    : Either.Left<TException, T>(leftValueFactory()));
        }

        public static IEither<TException, T> Where<TException, T>(
            this IEither<TException, T> either,
                 Func<T, Boolean> predicate)
            where TException : new()
        {
            return EitherExtensions.Where(either, predicate, () => new TException());
        }

        public static IEither<TException, T1> Select<TException, T0, T1>(
            this IEither<TException, T0> either,
                 Func<T0, T1> selector)
        {
            if (either == null)
                throw new ArgumentNullException("either");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return either.Case(
                left: _ => Either.Left<TException, T1>(_),
                right: _ => Either.Right<TException, T1>(selector(_)));
        }

        public static IEither<TException, T1> SelectMany<TException, T0, T1>(
            this IEither<TException, T0> either,
                 Func<T0, IEither<TException, T1>> selector)
        {
            if (either == null)
                throw new ArgumentNullException("either");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return either.Case(
                left: _ => Either.Left<TException, T1>(_),
                right: _ => selector(_));
        }

        public static IEither<TException, T2> SelectMany<TException, T0, T1, T2>(
            this IEither<TException, T0> either,
                 Func<T0, IEither<TException, T1>> selector,
                 Func<T0, T1, T2> projector)
        {
            if (either == null)
                throw new ArgumentNullException("either");
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (projector == null)
                throw new ArgumentNullException("projector");

            return either.SelectMany(_0 => selector(_0).Select(_1 => projector(_0, _1)));
        }
    }
}
