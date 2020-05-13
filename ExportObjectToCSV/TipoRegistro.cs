using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Rendimento.Layout
{
    public class TipoRegistro
    {


        public int ID { set; get; }

        public int IdLayout { set; get; }

        public string Nome { set; get; }

        public string Descricao { set; get; }

        public List<Campo> Campos { set; get; }


        public TipoRegistro()
        {

        }

        public TipoRegistro(int idTpReg, int layoutId, string nm, string desc, List<Campo> lstCampos)
        {
            this.ID = idTpReg;
            this.IdLayout = layoutId;
            this.Nome = nm;
            this.Descricao = desc;
            this.Campos = lstCampos;
        }

        public static List<TipoRegistro> getTipoRegistros()
        {

            IEnumerable<TipoRegistro> l = null;
            List<TipoRegistro> tp = new List<TipoRegistro>();

            try
            {

                using (SqlConnection conn = new SqlConnection("Data Source=REND-TESSQL-001;Initial Catalog=AB_INFOBANC;User ID=USER_CA;Password=USER_CA"))
                {
                    string comando = "SELECT ID_TIPO_REGISTRO AS ID, ID_LAYOUT AS IdLayout, NOME AS Nome, DESCRICAO AS Descricao FROM TIPOS_REGISTROS_LAYOUT ORDER BY ID_LAYOUT, ID_TIPO_REGISTRO ";
                    l = conn.Query<TipoRegistro>(comando);
                }

                foreach(TipoRegistro tr in l)
                {
                    tp.Add(new TipoRegistro(tr.ID, tr.IdLayout, tr.Nome, tr.Descricao, Campo.CamposByRegistro(tr.ID)));
                }

                return tp;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public static List<TipoRegistro> getTipoRegistros(List<Campo> campos)
        {

            IEnumerable<TipoRegistro> l = null;
            List<TipoRegistro> tp = new List<TipoRegistro>();

            try
            {

                using (SqlConnection conn = new SqlConnection("Data Source=REND-TESSQL-001;Initial Catalog=AB_INFOBANC;User ID=USER_CA;Password=USER_CA"))
                {
                    string comando = "SELECT ID_TIPO_REGISTRO AS IdTipoRegistro, ID_LAYOUT AS Idlayout, NOME AS Nome, DESCRICAO AS Descricao FROM TIPOS_REGISTROS_LAYOUT ORDER BY ID_LAYOUT, ID_TIPO_REGISTRO ";
                    l = conn.Query<TipoRegistro>(comando);
                }

                foreach (TipoRegistro tr in l)
                {
                    tp.Add(new TipoRegistro(tr.ID, tr.IdLayout, tr.Nome, tr.Descricao, campos.Where(x => x.IdTipoRegistro == tr.ID).ToList<Campo>()));
                }

                return tp;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }


        public static TipoRegistro getTipoRegistro(int idTipoRegistro)
        {
            return (getTipoRegistros().Where<TipoRegistro>(x => x.ID == idTipoRegistro).FirstOrDefault());
        }

        public static List<TipoRegistro> getTipoRegistroByLayout(int idLayout)
        {
            return (getTipoRegistros().Where<TipoRegistro>(x => x.IdLayout == idLayout).ToList<TipoRegistro>());
        }

    }
}
