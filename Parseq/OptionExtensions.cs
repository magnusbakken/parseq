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
    public static partial class Option
    {
        public static T Get<T>(this IOption<T> option)
        {
            return option.Value;
        }
    }

    public static class OptionExtensions
    {
        public static IOption<T> Where<T>(
            this IOption<T> option,
                 Func<T, Boolean> predicate)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return option.HasValue && predicate(option.Value)
                ? option
                : Option.None<T>();
        }

        public static IOption<T1> Select<T0, T1>(
            this IOption<T0> option,
                 Func<T0, T1> selector)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return option.HasValue
                ? Option.Some<T1>(selector(option.Value))
                : Option.None<T1>();
        }

        public static IOption<T1> SelectMany<T0, T1>(
            this IOption<T0> option,
                 Func<T0, IOption<T1>> selector)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            if (selector == null)
                throw new ArgumentNullException("selector");

            return option.HasValue
                ? selector(option.Value)
                : Option.None<T1>();
        }

        public static IOption<T2> SelectMany<T0, T1, T2>(
            this IOption<T0> option,
                 Func<T0, IOption<T1>> selector,
                 Func<T0, T1, T2> projector)
        {
            if (option == null)
                throw new ArgumentNullException("option");
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (projector == null)
                throw new ArgumentNullException("projector");

            return option.SelectMany(_0 => selector(_0).Select(_1 => projector(_0, _1)));
        }
    }
}
