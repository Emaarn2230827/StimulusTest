using STIMULUS_V2.Shared.Interface.ParentInterface;
using STIMULUS_V2.Shared.Models.DTOs;
using STIMULUS_V2.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STIMULUS_V2.Shared.Interface.ChildInterface
{
    //permet la récupération du compteur de Console.ReadLine et du tableau contenant les valeurs des readLine
    public class ExecuteCodeRequest
    {
        public string Code { get; set; }
        public int Cpt { get; set; }
        public string[] DataRead { get; set; }

        public ExecuteCodeRequest()
        {
            Code = null;
            Cpt = 0;
            DataRead = null;
        }
    }

    public interface IExerciceService : IModelService<Exercice, int>
    {
        Task<APIResponse<string>> ExecuteCode(ExecuteCodeRequest request);

    }
}
