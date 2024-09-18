using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STIMULUS_V2.Shared.Interface.ChildInterface
{
    public interface  ICompilationService
    {
        Task<string> CompileCSharpAsync(string code);
    }
}
