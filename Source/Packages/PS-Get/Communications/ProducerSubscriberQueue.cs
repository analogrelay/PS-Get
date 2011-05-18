using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PsGet.Communications {
    public class ProducerSubscriberQueue<T> : IDisposable {
        private bool _active = true;
        private Queue<T> _queue = new Queue<T>();
        private Semaphore _sem = new Semaphore(0, Int32.MaxValue);
        private ManualResetEvent _evt = new ManualResetEvent(false);
        private object _lock = new object();

        ~ProducerSubscriberQueue() {
            Dispose(false);
        }

        public void Enqueue(T item) {
            lock (_lock) {
                EnsureActive();
                _queue.Enqueue(item);
                _sem.Release();
            }
        }

        public void Close() {
            lock (_lock) {
                EnsureActive();
                _active = false;
                _evt.Set();
            }
        }

        public T WaitForItem() {
            if (WaitHandle.WaitAny(new WaitHandle[] { _sem, _evt }) == 0) {
                // Item is available
                lock (_lock) {
                    return _queue.Dequeue();
                }
            }
            else {
                // Queue was closed
                throw new OperationCanceledException();
            }
        }

        private void EnsureActive() {
            if (!_active) {
                throw new InvalidOperationException("Cannot perform action, queue is closed");
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _sem.Close();
                _evt.Close();
            }
        }
    }
}
