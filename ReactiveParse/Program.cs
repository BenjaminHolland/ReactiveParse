using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
namespace ReactiveParse
{
    public interface IInput<T>
    {
        IImmutableList<T> Head { get; }
        IObservable<T> Tail { get; }
    }
    public interface IResult<T>
    {
        IInput<T> State { get; }
        T Current { get; }
    }
    
    public delegate IObservable<IResult<T>> Parser<T>(IInput<T> input);
    class Input<T> : IInput<T>
    {
        public Input(IImmutableList<T> head,IObservable<T> tail)
        {
            Head = head;
            Tail = tail;
        }
        public Input(IImmutableList<T> head,T body,IObservable<T> tail)
        {
            Head = head.Add(body);
            Tail = tail;
        }
        public IImmutableList<T> Head { get; }
        public IObservable<T> Tail { get; }
    }

    class Result<T> : IResult<T>
    {
        public IInput<T> State { get; }
        public T Current { get; }
        public Result(T current,IInput<T> state)
        {
            State = state;
            Current = current;
        }
    }
    public static class ReactiveParse
    {
        public static Parser<T> None<T>()
        {
            return input=>Observable.Empty<IResult<T>>();
        }
        public static Parser<T> Result<T>(T current,IInput<T> state)
        {
            return input => Observable.Return(new Result<T>(current,state));
        }
        public static Parser<T> Read<T>()
        {
            return input => Observable.Create<IResult<T>>(output =>
            {
                return input.Tail.Subscribe(c =>
                {
                    output.OnNext(new Result<T>(c, new Input<T>(input.Head,c,input.Tail)));
                });
            });
        }
      

    }
    class Program
    {

        static void Main(string[] args)
        {
        }
    }
}
