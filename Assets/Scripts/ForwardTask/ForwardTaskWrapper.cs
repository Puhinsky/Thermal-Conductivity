using DatabaseLibrary;
using QFSW.QC;
using UnityEngine;

namespace ForwardTask
{
    public class ForwardTaskWrapper : MonoBehaviour
    {
        [Command("thomas")]
        public void ExecuteForwardTask(string filePath)
        {
            var database = Database.Load(filePath);
            var runner = new ForwardTaskRunner();
            runner.Execute(database);
        }
    }
}
