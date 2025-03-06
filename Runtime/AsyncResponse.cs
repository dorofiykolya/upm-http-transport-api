using System;
using System.Collections.Generic;

namespace HttpTransport.Rpc
{
    public interface IAsyncResponse
    {
        bool Resolved { get; }
    }

    public interface IAsyncResponse<T> : IAsyncResponse
    {
        IDisposable Subscribe(Action<RequestResult<T>> callback);

        RequestResult<T> Result { get; }
    }

    public abstract class AsyncResponse : IDisposable
    {
        public abstract void Resolve(long statusCode, long errorCode, object value, bool connected);
        public abstract void Dispose();
    }

    public class AsyncResponse<T> : AsyncResponse, IAsyncResponse<T>
    {
        private List<Action<RequestResult<T>>> _actions = new List<Action<RequestResult<T>>>();
        private RequestResult<T> _result;
        private bool _resolved;

        public bool Resolved => _resolved;

        public RequestResult<T> Result => _result;

        public IDisposable Subscribe(Action<RequestResult<T>> callback)
        {
            var disposable = new Disposable(_actions, callback);
            if (_resolved)
            {
                callback(_result);
            }
            else if (_actions != null)
            {
                _actions.Add(callback);
            }

            return disposable;
        }

        public override void Resolve(long statusCode, long errorCode, object value, bool connected)
        {
            if (_resolved)
            {
                throw new InvalidOperationException();
            }

            _result = new RequestResult<T>(statusCode, errorCode, value, connected);

            _resolved = true;
            var actions = _actions.ToArray();
            _actions.Clear();
            foreach (var action in actions)
            {
                action(_result);
            }
        }

        public override void Dispose()
        {
            _actions.Clear();
            _actions = null;
            _resolved = false;
            _result = null;
        }

        class Disposable : IDisposable
        {
            private List<Action<RequestResult<T>>> _actions;
            private Action<RequestResult<T>> _callback;

            public Disposable(List<Action<RequestResult<T>>> actions, Action<RequestResult<T>> callback)
            {
                _actions = actions;
                _callback = callback;
            }

            public void Dispose()
            {
                if (_callback != null)
                {
                    if (_actions.Count != 0)
                    {
                        _actions.Remove(_callback);
                    }

                    _actions = null;
                    _callback = null;
                }
            }
        }
    }
}
