using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace Net.SourceForge.Koogra.Excel2007
{
    public abstract class WorksheetChild
    {
        internal WorksheetChild(Worksheet ws, uint index)
        {
            Worksheet = ws;
            Index = index;
        }

        public uint Index { get; private set; }

        public Worksheet Worksheet { get; private set; }

    }
}
