using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace Rendimento.Layout
{
    public class Campo
    {

        public int IdCampo { set; get; }


        public int IdTipoRegistro { set; get; }

        public string Nome { set; get; }

        public string Descricao { set; get; }
        public int Inicio { set; get; }
        public int Fim { set; get; }
        
        public int Tamanho { set; get; }

        public Campo()
        {

        }

        public Campo (int idCampo, int idTpRegistro, string nm, string desc, int posInicio, int posFim, int tam)
        {
            this.IdCampo = idCampo;
            this.IdTipoRegistro = idTpRegistro;
            this.Nome = nm;
            this.Descricao = desc;
            this.Inicio = posInicio;
            this.Fim = posFim;
            this.Tamanho = tam;
        }

        public static List<Campo> getCampos()
        {
            IEnumerable<Campo> l = null;


            try
            {

                using (SqlConnection conn = new SqlConnection("Data Source=REND-TESSQL-001;Initial Catalog=AB_INFOBANC;User ID=USER_CA;Password=USER_CA"))
                {
                    string comando = " SELECT ID_CAMPO AS IdCampo, ID_TIPO_REGISTRO AS IdTipoRegistro, NOME AS Nome, ";
                    comando += " DESCRICAO AS Descricao, POSICAOINICIO AS Inicio, POSICAOFINAL AS Fim, (POSICAOFINAL - POSICAOINICIO) + 1 AS 'Tamanho'  ";
                    comando += " FROM CAMPOS_LAYOUT ";
                    comando += " ORDER BY    ID_TIPO_REGISTRO, POSICAOINICIO, POSICAOFINAL ";

                    l = conn.Query<Campo>(comando);
                }


                return new List<Campo>(l);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        public static Campo getCampo(int idCampo)
        {
            return (getCampos()).Where(x => x.IdCampo == idCampo).FirstOrDefault<Campo>();
        }

        public static List<Campo> CamposByRegistro(int idTipoRegistro)
        {
            return (getCampos()).Where(x => x.IdTipoRegistro == idTipoRegistro).ToList<Campo>();
        }

    }
}
