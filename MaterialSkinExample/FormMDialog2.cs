using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace MaterialSkinExample
{
    public partial class FormMDialog2 : MaterialDialog2
    {
        public FormMDialog2()
        {
            InitializeComponent();
        }

        public FormMDialog2(Form parentForm):base(parentForm)
        {
            InitializeComponent();
        }
    }
}
