using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionExtraction
{
    class ExpressionExtraction
    {
        internal void Start()
        {
            BrownCorpusRead bcr = new BrownCorpusRead("ca06");
            bcr.Start();
        }
    }
}
