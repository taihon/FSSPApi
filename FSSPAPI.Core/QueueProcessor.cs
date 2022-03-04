using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FSSPAPI.Core
{
    public class QueueProcessor
    {
        internal Queue<Person> _personQueue = new Queue<Person>();
        internal List<Person> _results = new List<Person>();
        internal State _currentState;
        internal string currentBatchGuid = string.Empty;
        internal APIExchanger _exchanger;
        internal bool _finished;
        internal int _batchSize = 50;

        internal void log(string message)
        {
            Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {message}");
        }
        internal void SetState(State st)
        {
            if(null != _currentState)
                _currentState.OnLeave();
            _currentState = st;
            st.OnEnter();
        }
        public List<Person> Process(List<Person> l)
        {
            _personQueue.Clear();
            _results.Clear();
            foreach (var person in l)
            {
                _personQueue.Enqueue(person);
            }
            var st = new IdleState();
            st.SetContext(this);
            this.SetState(st);
            _results.AddRange(l);
            if (_personQueue.Count == 0)
            {
                return null;
            }
            while (!_finished) {
                log($"current state is {_currentState.GetType().Name}");
                _currentState.Process();
            }
            return _results;
        }
        public QueueProcessor()
        {
            _exchanger = new APIExchanger();
            var idle = new IdleState();
            idle.SetContext(this);
            SetState(idle);
        }
    }
    internal abstract class State
    {
        protected QueueProcessor _context;
        public void SetContext(QueueProcessor ctx)
        {
            this._context = ctx;
        }
        internal abstract void OnEnter();

        internal abstract void OnLeave();
        internal abstract void Process();
    }
    internal class IdleState : State
    {

        internal override void OnEnter()
        {
            return;
        }

        internal override void OnLeave()
        {
            return;
        }

        internal override void Process()
        {
            if (_context._personQueue.Count > 0)
            {
                State st = new RequestNextBatchState();
                st.SetContext(_context);
                _context.SetState(st);
            }
            return;
        }
    }
    class PausedState : State
    {
        internal override void OnEnter()
        {
            return;
        }

        internal override void OnLeave()
        {
            return;
        }

        internal override void Process()
        {
            int SLEEP_SECONDS = 10000;
            Thread.Sleep(SLEEP_SECONDS);
            State st;
            if (_context.currentBatchGuid != string.Empty)
            {
                _context.log($"need to check {_context.currentBatchGuid}");
                st = new RequestBatchRequestStatusState();
            }
            else
            {
                if(_context._personQueue.Count > 0)
                {
                    st = new RequestNextBatchState();
                }
                else
                {
                    st = new FinishedState();
                }
            }
            st.SetContext(_context);
            _context.SetState(st);
        }
    }
    class ProcessState : State
    {
        internal override void OnEnter()
        {
            throw new NotImplementedException();
        }

        internal override void OnLeave()
        {
            throw new NotImplementedException();
        }

        internal override void Process()
        {
            throw new NotImplementedException();
        }
    }
    class RequestBatchRequestStatusState : State
    {
        internal override void OnEnter()
        {
            return;
        }

        internal override void OnLeave()
        {
            return;
        }

        internal override void Process()
        {
            //request batch status from api
            var status = _context._exchanger.GroupRequestStatusIsCompleted(_context.currentBatchGuid).GetAwaiter().GetResult();
            State st;
            if (status)
            {
                st = new ProcessReadyBatchState();
            }
            else
            {
                st = new PausedState();
            }
            st.SetContext(_context);
            _context.SetState(st);
        }
    }

    internal class ProcessReadyBatchState : State
    {
        internal override void OnEnter()
        {
            return;
        }

        internal override void OnLeave()
        {
            return;
        }

        internal override void Process()
        {
            try
            {
                var res = _context._exchanger.FetchReadyGroupRequest(_context.currentBatchGuid).GetAwaiter().GetResult();
                var batch = _context._results.Where(b => b.BatchId == _context.currentBatchGuid);
                foreach (var resrow in res)
                {
                    var nameSegments = resrow.Name.Split(' ');
                    var firstN = nameSegments[1];
                    var lastN = nameSegments[0];
                    var day = nameSegments[3];
                    var person = batch.FirstOrDefault(p => p.FirstName.ToUpper() == firstN && p.LastName.ToUpper() == lastN && p.BirthDate == day);
                    if (null != person)
                    {
                        person.Results.Add(resrow);
                    }
                }
                _context.currentBatchGuid = string.Empty;
            }
            catch(Exception e)
            {
                _context.log($"error while processing - {e.Message}");
            }
            State st = new PausedState();
            st.SetContext(_context);
            _context.SetState(st);
        }
    }

    internal class RequestNextBatchState : State
    {
        internal override void OnEnter()
        {
            return;
        }

        internal override void OnLeave()
        {
            return;
        }

        internal override void Process()
        {
            var batch = _context._personQueue.DequeueChunk(_context._batchSize).ToList();

            var id = _context._exchanger.SendGroupRequestAsync(batch).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(id)) {
                _context.currentBatchGuid = id;
                foreach(var p in batch)
                {
                    p.BatchId = id;
                }
            }
            else
            {
                foreach(var p in batch)
                {
                    _context._personQueue.Enqueue(p);
                }
            }
            var st = new PausedState();
            st.SetContext(_context);
            _context.SetState(st);
        }
    }

    class FinishedState : State
    {
        internal override void OnEnter()
        {
            
        }

        internal override void OnLeave()
        {
            
        }

        internal override void Process()
        {
            _context._finished = true;
        }
    }
}
