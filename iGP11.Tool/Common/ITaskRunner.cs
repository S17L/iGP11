using System;

namespace iGP11.Tool.Common
{
    public interface ITaskRunner
    {
        void Run(Action action);
    }
}