using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace otoface
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupInputDialog : Window
    {
        public GroupInputDialog()
        {
            InitializeComponent();
        }

        public string Expression => txtExpression.Text;
        public string Key => txtKey.Text;
        public string FadeFrame => txtFadeFrame.Text;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
