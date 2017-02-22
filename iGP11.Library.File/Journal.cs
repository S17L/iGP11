using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace iGP11.Library.File
{
    internal class Journal : IEnlistmentNotification
    {
        private readonly DirectoryContext _directoryContext;
        private readonly ICollection<IOperation> _operations = new List<IOperation>();

        public Journal(DirectoryContext directoryContext)
        {
            _directoryContext = directoryContext;
        }

        public void Add(IOperation operation)
        {
            operation.Commit();
            _operations.Add(operation);
        }

        void IEnlistmentNotification.Commit(Enlistment enlistment)
        {
            _operations.Clear();
            Done(enlistment);
        }

        void IEnlistmentNotification.InDoubt(Enlistment enlistment)
        {
            Discard(enlistment);
        }

        void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        void IEnlistmentNotification.Rollback(Enlistment enlistment)
        {
            Discard(enlistment);
        }

        private void Discard(Enlistment enlistment)
        {
            try
            {
                foreach (var operation in _operations.Reverse())
                {
                    operation.Rollback();
                }

                Done(enlistment);
            }
            catch (Exception exception)
            {
                _directoryContext.Remove(this);
                throw new TransactionException("transaction failed to roll back", exception);
            }
        }

        private void Done(Enlistment enlistment)
        {
            try
            {
                enlistment.Done();
            }
            finally
            {
                _directoryContext.Remove(this);
            }
        }
    }
}