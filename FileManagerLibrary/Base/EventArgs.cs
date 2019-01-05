using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileManagerLibrary.Base
{
    public class GenericEventArgsType <TData> : EventArgs
    {
        public GenericEventArgsType(TData data)
        {
            Data = data;
        }
        public TData Data { get; private set; }
    }
}
