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
    public interface IDelayed<out TValue>
        : IOption<TValue>
    {
        new Boolean HasValue
        {
            get;
        }
    }

    public static partial class Delayed
    {
        public static IDelayed<TValue> Create<TValue>(TValue value)
        {
            return new Delayed.ValueImpl<TValue>(value);
        }

        public static IDelayed<TValue> Create<TValue>(Func<TValue> valueFactory)
        {
            return new Delayed.ValueFactoryImpl<TValue>(valueFactory);
        }
    }

    public static partial class Delayed
    {
        class ValueImpl<TValue>
            : IDelayed<TValue>
        {
            public ValueImpl(TValue value)
            {
                this.Value = value;
            }

            public TValue Value
            {
                get;
                private set;
            }

            public Boolean HasValue
            {
                get { return true; }
            }

            Boolean IOption<TValue>.HasValue
            {
                get { return true; }
            }
        }

        class ValueFactoryImpl<TValue>
            : IDelayed<TValue>
        {
            private readonly Lazy<TValue> lazyInstance;

            public ValueFactoryImpl(Func<TValue> valueFactory)
            {
                if (valueFactory == null)
                    throw new ArgumentNullException("valueFactory");

                this.lazyInstance = new Lazy<TValue>(valueFactory);
            }

            public Boolean HasValue
            {
                get { return this.lazyInstance.IsValueCreated; }
            }

            public TValue Value
            {
                get { return this.lazyInstance.Value; }
            }

            Boolean IOption<TValue>.HasValue
            {
                get { return true; }
            }
        }
    }
}
