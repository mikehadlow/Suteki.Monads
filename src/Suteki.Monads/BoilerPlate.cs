using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Suteki.Monads
{
    public class BoilerPlate
    {
        public Pipeline<TransformedItem> SomeBoilerPlateCode(Pipeline<Item> items)
        {
            var transformedItems = new Pipeline<TransformedItem>(items.Failures);
            foreach (var item in items.Values)
            {
                if(item.HasValue)
                {
                    try
                    {
                        var transformedItem = TransformItem(item);
                        transformedItems.AddValue(transformedItem);
                    }
                    catch (TransformationFailedException ex)
                    {
                        transformedItems.RecordFailure(ex);
                    }
                }
            }
            return transformedItems;
        }

        public Pipeline<TransformedItem> SomeMonadicCode(Pipeline<Item> items)
        {
            return from item in items
                    select TransformItem(item);
        }

        private static TransformedItem TransformItem(Item item)
        {
            throw new NotImplementedException();
        }
    }

    public class TransformItemsResult
    {
        public IEnumerable<TransformedItem> TransformedItems { get; private set; }
        public IEnumerable<TransformationFailedException> Failures { get; private set; }

        public TransformItemsResult(IEnumerable<TransformedItem> transformedItems, IEnumerable<TransformationFailedException> failures)
        {
            TransformedItems = transformedItems;
            Failures = failures;
        }
    }

    public class Pipeline<TValue> 
        where TValue : INullable 
    {
        public IList<TValue> Values { get; private set; }
        public IList<TransformationFailedException> Failures { get; private set; }

        public Pipeline() : this(Enumerable.Empty<TransformationFailedException>())
        {
        }

        public Pipeline(IEnumerable<TransformationFailedException> failures)
        {
            Values = new List<TValue>();
            Failures = new List<TransformationFailedException>();
            foreach (var transformationFailedException in failures)
            {
                Failures.Add(transformationFailedException);
            }
        }

        public Pipeline<TValue> AddValue(TValue value)
        {
            Values.Add(value);
            return this;
        }

        public Pipeline<TValue> RecordFailure(TransformationFailedException exception)
        {
            Failures.Add(exception);
            return this;
        }
    }

    public static class PipelineExtensions
    {
        public static Pipeline<T> ToPipeline<T>(this T item) where T : INullable
        {
            return new Pipeline<T>().AddValue(item);
        }

        public static Pipeline<B> Bind<A, B>(this Pipeline<A> a, Func<A, Pipeline<B>> func) 
            where A : INullable
            where B : INullable
        {
            var bpipeline = new Pipeline<B>(a.Failures);
            foreach (var aval in a.Values)
            {
                if (aval.HasValue)
                {
                    try
                    {
                        foreach (var bval in func(aval).Values)
                        {
                            bpipeline.AddValue(bval);
                        }
                    }
                    catch (TransformationFailedException ex)
                    {
                        bpipeline.RecordFailure(ex);
                    }
                }
            }
            return bpipeline;
        }

        public static Pipeline<C> SelectMany<A, B, C>(Pipeline<A> a, Func<A, Pipeline<B>> func, Func<A, B, C> select)
            where A : INullable
            where B : INullable
            where C : INullable
        {
            return a.Bind(aval => func(aval).Bind(bval => select(aval, bval).ToPipeline()));
        }

        public static Pipeline<B> Select<A, B>(this Pipeline<A> a, Func<A, B> func)
            where A : INullable
            where B : INullable
        {
            return a.Bind(aval => func(aval).ToPipeline());
        }
    }

    public interface INullable
    {
        bool HasValue { get; }
    }

    public class Item : INullable
    {
        public bool HasValue { get; set; }
        public string Value { get; set; }
    }

    public class TransformedItem : INullable
    {
        public bool HasValue { get; set; }
        public int Value { get; set; }
    }

    [Serializable]
    public class TransformationFailedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TransformationFailedException()
        {
        }

        public TransformationFailedException(string message) : base(message)
        {
        }

        public TransformationFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TransformationFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}