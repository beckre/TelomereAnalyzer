using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelomereAnalyzer
{
    class DisplayingFinalResults
    {
        Nuclei _allNuclei = null;
        List<Nucleus> _nuclei = null;
        AllTelomeres _allTelomeres = null;

        public DisplayingFinalResults(Nuclei nuclei, AllTelomeres allTelomeres)
        {
            this._allNuclei = nuclei;
            _nuclei = _allNuclei._LstallNuclei;
            this._allTelomeres = allTelomeres;
        }

        public void PrintResultsOnConsole()
        {
            
            for(Int32 n = 0; n < _nuclei.Count; n++)
            {
                List<Telomere> telomeres = _nuclei[n]._LstnucleusTelomeres;
                Console.WriteLine("Nucleus " + _nuclei[n]._nucleusName + " beinhaltet " + telomeres.Count + " Telomere.\n");
                for (Int32 t = 0; t < telomeres.Count; t++)
                {
                    Console.WriteLine("Telomer " + telomeres[t]._telomereName + " Mittelpunkt X-Koordinate: " + telomeres[t]._telomereCenterPoint.X + " Y-Koordinate: " + telomeres[t]._telomereCenterPoint.Y);
                }
                Console.WriteLine("\n");
            }
        }
    }
}
