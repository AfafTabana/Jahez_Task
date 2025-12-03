using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLendingTest.Services
{
    public static class AsyncTestHelper
    {
        public static IQueryable<T> ToAsyncQueryable<T>(this IEnumerable<T> source)
        {
            return new AsyncQueryable<T>(source);
        }

        private class AsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> _source;

            public AsyncQueryable(IEnumerable<T> source)
            {
                _source = source;
                Expression = source.AsQueryable().Expression;
                Provider = new AsyncQueryProvider<T>(source.AsQueryable().Provider);
            }

            public Type ElementType => typeof(T);
            public System.Linq.Expressions.Expression Expression { get; }
            public IQueryProvider Provider { get; }

            public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new AsyncEnumerator<T>(_source.GetEnumerator());
            }
        }

        private class AsyncQueryProvider<T> : IQueryProvider
        {
            private readonly IQueryProvider _inner;

            public AsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
            {
                return new AsyncQueryable<T>((IEnumerable<T>)_inner.CreateQuery(expression));
            }

            public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
            {
                return new AsyncQueryable<TElement>((IEnumerable<TElement>)_inner.CreateQuery<TElement>(expression));
            }

            public object Execute(System.Linq.Expressions.Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }
        }

        private class AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public AsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public T Current => _inner.Current;

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }
        }
    }
}

