using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveParse
{
    public interface IInput
    {
        string Prefix { get; }
        char Head { get; }
    }
    public interface IResult
    {
        Char Head { get; }
        IObservable<char> Tail { get; }
    }

    public delegate IObservable<IResult> Parser(IObservable<char> input);
    class Result : IResult
    {
        public char Head { get; }

        public IObservable<char> Tail { get; }
        public Result(char head,IObservable<char> tail)
        {
            Head = head;
            Tail = tail;
        }
    }
    public static class ReactiveParse
    {
        public static Parser Zero()
        {
            return input=>Observable.Empty<IResult>();
        }
        public static Parser Result(char c)
        {
            return input => Observable.Return(new Result(c,Observable.Empty<char>()));
        }
        public static Parser Item()
        {
            return input => Observable.Create<IResult>(output =>
            {
                return input.Subscribe(c =>
                {
                    output.OnNext(new Result(c, input));
                });
            });
        }
        public static Parser Bind(this Parser first,Func<char,Parser> nextFactory)
        {
            return input => first(input).Select(r1 => nextFactory(r1.Head)(r1.Tail)).Merge();
        }

    }
    class Program
    {

        static void Main(string[] args)
        {
        }
    }
}
