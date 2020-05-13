using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rendimento.Layout
{
    public class Load
    {

        public List<Campo> Campos { set; get; }

        public List<TipoRegistro> TiposRegistros { set; get; }

        public List<Layout> Layouts { set; get; }


        public Load()
        {
        }
        
        
        public void loadData()
        {
            this.Campos = Campo.getCampos();
            this.TiposRegistros = TipoRegistro.getTipoRegistros();
            this.Layouts = Layout.getLayouts();
        }
    }
}
