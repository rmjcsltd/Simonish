using System;
using Rmjcs.Simonish.Models;

namespace Rmjcs.Simonish.Services
{
    interface INewResultSource
    {
        event EventHandler<NewResultEventArgs> NewResult;
    }

    internal class NewResultEventArgs : EventArgs
    {
        public Result Result { get; set; }
    }

}
