
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelExpertsUI.ModalForms
{

    /// <summary>
    /// Class helper/ Blueprint to hold product and supplier data.
    /// </summary>
    public class SelectedProdSup
    {

        public int prodsupid { get; set; }
        public int prodid { get; set; }
        public int supid { get; set; }
        public string prodname { get; set; }
        public string supnam { get; set; }

        public SelectedProdSup(int pid, int prdid, int suplid, string prdname, string sname)
        {
            prodsupid = pid;
            prodid = prdid;
            supid = suplid;
            prodname = prdname;
            supnam = sname;
      
        }

    
        //string representation of the class
        //each object will be represented by Prodname-Supname

        public override string ToString()
        {
            return prodname + " by " + supnam;
        }


    }


}
