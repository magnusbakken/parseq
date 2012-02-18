﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parseq.Combinators;

namespace Parseq
{
    public static class Combinator {

        public static Parser<TToken, TResult> Or<TToken, TResult>(
            this Parser<TToken, TResult> lparser,
            Parser<TToken, TResult> rparser)
        {
            if (lparser == null)
                throw new ArgumentNullException("lparser");
            if (rparser == null)
                throw new ArgumentNullException("rparser");

            return stream => {
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                switch ((reply = lparser(stream)).TryGetValue(out result,out message)){
                    case ReplyStatus.Success: return reply;
                    case ReplyStatus.Failure: return rparser(stream);
                    default: return Reply.Error<TToken, TResult>(reply.Stream, message);
                }
            };
        }

        public static Parser<TToken, TResult> And<TToken, TResult>(
            this Parser<TToken, TResult> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return stream => {
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                switch ((reply = parser(stream)).TryGetValue(out result, out message)){
                    case ReplyStatus.Success: return Reply.Success<TToken, TResult>(stream, result);
                    case ReplyStatus.Failure: return Reply.Failure<TToken, TResult>(stream);
                    default: return Reply.Error<TToken, TResult>(reply.Stream, message);
                }
            };
        }

        public static Parser<TToken, Unit> Not<TToken, TResult>(
            this Parser<TToken, TResult> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return stream =>{
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                switch ((reply = parser(stream)).TryGetValue(out result, out message)){
                    case ReplyStatus.Success: return Reply.Failure<TToken, Unit>(stream);
                    case ReplyStatus.Failure: return Reply.Success<TToken, Unit>(stream, Unit.Instance);
                    default: return Reply.Error<TToken, Unit>(reply.Stream, message);
                }
            };
        }

        public static Parser<TToken, Option<TResult>> Maybe<TToken, TResult>(
            this Parser<TToken, TResult> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            return stream =>{
                Reply<TToken, TResult> reply;
                TResult value; ErrorMessage message;
                switch ((reply = parser(stream)).TryGetValue(out value, out message)){
                    case ReplyStatus.Success: return Reply.Success<TToken, Option<TResult>>(stream, Option.Just(value));
                    case ReplyStatus.Failure: return Reply.Success<TToken, Option<TResult>>(stream, Option.None<TResult>());
                    default: return Reply.Error<TToken, Option<TResult>>(reply.Stream, message);
                }
            };
        }

        public static Parser<TToken, IEnumerable<TResult>> Sequence<TToken, TResult>(
            params Parser<TToken, TResult>[] parsers)
        {
            if (parsers == null)
                throw new ArgumentNullException("parsers");

            return stream => {
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                var collection = new List<TResult>();
                foreach (var p in parsers){
                    switch ((reply = p(stream)).TryGetValue(out result, out message)){
                        case ReplyStatus.Success:
                            collection.Add(result);
                            break;
                        case ReplyStatus.Failure:
                            return Reply.Failure<TToken, IEnumerable<TResult>>(stream);
                        default:
                            return Reply.Error<TToken, IEnumerable<TResult>>(stream, message);
                    }
                    stream = reply.Stream;
                }
                return Reply.Success<TToken,IEnumerable<TResult>>(stream, collection);
            };
        }

        public static Parser<TToken, TResult> Choice<TToken, TResult>(
            params Parser<TToken, TResult>[] parsers)
        {
            if (parsers == null)
                throw new ArgumentNullException("parsers");

            return stream => {
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                foreach (var p in parsers){
                    switch ((reply = p(stream)).TryGetValue(out result, out message)){
                        case ReplyStatus.Success:
                            return Reply.Success<TToken, TResult>(reply.Stream, result);
                        case ReplyStatus.Error:
                            return Reply.Error<TToken, TResult>(reply.Stream, message);
                    }
                }
                return Reply.Failure<TToken, TResult>(stream);
            };
        }

        public static Parser<TToken, IEnumerable<TResult>> Repeat<TToken, TResult>(
            this Parser<TToken, TResult> parser, int count)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            return stream => {
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                var collection = new List<TResult>();
                foreach (var i in Enumerable.Range(0, count)){
                    switch ((reply = parser(stream)).TryGetValue(out result, out message)){
                        case ReplyStatus.Success:
                            collection.Add(result);
                            break;
                        case ReplyStatus.Failure:
                            return Reply.Failure<TToken, IEnumerable<TResult>>(stream);
                        case ReplyStatus.Error:
                            return Reply.Error<TToken, IEnumerable<TResult>>(stream, message);
                    }
                    stream = reply.Stream;
                }
                return Reply.Success<TToken,IEnumerable<TResult>>(stream, collection);
            };
        }

        public static Parser<TToken, IEnumerable<TResult>> Many<TToken, TResult>(
            this Parser<TToken, TResult> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return stream => {
                Reply<TToken, TResult> reply;
                TResult result; ErrorMessage message;
                var collection = new List<TResult>();
                while (true){
                    switch ((reply = parser(stream)).TryGetValue(out result, out message)){
                        case ReplyStatus.Success:
                            collection.Add(result);
                            break;
                        case ReplyStatus.Failure:
                            return Reply.Success<TToken, IEnumerable<TResult>>(stream, collection);
                        case ReplyStatus.Error:
                            return Reply.Error<TToken, IEnumerable<TResult>>(stream, message);
                    }
                    stream = reply.Stream;
                }
            };
        }

        public static Parser<TToken, IEnumerable<TResult>> Many<TToken, TResult>(
            this Parser<TToken, TResult> parser, int count)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            return from x in parser.Repeat(count)
                   from y in parser.Many()
                   select x.Concat(y);
        }

        public static Parser<TToken, Unit> Ignore<TToken, TResult>(
            this Parser<TToken, TResult> parser)
        {
            if (parser == null)
                throw new ArgumentNullException("parser");

            return parser.Select(_ => Unit.Instance);
        }
    }
}
