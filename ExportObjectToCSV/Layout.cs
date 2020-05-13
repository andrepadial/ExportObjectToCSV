using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace Rendimento.Layout
{
    public class Layout
    {

        public int ID { set; get; }

        public string Nome { set; get; }

        public string Descricao { set; get; }

        public List<TipoRegistro> TipoReg { set; get; }



        public Layout()
        {

        }

        public Layout (int id, string nomeLayout, string descLayout, List<TipoRegistro> tpReg)
        {
            this.ID = id;
            this.Nome = nomeLayout;
            this.Descricao = descLayout;
            this.TipoReg = tpReg;
        }

        public static List<Layout> getLayouts()
        {
            IEnumerable<Layout> l = null;
            List<Layout> lay = new List<Layout>();


            try
            {

                using (SqlConnection conn = new SqlConnection("Data Source=REND-TESSQL-001;Initial Catalog=AB_INFOBANC;User ID=USER_CA;Password=USER_CA"))
                {
                    string comando = "SELECT ID_LAYOUT AS ID, NOME AS Nome, DESCRICAO AS Descricao FROM LAYOUTS_ARQUIVOS ORDER BY ID_LAYOUT ";
                    l = conn.Query<Layout>(comando);
                }

                foreach(Layout la in l)
                {
                    lay.Add(new Layout(la.ID, la.Nome, la.Descricao, TipoRegistro.getTipoRegistroByLayout(la.ID)));
                }


                return lay;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public static Layout getLayout(int idLayout)
        {
            return (getLayouts()).Where(x => x.ID == idLayout).FirstOrDefault<Layout>();
        }

        public static Layout getLayout(string nm)
        {
            return (getLayouts()).Where(x => x.Nome.Contains(nm)).FirstOrDefault<Layout>();
        }
    }
}
