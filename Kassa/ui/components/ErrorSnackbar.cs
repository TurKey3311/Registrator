using Kassa;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Registrator.ui.components
{
    internal class ErrorSnackbar
    {
        public void ShowErrorSnackbar(Form form, string message)
        {
            var snackbar = new MaterialSnackBar(message);
            snackbar.BackColor = Color.Red;
            snackbar.ForeColor = Color.White;
            snackbar.Show(form);
        }

        internal void ShowErrorSnackbar(Save save, string v)
        {
            throw new NotImplementedException();
        }
    }
}
